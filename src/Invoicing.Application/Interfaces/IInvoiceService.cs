using Invoicing.Core.Errors;
using Invoicing.Core.Models;
using Invoicing.Core.Results;

namespace Invoicing.Application.Interfaces;

public interface IInvoiceService
{
    Task<Result<IReadOnlyCollection<Invoice>, ApplicationError>> GetSentInvoicesAsync(InvoiceRequest invoiceRequest);
    Task<Result<IReadOnlyCollection<Invoice>, ApplicationError>> GetReceivedInvoicesAsync(InvoiceRequest invoiceRequest);
    Task<Result<Invoice, ApplicationError>> CreateInvoiceAsync(Invoice invoice);
}