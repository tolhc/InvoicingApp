using FluentAssertions;
using Invoicing.Application.Tests.Unit;
using Invoicing.Core.Errors;
using Invoicing.Core.Models;
using Invoicing.Infrastructure.Data;
using Invoicing.Infrastructure.Data.Models;
using Invoicing.Infrastructure.Data.Repositories;
using Moq;

namespace Invoicing.Infrastructure.Tests.Unit.Data.Repositories;

public partial class InvoiceRepositoryTests
{
    private readonly Mock<IDatabaseContext> _dbContextMock = new();

    [Fact]
    public async Task CreateInvoiceAsync_ShouldReturnCreatedInvoice()
    {
        // Arrange

        _dbContextMock.Setup(c => c.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>()))
            .ReturnsAsync(1);

        var invoice = GetInvoiceStub();

        // Act
        
        var invoiceRepo = new InvoiceRepository(_dbContextMock.Object);
        var result = await invoiceRepo.CreateInvoiceAsync(invoice);
        
        // Assert
        
        result.IsFailure.Should().BeFalse();
        result.Value.Should().Be(invoice);

        _dbContextMock.Verify(c => c.ExecuteAsync(
            It.Is<string>(s => s ==
            """
            INSERT INTO Invoices (Id, DateIssued, NetAmount, VatAmount, TotalAmount, Description, CompanyId, CounterPartyCompanyId)
            VALUES (@InvoiceId, @DateIssued, @NetAmount, @VatAmount, @TotalAmount, @Description, @CompanyId, @CounterPartyCompanyId)
            """), 
            It.Is<InvoiceDto>(i => 
                i.Id == invoice.Id
                && i.DateIssued == invoice.DateIssued
                && i.NetAmount == invoice.NetAmount 
                && i.VatAmount == invoice.VatAmount
                && i.TotalAmount == invoice.TotalAmount
                && i.Description == invoice.Description
                && i.CompanyId == invoice.IssuerCompanyId
                && i.CounterPartyCompanyId == invoice.ReceiverCompanyId)),
            
            Times.Once);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task CreateInvoiceAsync_WhenErrorResult_ShouldReturnFailure(int dbCode)
    {
        // Arrange
        _dbContextMock.Setup(c => c.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>()))
            .ReturnsAsync(dbCode);
        
        // Act
        
        var invoiceRepo = new InvoiceRepository(_dbContextMock.Object);
        var result = await invoiceRepo.CreateInvoiceAsync(GetInvoiceStub());
        
        // Assert
        
        result.IsFailure.Should().BeTrue();
        result.Error.Description.Should().Match("Failed to create invoice with db result code *");
        result.Error.ErrorCode.Should().Be(ErrorCode.OperationFailure);
    }
    
    [Theory]
    [InlineData(2627, ErrorCode.Conflict, "Failed to create invoice because an invoice with the same key already exists")]
    [InlineData(547, ErrorCode.BadRequest, "Failed to create invoice because IssuerCompanyId or ReceiverCompanyId was incorrect")]
    [InlineData(200, ErrorCode.OperationFailure, "Failed to create invoice with error code 200")]
    [InlineData(0, ErrorCode.OperationFailure, "Failed to create invoice with error code 0")]
    public async Task CreateInvoiceAsync_WhenDbThrowsSqlException_ShouldReturnFailure(int exceptionNumber, ErrorCode expectedErrorCode, string expectedErrorMessage)
    {
        // Arrange
        _dbContextMock.Setup(c => c.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>()))
            .ThrowsAsync(new SqlExceptionBuilder().WithErrorNumber(exceptionNumber)
                    .Build());
        
        // Act
        
        var invoiceRepo = new InvoiceRepository(_dbContextMock.Object);
        var result = await invoiceRepo.CreateInvoiceAsync(GetInvoiceStub());
        
        // Assert
        
        result.IsFailure.Should().BeTrue();
        result.Error.Description.Should().Match(expectedErrorMessage);
        result.Error.ErrorCode.Should().Be(expectedErrorCode);
    }
    
    [Fact]
    public async Task CreateInvoiceAsync_WhenDbThrows_ShouldReturnFailure()
    {
        // Arrange
        _dbContextMock.Setup(c => c.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>()))
            .ThrowsAsync(new Exception("boom"));
        
        // Act
        
        var invoiceRepo = new InvoiceRepository(_dbContextMock.Object);
        var result = await invoiceRepo.CreateInvoiceAsync(GetInvoiceStub());
        
        // Assert
        
        result.IsFailure.Should().BeTrue();
        result.Error.Description.Should().Match("Failed to create invoice with exception boom");
        result.Error.ErrorCode.Should().Be(ErrorCode.GeneralFailure);
    }
    
    private static Invoice GetInvoiceStub(string description = "Some invoice description") => new Invoice
    (
        Guid.NewGuid(),
        DateTime.UtcNow,
        4.1f,
        1.2f,
        5.3f,
        description,
        Guid.NewGuid(),
        Guid.NewGuid()
    );
}