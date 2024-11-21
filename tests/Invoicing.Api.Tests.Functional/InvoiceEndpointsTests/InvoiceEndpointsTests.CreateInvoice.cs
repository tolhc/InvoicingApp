using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Invoicing.Api.Mappings;
using Invoicing.Api.ViewModels;
using Invoicing.Application.Tests.Unit;
using Invoicing.Core.Errors;
using Invoicing.Core.Models;
using Invoicing.Core.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Invoicing.Api.Tests.Functional.InvoiceEndpointsTests;

public partial class InvoiceEndpointsTests
{
    
    [Fact]
    public async Task CreateInvoice_WhenCompanyIdDoesntMatchPostedCompanyId_ShouldReturnBadRequest()
    {
        // Arrange
        
        var token = await _client.GetDemoToken("142B42A2-7832-46A7-9DDC-C0CC805816CB");
        _client.DefaultRequestHeaders.Authorization = null;
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.PostAsync("/invoice", _defaultPostedInvoiceVmContent);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        result!.Detail.Should().Be("Issuer company id is different that the authorized one");
    }
    
    [Fact]
    public async Task CreateInvoice_ShouldReturnCreatedInvoice()
    {
        // Arrange
        var invoiceVm = JsonSerializer.Deserialize<InvoiceVm>(_defaultPostedInvoiceVmString)!;
        
        _dbContextMock.Setup(db => db.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>()))
            .ReturnsAsync(1);

        var content = new StringContent(_defaultPostedInvoiceVmString, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/invoice", content);

        // Assert
        response.EnsureSuccessStatusCode();
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var resultVm = await response.Content.ReadFromJsonAsync<InvoiceVm>();
        resultVm.Should().Be(invoiceVm with { Id = resultVm!.Id });
        response.Headers.Location.Should().Be(new Uri($"invoices/sent?invoice_id={resultVm.Id.ToString()}", UriKind.Relative));
    }
    
    [Fact]
    public async Task CreateInvoice_WhenDbGivesErrorCode_ShouldReturnInternalServerError()
    {
        // Arrange
        
        _dbContextMock.Setup(db => db.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>()))
            .ReturnsAsync(0);

        var content = new StringContent(_defaultPostedInvoiceVmString, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/invoice", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        result!.Detail.Should().Be($"Failed to create invoice with db result code 0");
    }
    
    [Fact]
    public async Task CreateInvoice_WhenDbThrowsGenericException_ShouldReturnInternalServerError()
    {
        // Arrange
        _dbContextMock.Setup(db => db.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>()))
            .Throws(new Exception("boom"));

        var content = new StringContent(_defaultPostedInvoiceVmString, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/invoice", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        result!.Detail.Should().Be("Failed to create invoice with exception boom");
    }

    [Theory]
    [InlineData(2627, HttpStatusCode.Conflict,
        "Failed to create invoice because an invoice with the same key already exists")]
    [InlineData(547, HttpStatusCode.BadRequest,
        "Failed to create invoice because IssuerCompanyId or ReceiverCompanyId was incorrect")]
    [InlineData(200, HttpStatusCode.InternalServerError, "Failed to create invoice with error code 200")]
    [InlineData(0, HttpStatusCode.InternalServerError, "Failed to create invoice with error code 0")]
    public async Task CreateInvoice_WhenDbThrowsSqlException_ShouldReturnInternalServerError(int exceptionNumber,
        HttpStatusCode expectedStatusCodeCode, string expectedErrorMessage)
    {
        // Arrange
        _dbContextMock.Setup(db => db.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>()))
            .Throws(new SqlExceptionBuilder().WithErrorNumber(exceptionNumber).Build());

        var content = new StringContent(_defaultPostedInvoiceVmString, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/invoice", content);

        // Assert
        response.StatusCode.Should().Be(expectedStatusCodeCode);
        var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        result!.Detail.Should().Be(expectedErrorMessage);
    }
}