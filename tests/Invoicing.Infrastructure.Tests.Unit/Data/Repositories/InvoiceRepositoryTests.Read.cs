using Dapper;
using FluentAssertions;
using Invoicing.Core.Errors;
using Invoicing.Core.Models;
using Invoicing.Infrastructure.Data.Models;
using Invoicing.Infrastructure.Data.Repositories;
using Moq;

namespace Invoicing.Infrastructure.Tests.Unit.Data.Repositories;

public partial class InvoiceRepositoryTests
{
    [Fact]
    public async Task ReadInvoicesAsync_ShouldReturnInvoices()
    {
        // Arrange
        _dbContextMock.Setup(c => c.QueryAsync<InvoiceDto>(It.IsAny<string>(), It.IsAny<object>()))
            .ReturnsAsync([
                GetInvoiceDtoStub("invoice description 1"),
                GetInvoiceDtoStub("invoice description 2")
            ]);
        var invoiceRequest = new InvoiceRequest(null, null, null, null);
        
        // Act
        var invoiceRepository = new InvoiceRepository(_dbContextMock.Object);
        var result = await invoiceRepository.ReadInvoicesAsync(invoiceRequest);
        
        // Assert
        result.IsFailure.Should().BeFalse();
        result.Value.Should().HaveCount(2);
        result.Value.First().Description.Should().Be("invoice description 1");
        result.Value.Last().Description.Should().Be("invoice description 2");
        
        _dbContextMock.Verify(c => c.QueryAsync<InvoiceDto>(
            It.Is<string>(query => query == "SELECT Id, DateIssued, NetAmount, VatAmount, TotalAmount, Description, " + 
                "CompanyId, CounterPartyCompanyId FROM Invoices WHERE 1=1"),
            It.Is<DynamicParameters>(param => !param.ParameterNames.Any())), 
            Times.Once);
    }
    
    
    [Theory]
    [MemberData(nameof(VariousFilterCombinations))]
    public async Task ReadInvoicesAsync_ShouldReturnInvoices_WithVariousFilters(InvoiceRequest invoiceRequest, string expectedQuery, Func<DynamicParameters, bool> parametersMatch)
    {
        // Arrange
        _dbContextMock.Setup(c => c.QueryAsync<InvoiceDto>(It.IsAny<string>(), It.IsAny<object>()))
            .ReturnsAsync([
                GetInvoiceDtoStub("invoice description 1"),
                GetInvoiceDtoStub("invoice description 2")
            ]);
        
        // Act
        var invoiceRepository = new InvoiceRepository(_dbContextMock.Object);
        var result = await invoiceRepository.ReadInvoicesAsync(invoiceRequest);
        
        // Assert
        result.IsFailure.Should().BeFalse();
        result.Value.Should().HaveCount(2);
        result.Value.First().Description.Should().Be("invoice description 1");
        result.Value.Last().Description.Should().Be("invoice description 2");
        
        _dbContextMock.Verify(c => c.QueryAsync<InvoiceDto>(
                It.Is<string>(query => query == expectedQuery),
                It.Is<DynamicParameters>(param => parametersMatch(param))), 
            Times.Once);
    }
    
    [Fact]
    public async Task ReadInvoicesAsync_WhenDbThrows_ShouldReturnFailure()
    {
        // Arrange
        _dbContextMock.Setup(c => c.QueryAsync<InvoiceDto>(It.IsAny<string>(), It.IsAny<object>()))
            .ThrowsAsync(new Exception("boom"));
        var invoiceRequest = new InvoiceRequest(null, null, null, null);
        // Act
        
        var invoiceRepo = new InvoiceRepository(_dbContextMock.Object);
        var result = await invoiceRepo.ReadInvoicesAsync(invoiceRequest);
        
        // Assert
        
        result.IsFailure.Should().BeTrue();
        result.Error.Description.Should().Match("Failed to get invoices with exception boom");
        result.Error.ErrorCode.Should().Be(ErrorCode.GeneralFailure);
    }

    private static InvoiceDto GetInvoiceDtoStub(string description = "Some invoice description") => new()
    {
        Id = Guid.NewGuid(),
        DateIssued = DateTime.UtcNow,
        NetAmount = 4.1f,
        VatAmount = 1.2f,
        TotalAmount = 5.3f,
        Description = description,
        CompanyId = Guid.NewGuid(),
        CounterPartyCompanyId = Guid.NewGuid()
    };
    
    public static IEnumerable<object[]> VariousFilterCombinations()
    {
        var companyId = Guid.NewGuid();
        var invoiceId = Guid.NewGuid();
        var counterPartyCompanyId = Guid.NewGuid();
        var dateIssued = DateTime.UtcNow;
        var list = new List<object[]>();
        list.Add(
        [
            new InvoiceRequest(null, null, null, null),
            "SELECT Id, DateIssued, NetAmount, VatAmount, TotalAmount, Description, " +
            "CompanyId, CounterPartyCompanyId FROM Invoices WHERE 1=1",
            new Func<DynamicParameters, bool>(param => !param.ParameterNames.Any())
        ]);
        
        list.Add(
        [
            new InvoiceRequest(companyId, null, null, null),
            "SELECT Id, DateIssued, NetAmount, VatAmount, TotalAmount, Description, " +
            "CompanyId, CounterPartyCompanyId FROM Invoices WHERE 1=1 AND CompanyId = @CompanyId",
            new Func<DynamicParameters, bool>(param =>
                param.ParameterNames.Contains("CompanyId") 
                && param.Get<Guid>("@CompanyId") == companyId)
        ]);
        
        list.Add(
        [
            new InvoiceRequest(companyId, invoiceId, null, null),
            "SELECT Id, DateIssued, NetAmount, VatAmount, TotalAmount, Description, " +
            "CompanyId, CounterPartyCompanyId FROM Invoices WHERE 1=1 AND CompanyId = @CompanyId "+
            "AND Id = @Id",
            new Func<DynamicParameters, bool>(param =>
                param.ParameterNames.Contains("CompanyId") 
                && param.Get<Guid>("@CompanyId") == companyId
                && param.ParameterNames.Contains("Id") 
                && param.Get<Guid>("@Id") == invoiceId)
        ]);
        
        list.Add(
        [
            new InvoiceRequest(companyId, invoiceId, counterPartyCompanyId, null),
            "SELECT Id, DateIssued, NetAmount, VatAmount, TotalAmount, Description, " +
            "CompanyId, CounterPartyCompanyId FROM Invoices WHERE 1=1 AND CompanyId = @CompanyId "+
            "AND Id = @Id AND CounterPartyCompanyId = @CounterPartyCompanyId",
            new Func<DynamicParameters, bool>(param =>
                param.ParameterNames.Contains("CompanyId") 
                && param.Get<Guid>("@CompanyId") == companyId
                && param.ParameterNames.Contains("Id") 
                && param.Get<Guid>("@Id") == invoiceId
                && param.ParameterNames.Contains("CounterPartyCompanyId") 
                && param.Get<Guid>("@CounterPartyCompanyId") == counterPartyCompanyId)
        ]);
        
        list.Add(
        [
            new InvoiceRequest(companyId, invoiceId, counterPartyCompanyId, dateIssued),
            "SELECT Id, DateIssued, NetAmount, VatAmount, TotalAmount, Description, " +
            "CompanyId, CounterPartyCompanyId FROM Invoices WHERE 1=1 AND CompanyId = @CompanyId "+
            "AND Id = @Id AND CounterPartyCompanyId = @CounterPartyCompanyId " +
            "AND DateIssued = @DateIssued",
            new Func<DynamicParameters, bool>(param =>
                param.ParameterNames.Contains("CompanyId") 
                && param.Get<Guid>("@CompanyId") == companyId
                && param.ParameterNames.Contains("Id") 
                && param.Get<Guid>("@Id") == invoiceId
                && param.ParameterNames.Contains("CounterPartyCompanyId") 
                && param.Get<Guid>("@CounterPartyCompanyId") == counterPartyCompanyId
                && param.ParameterNames.Contains("DateIssued") 
                && param.Get<DateTime>("@DateIssued") == dateIssued)
        ]);
        
        list.Add(
        [
            new InvoiceRequest(null, invoiceId, null, dateIssued),
            "SELECT Id, DateIssued, NetAmount, VatAmount, TotalAmount, Description, " +
            "CompanyId, CounterPartyCompanyId FROM Invoices WHERE 1=1 AND Id = @Id " +
            "AND DateIssued = @DateIssued",
            new Func<DynamicParameters, bool>(param =>
                param.ParameterNames.Contains("Id") 
                && param.ParameterNames.Contains("DateIssued") 
                && param.Get<DateTime>("@DateIssued") == dateIssued)
        ]);

        return list;

    }
}