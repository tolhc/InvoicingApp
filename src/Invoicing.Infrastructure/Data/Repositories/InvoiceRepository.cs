using System.Text;
using Dapper;
using Invoicing.Application.Interfaces;
using Invoicing.Core.Models;
using Invoicing.Infrastructure.Data.Mappings;
using Invoicing.Infrastructure.Data.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Invoicing.Infrastructure.Data.Repositories;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly string _invoicingDbConnectionString;

    public InvoiceRepository(IConfiguration configuration)
    {
        _invoicingDbConnectionString = configuration.GetConnectionString("InvoicingDb") ?? throw new NullReferenceException("The InvoicingDb connection string is missing.");
    }

    public async Task<Invoice> CreateInvoiceAsync(Invoice invoice)
    {
        const string query = """
                             INSERT INTO Invoices (InvoiceId, DateIssued, NetAmount, VatAmount, TotalAmount, Description, CompanyId, CounterPartyCompanyId)
                             VALUES (@InvoiceId, @DateIssued, @NetAmount, @VatAmount, @TotalAmount, @Description, @CompanyId, @CounterPartyCompanyId)
                             """;
        
        await using var connection = new SqlConnection(_invoicingDbConnectionString);
        var result = await connection.ExecuteAsync(query, invoice.ToInvoiceDto());

        if (result > 0)
        {
            return invoice;
        }
        
        throw new Exception("Failed to create invoice."); //TODO: replace with result pattern
    }

    public async Task<IEnumerable<Invoice>> ReadInvoicesAsync(InvoiceRequest invoiceRequest)
    {
        //shortcuting here with 1=1 in order to not check everytime if where has been added, I guess I could do it with a dummy method though
        var queryBuilder = new StringBuilder(
            "SELECT InvoiceId, DateIssued, NetAmount, VatAmount, TotalAmount, Description, CompanyId, CounterPartyCompanyId FROM Invoices WHERE 1=1");
        
        var parameters = new DynamicParameters();
        
        if (invoiceRequest.CompanyId != null)
        {
            queryBuilder.Append(" AND CompanyId = @CompanyId");
            parameters.Add("@CompanyId", invoiceRequest.CompanyId);
        }

        if (invoiceRequest.InvoiceId != null)
        {
            queryBuilder.Append(" AND InvoiceId = @InvoiceId");
            parameters.Add("@InvoiceId", invoiceRequest.InvoiceId);
        }
        
        if (invoiceRequest.CounterPartyCompanyId != null)
        {
            queryBuilder.Append(" AND CounterPartyCompanyId = @CounterPartyCompanyId");
            parameters.Add("@CounterPartyCompanyId", invoiceRequest.CounterPartyCompanyId);
        }

        if (invoiceRequest.DateIssued != null)
        {
            queryBuilder.Append(" AND DateIssued = @DateIssued"); //TODO: not sure about the date tbh, if it should be DateOnly, if it should support greater/less than
            parameters.Add("@DateIssued", invoiceRequest.DateIssued);
        }
        
        //var rawQuery = queryBuilder.ToString();
        
        await using var connection = new SqlConnection(_invoicingDbConnectionString);
        var result = await connection.QueryAsync<InvoiceDto>(queryBuilder.ToString(), parameters);

        return result.Select(i => i.ToInvoice());
    }
}