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
            InvoiceId: Guid.NewGuid(),
            DateIssued: DateTime.UtcNow,
            NetAmount: 4.1f,
            VatAmount: 1.2f,
            TotalAmount: 5.3f,
            Description: "Some invoice description",
            IssuerCompanyId:  Guid.NewGuid(),
            ReceiverCompanyId: Guid.NewGuid()
        );
        
        _mockInvoiceRepository.Setup(m => m.CreateInvoiceAsync(It.IsAny<Invoice>())).ReturnsAsync(invoice);
        
        // Act
        var invoiceService = new InvoiceService(_mockInvoiceRepository.Object);
        var result = await invoiceService.CreateInvoiceAsync(invoice);
        
        // Assert
        result.Should().Be(invoice);
        
        _mockInvoiceRepository.Verify(c => c.CreateInvoiceAsync(It.Is<Invoice>(passedInvoice  => passedInvoice == invoice)), Times.Once);
    }

}