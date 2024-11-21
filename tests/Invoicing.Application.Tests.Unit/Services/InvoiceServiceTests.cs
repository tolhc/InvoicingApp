using System.Net;
using FluentAssertions;
using Invoicing.Application.Interfaces;
using Invoicing.Application.Services;
using Invoicing.Core.Errors;
using Invoicing.Core.Models;
using Moq;

namespace Invoicing.Application.Tests.Unit.Services;

public class InvoiceServiceTests
{
    private readonly Mock<IInvoiceRepository> _mockInvoiceRepository = new();

    [Fact]
    public async Task CreateInvoiceAsync_ShouldReturnInvoice()
    {
        // Arrange

        var invoice = new Invoice
        (
            Guid.NewGuid(),
            DateTime.UtcNow,
            4.1f,
            1.2f,
            5.3f,
            "Some invoice description",
            Guid.NewGuid(),
            Guid.NewGuid()
        );

        _mockInvoiceRepository.Setup(m => m.CreateInvoiceAsync(It.IsAny<Invoice>())).ReturnsAsync(invoice);

        // Act
        var invoiceService = new InvoiceService(_mockInvoiceRepository.Object);
        var result = await invoiceService.CreateInvoiceAsync(invoice);

        // Assert
        result.IsFailure.Should().BeFalse();
        result.Value.Should().Be(invoice);

        _mockInvoiceRepository.Verify(
            c => c.CreateInvoiceAsync(It.Is<Invoice>(passedInvoice =>
                passedInvoice == invoice)),
            Times.Once);
    }

    [Theory]
    [InlineData(ErrorCode.GeneralFailure, HttpStatusCode.InternalServerError)]
    [InlineData(ErrorCode.OperationFailure, HttpStatusCode.InternalServerError)]
    [InlineData(ErrorCode.Conflict, HttpStatusCode.Conflict)]
    [InlineData(ErrorCode.BadRequest, HttpStatusCode.BadRequest)]
    [InlineData((ErrorCode)1000, HttpStatusCode.InternalServerError)]
    public async Task CreateInvoiceAsync_WhenDbError_ShouldHandleErrorAndReturnIt(ErrorCode errorCode, HttpStatusCode expectedStatusCode)
    {
        // Arrange

        var invoice = new Invoice
        (
            Guid.NewGuid(),
            DateTime.UtcNow,
            4.1f,
            1.2f,
            5.3f,
            "Some invoice description",
            Guid.NewGuid(),
            Guid.NewGuid()
        );

        _mockInvoiceRepository.Setup(m => m.CreateInvoiceAsync(It.IsAny<Invoice>()))
            .ReturnsAsync(new DbError("DbError", errorCode));

        var expectedApplicationError = new ApplicationError("DbError", expectedStatusCode);

        // Act
        var invoiceService = new InvoiceService(_mockInvoiceRepository.Object);
        var result = await invoiceService.CreateInvoiceAsync(invoice);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(expectedApplicationError);

        _mockInvoiceRepository.Verify(
            c => c.CreateInvoiceAsync(It.Is<Invoice>(passedInvoice =>
                passedInvoice == invoice)),
            Times.Once);
    }

    [Fact]
    public async Task GetSentInvoicesAsync_ShouldReturnInvoices()
    {
        // Arrange

        var companyId = Guid.NewGuid();
        var counterPartyCompanyId = Guid.NewGuid();

        var invoiceRequest = new InvoiceRequest(companyId, null, counterPartyCompanyId, null);

        _mockInvoiceRepository.Setup(m => m.ReadInvoicesAsync(It.IsAny<InvoiceRequest>()))
            .ReturnsAsync(new List<Invoice>
            {
                new(
                    Guid.NewGuid(),
                    DateTime.UtcNow,
                    4.1f,
                    1.2f,
                    5.3f,
                    "Some invoice description",
                    companyId,
                    counterPartyCompanyId
                ),
                new(
                    Guid.NewGuid(),
                    DateTime.UtcNow,
                    4.1f,
                    1.2f,
                    5.3f,
                    "Some invoice description 2",
                    companyId,
                    counterPartyCompanyId
                )
            });

        // Act
        var invoiceService = new InvoiceService(_mockInvoiceRepository.Object);
        var result = await invoiceService.GetSentInvoicesAsync(invoiceRequest);

        // Assert
        result.IsFailure.Should().BeFalse();
        result.Value.Should().HaveCount(2);
        result.Value.First().Description.Should().Be("Some invoice description");

        _mockInvoiceRepository.Verify(
            c => c.ReadInvoicesAsync(It.Is<InvoiceRequest>(passedInvoiceRequest =>
                passedInvoiceRequest == invoiceRequest)),
            Times.Once);
    }

    [Fact]
    public async Task GetReceivedInvoicesAsync_ShouldReturnInvoices()
    {
        // Arrange

        var companyId = Guid.NewGuid();
        var counterPartyCompanyId = Guid.NewGuid();

        var invoiceRequest = new InvoiceRequest(companyId, null, counterPartyCompanyId, null);

        _mockInvoiceRepository.Setup(m => m.ReadInvoicesAsync(It.IsAny<InvoiceRequest>()))
            .ReturnsAsync(new List<Invoice>
            {
                new(
                    Guid.NewGuid(),
                    DateTime.UtcNow,
                    4.1f,
                    1.2f,
                    5.3f,
                    "Some invoice description",
                    counterPartyCompanyId,
                    companyId
                ),
                new(
                    Guid.NewGuid(),
                    DateTime.UtcNow,
                    4.1f,
                    1.2f,
                    5.3f,
                    "Some invoice description 2",
                    counterPartyCompanyId,
                    companyId
                )
            });

        // Act
        var invoiceService = new InvoiceService(_mockInvoiceRepository.Object);
        var result = await invoiceService.GetReceivedInvoicesAsync(invoiceRequest);

        // Assert
        result.IsFailure.Should().BeFalse();
        result.Value.Should().HaveCount(2);
        result.Value.First().Description.Should().Be("Some invoice description");

        _mockInvoiceRepository.Verify(
            c => c.ReadInvoicesAsync(It.Is<InvoiceRequest>(passedInvoiceRequest =>
                    passedInvoiceRequest.CompanyId == invoiceRequest.CounterPartyCompanyId
                    && passedInvoiceRequest.CounterPartyCompanyId == invoiceRequest.CompanyId)),
            Times.Once);
    }
}
