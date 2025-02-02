using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Moq;
using PujcovadloServer.Authentication;
using PujcovadloServer.AuthorizationHandlers.Image;
using PujcovadloServer.Business.Enums;
using PujcovadloServer.Business.Services.Interfaces;

namespace Tests.AuthorizationHandlers.Image;

public class ImageOwnerAuthorizationHandlerTest
{
    ImageOwnerAuthorizationHandler _authorizationHandler;
    Mock<IAuthenticateService> _authenticateService;

    /// <summary>
    /// Unauthenticated user which is completely anonymous.
    /// </summary>
    ClaimsPrincipal _unauthenticatedUser;

    private string? _unauthenticatedUserId = null;

    /// <summary>
    /// Authenticated user with the User role.
    /// </summary>
    ClaimsPrincipal _user;

    string _userId = "1";

    /// <summary>
    /// Authenticated user with the Owner role and is owner of the loan.
    /// </summary>
    ClaimsPrincipal _owner;

    private string _ownerId = "2";

    /// <summary>
    /// Authenticated user with the Tentant role and the tenant of the loan.
    /// </summary>
    ClaimsPrincipal _tenant;

    private string _tenantId = "3";

    /// <summary>
    /// Auhenticated user with the Admin role.
    /// </summary>
    ClaimsPrincipal _admin;

    string _adminId = "4";

    private PujcovadloServer.Business.Entities.Item _item;

    private PujcovadloServer.Business.Entities.Loan _loan;

    private PujcovadloServer.Business.Entities.PickupProtocol _pickupProtocol;

    private PujcovadloServer.Business.Entities.ReturnProtocol _returnProtocol;

    private PujcovadloServer.Business.Entities.Image _image;


    [SetUp]
    public void Setup()
    {
        _authenticateService = new Mock<IAuthenticateService>();
        _authorizationHandler = new ImageOwnerAuthorizationHandler(_authenticateService.Object);

        _unauthenticatedUser = new ClaimsPrincipal();
        _user = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
        {
            new Claim(ClaimTypes.Name, "ned.flanders"), new Claim(ClaimTypes.Sid, _userId),
            new Claim(ClaimTypes.Role, UserRoles.User)
        }));
        _owner = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
        {
            new Claim(ClaimTypes.Name, "marge.simpson"), new Claim(ClaimTypes.Sid, _ownerId),
            new Claim(ClaimTypes.Role, UserRoles.Owner), new Claim(ClaimTypes.Role, UserRoles.User)
        }));
        _tenant = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
        {
            new Claim(ClaimTypes.Name, "bart.simpson"), new Claim(ClaimTypes.Sid, _tenantId),
            new Claim(ClaimTypes.Role, UserRoles.Tenant), new Claim(ClaimTypes.Role, UserRoles.User)
        }));
        _admin = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
        {
            new Claim(ClaimTypes.Name, "homer.simpson"), new Claim(ClaimTypes.Sid, _adminId),
            new Claim(ClaimTypes.Role, UserRoles.Admin)
        }));

        _item = new PujcovadloServer.Business.Entities.Item
        {
            Owner = new ApplicationUser { Id = _ownerId }
        };

        _loan = new PujcovadloServer.Business.Entities.Loan
        {
            Item = _item,
            Tenant = new ApplicationUser { Id = _tenantId }
        };

        _pickupProtocol = new PujcovadloServer.Business.Entities.PickupProtocol
        {
            Description = "Description",
        };

        _returnProtocol = new PujcovadloServer.Business.Entities.ReturnProtocol
        {
            Description = "Description",
        };

        _image = new PujcovadloServer.Business.Entities.Image
        {
            Owner = new ApplicationUser { Id = _ownerId },
            Item = _item,
            Name = "TestImage.jpg",
            Extension = ".jpg",
            MimeType = "image/jpeg",
        };
    }

    #region NoOwnerUsers

    [Test]
    public async Task HandleAsync_UserDoesIsNotTheOwnerAndImageBelongsToItem_ReturnsFalse()
    {
        // arrange
        _image.Item = _item;

        var users = new List<ClaimsPrincipal> { _unauthenticatedUser, _user, _tenant, _admin };
        var requirements = new List<IAuthorizationRequirement> { ImageOperations.Read, ImageOperations.Delete };

        foreach (var user in users)
        {
            // Mock user
            _authenticateService.Setup(x => x.TryGetCurrentUserId()).Returns(user.FindFirstValue(ClaimTypes.Sid));

            // act - every requirement should fail
            foreach (var requirement in requirements)
            {
                var context = new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, user,
                    _image);
                await _authorizationHandler.HandleAsync(context);
                Assert.That(context.HasSucceeded, Is.False);
            }
        }
    }

    [Test]
    public async Task HandleAsync_UserIsNotTheOwnerAndImageBelongsToPickupProtocol_ReturnsFalse()
    {
        // arrange
        _loan.Item = _item;
        _pickupProtocol.Loan = _loan;
        _image.PickupProtocol = _pickupProtocol;

        var users = new List<ClaimsPrincipal> { _unauthenticatedUser, _user, _tenant, _admin };
        var requirements = new List<IAuthorizationRequirement> { ImageOperations.Read, ImageOperations.Delete };

        foreach (var user in users)
        {
            // Mock user
            _authenticateService.Setup(x => x.TryGetCurrentUserId()).Returns(user.FindFirstValue(ClaimTypes.Sid));

            // act - every requirement should fail
            foreach (var requirement in requirements)
            {
                var context = new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, user,
                    _image);
                await _authorizationHandler.HandleAsync(context);
                Assert.That(context.HasSucceeded, Is.False);
            }
        }
    }

    [Test]
    public async Task HandleAsync_UserIsNotTheOwnerAndImageBelongsToReturnProtocol_ReturnsFalse()
    {
        // arrange
        _loan.Item = _item;
        _returnProtocol.Loan = _loan;
        _image.ReturnProtocol = _returnProtocol;

        var users = new List<ClaimsPrincipal> { _unauthenticatedUser, _user, _tenant, _admin };
        var requirements = new List<IAuthorizationRequirement> { ImageOperations.Read, ImageOperations.Delete };

        foreach (var user in users)
        {
            // Mock user
            _authenticateService.Setup(x => x.TryGetCurrentUserId()).Returns(user.FindFirstValue(ClaimTypes.Sid));

            // act - every requirement should fail
            foreach (var requirement in requirements)
            {
                var context = new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, user,
                    _image);
                await _authorizationHandler.HandleAsync(context);
                Assert.That(context.HasSucceeded, Is.False);
            }
        }
    }

    #endregion

    #region ReadAndDelete

    [Test]
    public async Task Read_UserIsOwner_ReturnsTrue()
    {
        // No matter whether the image belongs to a pickup protocol or return protocol or item, the owner can do it.

        var requirements = new List<IAuthorizationRequirement> { ImageOperations.Read, ImageOperations.Delete };

        // Mock user
        _authenticateService.Setup(x => x.TryGetCurrentUserId()).Returns(_owner.FindFirstValue(ClaimTypes.Sid));

        // act - every requirement should fail
        foreach (var requirement in requirements)
        {
            var context =
                new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, _owner, _image);
            await _authorizationHandler.HandleAsync(context);
            Assert.That(context.HasSucceeded, Is.True);
        }
    }

    #endregion
}