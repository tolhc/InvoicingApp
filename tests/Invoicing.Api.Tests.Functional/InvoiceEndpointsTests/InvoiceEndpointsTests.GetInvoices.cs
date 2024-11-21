using System.Net;
using FluentAssertions;
using Invoicing.Api.ViewModels;
using Invoicing.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Invoicing.Api.Tests.Functional.InvoiceEndpointsTests;

public partial class InvoiceEndpointsTests
{
    [Fact]
    public async Task GetSentInvoices_ShouldReturnInvoices()
    {
        // Arrange

        var invoice1 = GetInvoiceDtoStub("131edd7c-c0e3-42c2-bdcc-e149b7170e3d", "invoice 1");
        var invoice2 = GetInvoiceDtoStub("8a7f75f3-1937-4a6d-b956-48791d3b1418", "invoice 2");
        var invoice3 = GetInvoiceDtoStub("2ac563fb-9d81-469d-9868-af8baa6f7edd", "invoice 3");

        _dbContextMock.Setup(db => db.QueryAsync<InvoiceDto>(It.IsAny<string>(), It.IsAny<object>()))
            .ReturnsAsync([invoice1, invoice2, invoice3]);

        // Act
        var response = await _client.GetAsync("/invoice/sent");

        // Assert
        response.EnsureSuccessStatusCode();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<InvoiceVm>>()!;
        CompareInvoices(invoice1, result![0]);
        CompareInvoices(invoice2, result![1]);
        CompareInvoices(invoice3, result![2]);
    }

    [Fact]
    public async Task GetSentInvoices_WhenNoInvoices_ShouldReturnNotFound()
    {
        // Arrange

        _dbContextMock.Setup(db => db.QueryAsync<InvoiceDto>(It.IsAny<string>(), It.IsAny<object>()))
            .ReturnsAsync([]);

        // Act
        var response = await _client.GetAsync("/invoice/sent");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        result!.Detail.Should().Be("No sent invoices found");
    }

    [Fact]
    public async Task GetSentInvoices_WhenDbThrows_ShouldReturnInternalServerError()
    {
        // Arrange

        _dbContextMock.Setup(db => db.QueryAsync<InvoiceDto>(It.IsAny<string>(), It.IsAny<object>()))
            .ThrowsAsync(new Exception("boom"));

        // Act
        var response = await _client.GetAsync("/invoice/sent");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        result!.Detail.Should().Be("Failed to get invoices with exception boom");
    }

    [Fact]
    public async Task GetReceivedInvoices_ShouldReturnInvoices()
    {
        // Arrange

        var invoice1 = GetInvoiceDtoStub("131edd7c-c0e3-42c2-bdcc-e149b7170e3d", "invoice 1");
        var invoice2 = GetInvoiceDtoStub("8a7f75f3-1937-4a6d-b956-48791d3b1418", "invoice 2");
        var invoice3 = GetInvoiceDtoStub("2ac563fb-9d81-469d-9868-af8baa6f7edd", "invoice 3");

        _dbContextMock.Setup(db => db.QueryAsync<InvoiceDto>(It.IsAny<string>(), It.IsAny<object>()))
            .ReturnsAsync([invoice1, invoice2, invoice3]);

        // Act
        var response = await _client.GetAsync("/invoice/received");

        // Assert
        response.EnsureSuccessStatusCode();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<InvoiceVm>>()!;
        CompareInvoices(invoice1, result![0]);
        CompareInvoices(invoice2, result![1]);
        CompareInvoices(invoice3, result![2]);
    }

    [Fact]
    public async Task GetReceivedInvoices_WhenNoInvoices_ShouldReturnNotFound()
    {
        // Arrange

        _dbContextMock.Setup(db => db.QueryAsync<InvoiceDto>(It.IsAny<string>(), It.IsAny<object>()))
            .ReturnsAsync([]);

        // Act
        var response = await _client.GetAsync("/invoice/received");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        result!.Detail.Should().Be("No received invoices found");
    }

    [Fact]
    public async Task GetReceivedInvoices_WhenDbThrows_ShouldReturnInternalServerError()
    {
        // Arrange

        _dbContextMock.Setup(db => db.QueryAsync<InvoiceDto>(It.IsAny<string>(), It.IsAny<object>()))
            .ThrowsAsync(new Exception("boom"));

        // Act
        var response = await _client.GetAsync("/invoice/received");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        result!.Detail.Should().Be("Failed to get invoices with exception boom");
    }

    private static InvoiceDto GetInvoiceDtoStub(string id, string description = "Some invoice description") => new()
    {
        Id = Guid.Parse(id),
        DateIssued = new DateTime(2024, 11, 20),
        NetAmount = 4.1f,
        VatAmount = 1.2f,
        TotalAmount = 5.3f,
        Description = description,
        CompanyId = Guid.Parse("713404a2-35d3-4a31-ab11-9b248abe71e6"),
        CounterPartyCompanyId = Guid.Parse("4ed1a415-4bd4-4b98-84db-0bddc85f052b")
    };

    private static void CompareInvoices(InvoiceDto expected, InvoiceVm actual)
    {
        actual.Id.Should().Be(expected.Id);
        actual.DateIssued.Should().Be(expected.DateIssued);
        actual.NetAmount.Should().Be(expected.NetAmount);
        actual.VatAmount.Should().Be(expected.VatAmount);
        actual.TotalAmount.Should().Be(expected.TotalAmount);
        actual.Description.Should().Be(expected.Description);
        actual.IssuerCompanyId.Should().Be(expected.CompanyId);
        actual.ReceiverCompanyId.Should().Be(expected.CounterPartyCompanyId);
    }
}
