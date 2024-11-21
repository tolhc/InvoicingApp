using FluentAssertions;
using Invoicing.Application.Interfaces;
using Invoicing.Application.Services;
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
            c => c.CreateInvoiceAsync(It.Is<Invoice>(passedInvoice => passedInvoice == invoice)), Times.Once);
    }
}