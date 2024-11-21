using System.Net;
using System.Net.Http.Headers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace Invoicing.Api.Tests.Functional.InvoiceEndpointsTests;

public partial class InvoiceEndpointsTests
{
    [Fact]
    public async Task Validation_WhenInvalidCompanyId_ShouldReturnForbidden()
    {
        // Arrange
        
        var token = await MockTokenGeneration.GetDemoToken(_client, "BD2717F2-BAE0-43CA-AC43");
        _client.DefaultRequestHeaders.Authorization = null;
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        

        // Act
        var response = await _client.PostAsync("/invoice", _defaultPostedInvoiceVmContent);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        result!.Detail.Should().Be("Invalid companyId specified");
    }
    
    [Fact]
    public async Task Validation_WhenNoCompanyId_ShouldReturnForbidden()
    {
        // Arrange
        
        var token = await _client.GetDemoToken(null);
        _client.DefaultRequestHeaders.Authorization = null;
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        

        // Act
        var response = await _client.PostAsync("/invoice", _defaultPostedInvoiceVmContent);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        result!.Detail.Should().Be("No companyId specified");
    }
}