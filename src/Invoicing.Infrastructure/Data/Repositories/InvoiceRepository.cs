using System.Text;
using Dapper;
using Invoicing.Application.Interfaces;
using Invoicing.Core.Errors;
using Invoicing.Core.Models;
using Invoicing.Core.Results;
using Invoicing.Infrastructure.Data.Mappings;
using Invoicing.Infrastructure.Data.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Invoicing.Infrastructure.Data.Repositories;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly IDatabaseContext _dbContext;

    public InvoiceRepository(IDatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<Invoice, DbError>> CreateInvoiceAsync(Invoice invoice)
    {
        try
        {
            const string query = """
                                 INSERT INTO Invoices (Id, DateIssued, NetAmount, VatAmount, TotalAmount, Description, CompanyId, CounterPartyCompanyId)
                                 VALUES (@Id, @DateIssued, @NetAmount, @VatAmount, @TotalAmount, @Description, @CompanyId, @CounterPartyCompanyId)
                                 """;

            var result = await _dbContext.ExecuteAsync(query, invoice.ToInvoiceDto());

            if (result <= 0)
            {
                return new DbError($"Failed to create invoice with db result code {result}",
                    ErrorCode.OperationFailure);
            }

            return invoice;
        }
        catch (SqlException sqlEx)
        {
            return sqlEx.Number switch
            {
                2627 => new DbError("Failed to create invoice because an invoice with the same key already exists",
                    ErrorCode.Conflict), // primary key issue
                547 => new DbError(
                    $"Failed to create invoice because {nameof(invoice.IssuerCompanyId)} or {nameof(invoice.ReceiverCompanyId)} was incorrect",
                    ErrorCode.BadRequest), // foreign key issue

                _ => new DbError($"Failed to create invoice with error code {sqlEx.Number}",
                    ErrorCode.OperationFailure)
            };
        }
        catch (Exception ex)
        {
            return new DbError($"Failed to create invoice with exception {ex.Message}", ErrorCode.GeneralFailure);
        }
    }

    public async Task<Result<IEnumerable<Invoice>, DbError>> ReadInvoicesAsync(InvoiceRequest invoiceRequest)
    {
        try
        {
            //shortcuting here with 1=1 in order to not check everytime if where has been added, I guess I could do it with a dummy method though
            var queryBuilder = new StringBuilder(
                "SELECT Id, DateIssued, NetAmount, VatAmount, TotalAmount, Description, CompanyId, CounterPartyCompanyId FROM Invoices WHERE 1=1");

            var parameters = new DynamicParameters();

            if (invoiceRequest.CompanyId != null)
            {
                queryBuilder.Append(" AND CompanyId = @CompanyId");
                parameters.Add("@CompanyId", invoiceRequest.CompanyId);
            }

            if (invoiceRequest.InvoiceId != null)
            {
                queryBuilder.Append(" AND Id = @Id");
                parameters.Add("@Id", invoiceRequest.InvoiceId);
            }

            if (invoiceRequest.CounterPartyCompanyId != null)
            {
                queryBuilder.Append(" AND CounterPartyCompanyId = @CounterPartyCompanyId");
                parameters.Add("@CounterPartyCompanyId", invoiceRequest.CounterPartyCompanyId);
            }

            if (invoiceRequest.DateIssued != null)
            {
                queryBuilder.Append(
                    " AND DateIssued = @DateIssued"); // not sure about the date tbh, if it should be DateOnly in case we don't care about time and if it should support greater/less than
                parameters.Add("@DateIssued", invoiceRequest.DateIssued);
            }

            //var rawQuery = queryBuilder.ToString();

            var result = await _dbContext.QueryAsync<InvoiceDto>(queryBuilder.ToString(), parameters);

            var invoices = result.Select(i => i.ToInvoice());

            return Result<IEnumerable<Invoice>, DbError>.Success(invoices);
        }
        catch (Exception ex)
        {
            return new DbError($"Failed to get invoices with exception {ex.Message}", ErrorCode.GeneralFailure);
        }
    }
}
