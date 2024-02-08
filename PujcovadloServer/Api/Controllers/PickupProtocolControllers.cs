using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PujcovadloServer.Api.Filters;
using PujcovadloServer.AuthorizationHandlers;
using PujcovadloServer.Business.Entities;
using PujcovadloServer.Business.Enums;
using PujcovadloServer.Business.Facades;
using PujcovadloServer.Requests;
using PujcovadloServer.Responses;

namespace PujcovadloServer.Api.Controllers;

[ApiController]
[Route("api/loans/{loanId}/pickup-protocol")]
[Authorize(Roles = UserRoles.Owner)]
[ServiceFilter(typeof(ExceptionFilter))]
public class PickupProtocolControllers : ACrudController<PickupProtocol>
{
    private readonly LoanFacade _loanFacade;
    private readonly OwnerFacade _ownerFacade;
    private readonly IMapper _mapper;

    public PickupProtocolControllers(LoanFacade loanFacade, OwnerFacade ownerFacade,
        IAuthorizationService authorizationService,
        LinkGenerator urlHelper, IMapper mapper) : base(authorizationService, urlHelper)
    {
        _loanFacade = loanFacade;
        _ownerFacade = ownerFacade;
        _mapper = mapper;
    }

    /// <summary>
    /// Create a new PickupProtocol for the loan.
    /// </summary>
    /// <param name="loanId">Loan id</param>
    /// <param name="request">Pickup protocol data</param>
    /// <returns>Created pickup protocol.</returns>
    /// <response code="201">Created pickup protocol.</response>
    /// <response code="400">Invalid input data or action is not allowed.</response>
    /// <response code="403">User is not allowed to create a pickup protocol for the loan.</response>
    /// <response code="404">Loan not found.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PickupProtocolResponse>> CreateProtocol(int loanId,
        [FromBody] PickupProtocolRequest request)
    {
        // Get loan
        var loan = await _loanFacade.GetLoan(loanId);
        await CheckPermissions<Loan>(loan, LoanAuthorizationHandler.Operations.CreatePickupProtocol);

        // Create protocol
        var protocol = await _ownerFacade.CreatePickupProtocol(loan, request);

        // build response
        var response = _mapper.Map<PickupProtocolResponse>(protocol);

        // HATEOS links
        return CreatedAtAction(_urlHelper.GetUriByAction(HttpContext, nameof(GetProtocol), values: protocol.Id), response);
    }

    /// <summary>
    /// Returns pickup protocol for the loan.
    /// </summary>
    /// <param name="loanId">Id of the loan</param>
    /// <returns>Pickup protocol.</returns>
    /// <response code="200">Returns pickup protocol.</response>
    /// <response code="403">User is not allowed to read the pickup protocol.</response>
    /// <response code="404">Loan not found.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)] 
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PickupProtocolResponse>> GetProtocol(int loanId)
    {
        // Get loan
        var loan = await _loanFacade.GetLoan(loanId);
        await CheckPermissions<Loan>(loan, LoanAuthorizationHandler.Operations.Read);

        // Get protocol
        var protocol = _loanFacade.GetPickupProtocol(loan);
        await CheckPermissions(protocol, PickupProtocolAuthorizationHandler.Operations.Read);

        // build response
        var response = _mapper.Map<PickupProtocolResponse>(protocol);

        // HATEOS links
        // TODO: add loan link later. the problem is that we have two loan controllers for owner and tenant and we need to distinguish them
        /*response.Links.Add(new LinkResponse(
            _urlHelper.GetUriByAction(HttpContext, nameof(), values: new { loan.Id }),
            "LOAN", "GET"));*/

        // Add update link if user has permission
        if (await CanPerformOperation(protocol, PickupProtocolAuthorizationHandler.Operations.Update))
        {
            response.Links.Add(new LinkResponse(
                _urlHelper.GetUriByAction(HttpContext, nameof(UpdateProtocol), values: new { loanId }),
                "UPDATE", "PUT"));
            
            // todo: add images link
            
            // todo: add link for creating a new image
        }

        return Ok(response);
    }

    /// <summary>
    /// Updates pickup protocol for the loan.
    /// </summary>
    /// <param name="loanId">Loan id</param>
    /// <param name="request">Pickup protocol data.</param>
    /// <returns></returns>
    /// <response code="204">Pickup protocol updated.</response>
    /// <response code="400">Invalid input data or action is not allowed.</response>
    /// <response code="403">User is not allowed to update the pickup protocol.</response>
    /// <response code="404">Loan not found.</response>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PickupProtocolResponse>> UpdateProtocol(int loanId,
        [FromBody] PickupProtocolRequest request)
    {
        // Get loan
        var loan = await _loanFacade.GetLoan(loanId);
        await CheckPermissions<Loan>(loan, LoanAuthorizationHandler.Operations.Read);

        // Get protocol
        var protocol = _loanFacade.GetPickupProtocol(loan);
        await CheckPermissions(protocol, PickupProtocolAuthorizationHandler.Operations.Update);

        // Update protocol
        await _ownerFacade.UpdatePickupProtocol(protocol, request);

        return NoContent();
    }
}