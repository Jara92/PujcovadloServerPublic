using System.Net;
using IntegrationTests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using PujcovadloServer;
using PujcovadloServer.Business.Enums;
using PujcovadloServer.Data;
using PujcovadloServer.Requests;
using PujcovadloServer.Responses;
using Xunit;
using Xunit.Abstractions;
using Assert = NUnit.Framework.Assert;

namespace IntegrationTests.IntegrationTests.Areas.Api.Controllers.LoanController;

public class AcceptedLoanActionsTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _application;
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;
    private TestData _data;

    private LoanUpdateRequest _loanUpdateRequest;

    private readonly string _apiPath = "/api/loans/";

    public AcceptedLoanActionsTests(CustomWebApplicationFactory<Program> factory, ITestOutputHelper output)
    {
        _application = factory;
        _client = _application.CreateClient();
        _output = output;
        _data = new TestData();

        // Arrange
        using (var scope = _application.Services.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<PujcovadloServerContext>();
            Utilities.ReinitializeDbForTests(db, _data);
        }

        // Define example update request
        _loanUpdateRequest = new LoanUpdateRequest()
        {
            Id = _data.Loan1.Id,
            Status = LoanStatus.Accepted
        };
    }

    [Fact]
    public async Task Cancel_OwnerCancelsAcceptedLoan_Ok()
    {
        UserHelper.SetAuthorizationHeader(_client, UserHelper.OwnerToken);

        // Set loan id and new status
        _loanUpdateRequest.Id = _data.LoanAccepted.Id;
        _loanUpdateRequest.Status = LoanStatus.Cancelled;

        // Perform the action
        var response = await _client.PutAsJsonAsync($"{_apiPath}{_loanUpdateRequest.Id}", _loanUpdateRequest);

        // Check http status
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        // Check if the loan status was updated
        var updatedLoan = await response.Content.ReadAsAsync<LoanResponse>();
        Assert.That(updatedLoan.Status, Is.EqualTo(_loanUpdateRequest.Status));
    }

    [Fact]
    public async Task Cancel_TenantCancelsAcceptedLoan_Ok()
    {
        UserHelper.SetAuthorizationHeader(_client, UserHelper.TenantToken);

        // Set loan id and new status
        _loanUpdateRequest.Id = _data.LoanAccepted.Id;
        _loanUpdateRequest.Status = LoanStatus.Cancelled;

        // Perform the action
        var response = await _client.PutAsJsonAsync($"{_apiPath}{_loanUpdateRequest.Id}", _loanUpdateRequest);

        // Check http status
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        // Check if the loan status was updated
        var updatedLoan = await response.Content.ReadAsAsync<LoanResponse>();
        Assert.That(updatedLoan.Status, Is.EqualTo(_loanUpdateRequest.Status));
    }

    [Fact]
    public async Task PrepareForPickup_OwnerPreparesForPickupButPickupProtocolIsNotSet_UnprocessableEntity()
    {
        UserHelper.SetAuthorizationHeader(_client, UserHelper.OwnerToken);

        // Set loan id and new status
        _loanUpdateRequest.Id = _data.LoanAccepted.Id;
        _loanUpdateRequest.Status = LoanStatus.PreparedForPickup;

        // Perform the action
        var response = await _client.PutAsJsonAsync($"{_apiPath}{_loanUpdateRequest.Id}", _loanUpdateRequest);

        // Check http status
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.UnprocessableEntity));
    }

    [Fact]
    public async Task PrepareForPickup_OwnerPreparesForPickup_Ok()
    {
        UserHelper.SetAuthorizationHeader(_client, UserHelper.OwnerToken);

        // Set loan id and new status
        _loanUpdateRequest.Id = _data.LoanAcceptedWithPickupProtocol.Id;
        _loanUpdateRequest.Status = LoanStatus.PreparedForPickup;

        // Perform the action
        var response = await _client.PutAsJsonAsync($"{_apiPath}{_loanUpdateRequest.Id}", _loanUpdateRequest);

        // Check http status
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        // Check if the loan status was updated
        var updatedLoan = await response.Content.ReadAsAsync<LoanResponse>();
        Assert.That(updatedLoan.Status, Is.EqualTo(_loanUpdateRequest.Status));
    }

    [Fact]
    public async Task Activate_OwnerActivatesTheLoan_Ok()
    {
        UserHelper.SetAuthorizationHeader(_client, UserHelper.OwnerToken);

        // Set loan id and new status
        _loanUpdateRequest.Id = _data.LoanAccepted.Id;
        _loanUpdateRequest.Status = LoanStatus.Active;

        // Perform the action
        var response = await _client.PutAsJsonAsync($"{_apiPath}{_loanUpdateRequest.Id}", _loanUpdateRequest);

        // Check http status
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        // Check if the loan status was updated
        var updatedLoan = await response.Content.ReadAsAsync<LoanResponse>();
        Assert.That(updatedLoan.Status, Is.EqualTo(_loanUpdateRequest.Status));
    }

    [Fact]
    public async Task DisallowedActions_OwnerTriesToChangeStatusToDisallowed_UnprocessableEntity()
    {
        UserHelper.SetAuthorizationHeader(_client, UserHelper.OwnerToken);

        // Set loan id and new status
        _loanUpdateRequest.Id = _data.LoanAccepted.Id;

        var disallowedStatuses = new List<LoanStatus>
        {
            LoanStatus.Inquired,
            LoanStatus.Denied,
            LoanStatus.PickupDenied,
            LoanStatus.PreparedForReturn,
            LoanStatus.ReturnDenied,
            LoanStatus.Returned,
        };

        foreach (var status in disallowedStatuses)
        {
            _loanUpdateRequest.Status = status;

            // Perform the action
            var response = await _client.PutAsJsonAsync($"{_apiPath}{_loanUpdateRequest.Id}", _loanUpdateRequest);

            // Check http status
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.UnprocessableEntity));
        }
    }

    [Fact]
    public async Task DisallowedActions_TenantTriesToChangeStatusToDisallowed_UnprocessableEntity()
    {
        UserHelper.SetAuthorizationHeader(_client, UserHelper.TenantToken);

        // Set loan id and new status
        _loanUpdateRequest.Id = _data.LoanAccepted.Id;

        var disallowedStatuses = new List<LoanStatus>
        {
            LoanStatus.Inquired,
            LoanStatus.Denied,
            LoanStatus.PreparedForPickup,
            LoanStatus.PickupDenied,
            LoanStatus.Active,
            LoanStatus.PreparedForReturn,
            LoanStatus.ReturnDenied,
            LoanStatus.Returned,
        };

        foreach (var status in disallowedStatuses)
        {
            _loanUpdateRequest.Status = status;

            // Perform the action
            var response = await _client.PutAsJsonAsync($"{_apiPath}{_loanUpdateRequest.Id}", _loanUpdateRequest);

            // Check http status
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.UnprocessableEntity));
        }
    }
}