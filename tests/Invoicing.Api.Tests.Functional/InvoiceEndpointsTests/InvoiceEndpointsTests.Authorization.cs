using System.Net;
using System.Net.Http.Headers;
using FluentAssertions;

namespace Invoicing.Api.Tests.Functional.InvoiceEndpointsTests;

public partial class InvoiceEndpointsTests
{
    [Fact]
    public async Task AuthenticatedEndpointCall_WhenNoToken_ShouldReturnUnauthorized()
    {
        // Arrange

        _client.DefaultRequestHeaders.Authorization = null;

        // Act
        var response = await _client.GetAsync("/invoice/sent");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AuthorizedEndpointCall_WhenUserRoleNotAuthorized_ShouldReturnForbidden()
    {
        // Arrange

        var token = await _client.GetDemoToken("BD2717F2-BAE0-43CA-AC43-43F48E6A1397", "RandomRole");
        _client.DefaultRequestHeaders.Authorization = null;
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.PostAsync("/invoice", _defaultPostedInvoiceVmContent);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }


}
