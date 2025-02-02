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

public class InquiredLoanActionsTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _application;
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;
    private TestData _data;

    private LoanUpdateRequest _loanUpdateRequest;

    private readonly string _apiPath = "/api/loans/";

    public InquiredLoanActionsTests(CustomWebApplicationFactory<Program> factory, ITestOutputHelper output)
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
    public async Task Cancel_OwnerTriesToCancelInquiredLoan_UnprocessableEntity()
    {
        UserHelper.SetAuthorizationHeader(_client, UserHelper.OwnerToken);

        // Set loan id and new status
        _loanUpdateRequest.Id = _data.LoanInquired.Id;
        _loanUpdateRequest.Status = LoanStatus.Cancelled;

        // Perform the action
        var response = await _client.PutAsJsonAsync($"{_apiPath}{_loanUpdateRequest.Id}", _loanUpdateRequest);

        // Check http status
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.UnprocessableEntity));
    }

    [Fact]
    public async Task Cancel_TenantCancellsInquiredLoan_Ok()
    {
        UserHelper.SetAuthorizationHeader(_client, UserHelper.TenantToken);

        // Set loan id and new status
        _loanUpdateRequest.Id = _data.LoanInquired.Id;
        _loanUpdateRequest.Status = LoanStatus.Cancelled;

        // Perform the action
        var response = await _client.PutAsJsonAsync($"{_apiPath}{_loanUpdateRequest.Id}", _loanUpdateRequest);

        // Check http status
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Fact]
    public async Task Accept_OwnerAcceptsLoan_Ok()
    {
        UserHelper.SetAuthorizationHeader(_client, UserHelper.OwnerToken);

        // Set loan id and new status
        _loanUpdateRequest.Id = _data.LoanInquired.Id;
        _loanUpdateRequest.Status = LoanStatus.Accepted;

        // Perform the action
        var response = await _client.PutAsJsonAsync($"{_apiPath}{_loanUpdateRequest.Id}", _loanUpdateRequest);

        // Check http status
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        // Get response body
        var loanResponse = await response.Content.ReadAsAsync<LoanResponse>();

        // Check that the status is updated
        Assert.That(loanResponse.Status, Is.EqualTo(LoanStatus.Accepted));
    }

    [Fact]
    public async Task Accept_TenantTriesToAcceptHisLoan_UnprocessableEntity()
    {
        UserHelper.SetAuthorizationHeader(_client, UserHelper.TenantToken);

        // Set loan id and new status
        _loanUpdateRequest.Id = _data.LoanInquired.Id;
        _loanUpdateRequest.Status = LoanStatus.Accepted;

        // Perform the action
        var response = await _client.PutAsJsonAsync($"{_apiPath}{_loanUpdateRequest.Id}", _loanUpdateRequest);

        // Check http status
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.UnprocessableEntity));
    }

    [Fact]
    public async Task Deny_OwnerDeniedLoan_Ok()
    {
        UserHelper.SetAuthorizationHeader(_client, UserHelper.OwnerToken);

        // Set loan id and new status
        _loanUpdateRequest.Id = _data.LoanInquired.Id;
        _loanUpdateRequest.Status = LoanStatus.Denied;

        // Perform the action
        var response = await _client.PutAsJsonAsync($"{_apiPath}{_loanUpdateRequest.Id}", _loanUpdateRequest);

        // Check http status
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        // Get response body
        var loanResponse = await response.Content.ReadAsAsync<LoanResponse>();

        // Check that the status is updated
        Assert.That(loanResponse.Status, Is.EqualTo(LoanStatus.Denied));
    }

    [Fact]
    public async Task Deny_TenantTriesToDenyHisLoan_UnprocessableEntity()
    {
        UserHelper.SetAuthorizationHeader(_client, UserHelper.TenantToken);

        // Set loan id and new status
        _loanUpdateRequest.Id = _data.LoanInquired.Id;
        _loanUpdateRequest.Status = LoanStatus.Denied;

        // Perform the action
        var response = await _client.PutAsJsonAsync($"{_apiPath}{_loanUpdateRequest.Id}", _loanUpdateRequest);

        // Check http status
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.UnprocessableEntity));
    }

    [Fact]
    public async Task DisallowedActions_OwnerTriesToChangeStatusToDisallowed_UnprocessableEntity()
    {
        UserHelper.SetAuthorizationHeader(_client, UserHelper.OwnerToken);

        // Set loan id and new status
        _loanUpdateRequest.Id = _data.LoanInquired.Id;

        var disallowedStatuses = new List<LoanStatus>
        {
            LoanStatus.Cancelled,
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

    [Fact]
    public async Task DisallowedActions_TenantTriesToChangeStatusToDisallowed_UnprocessableEntity()
    {
        UserHelper.SetAuthorizationHeader(_client, UserHelper.TenantToken);

        // Set loan id and new status
        _loanUpdateRequest.Id = _data.LoanInquired.Id;

        var disallowedStatuses = new List<LoanStatus>
        {
            LoanStatus.Accepted,
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