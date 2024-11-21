using System.Net;
using Invoicing.Application.Interfaces;
using Invoicing.Core.Errors;
using Invoicing.Core.Models;
using Invoicing.Core.Results;

namespace Invoicing.Application.Services;

public class InvoiceService : IInvoiceService
{
    private readonly IInvoiceRepository _repository;

    public InvoiceService(IInvoiceRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IReadOnlyCollection<Invoice>, ApplicationError>> GetSentInvoicesAsync(
        InvoiceRequest invoiceRequest)
    {
        var result = await _repository.ReadInvoicesAsync(invoiceRequest);
        if (result.IsFailure)
        {
            return HandleDbError(result.Error);
        }

        return result.Value.ToList().AsReadOnly();
    }

    public async Task<Result<IReadOnlyCollection<Invoice>, ApplicationError>> GetReceivedInvoicesAsync(
        InvoiceRequest invoiceRequest)
    {
        var result = await _repository.ReadInvoicesAsync(invoiceRequest with
        {
            CompanyId = invoiceRequest.CounterPartyCompanyId,
            CounterPartyCompanyId = invoiceRequest.CompanyId
        });

        if (result.IsFailure)
        {
            return HandleDbError(result.Error);
        }

        return result.Value.ToList().AsReadOnly();
    }

    public async Task<Result<Invoice, ApplicationError>> CreateInvoiceAsync(Invoice invoice)
    {
        var result = await _repository.CreateInvoiceAsync(invoice);

        if (result.IsFailure)
        {
            return HandleDbError(result.Error);
        }

        return result.Value;
    }

    private static ApplicationError HandleDbError(DbError dbError)
    {
        return dbError.ErrorCode switch
        {
            ErrorCode.GeneralFailure => new ApplicationError(dbError.Description, HttpStatusCode.InternalServerError),
            ErrorCode.OperationFailure => new ApplicationError(dbError.Description, HttpStatusCode.InternalServerError),
            ErrorCode.Conflict => new ApplicationError(dbError.Description, HttpStatusCode.Conflict),
            ErrorCode.BadRequest => new ApplicationError(dbError.Description, HttpStatusCode.BadRequest),

            _ => new ApplicationError(dbError.Description, HttpStatusCode.InternalServerError)
        };
    }
}
