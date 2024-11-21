using Invoicing.Core.Errors;
using Invoicing.Core.Models;
using Invoicing.Core.Results;

namespace Invoicing.Application.Interfaces;

public interface IInvoiceRepository
{
    Task<Result<Invoice, DbError>> CreateInvoiceAsync(Invoice invoice);
    Task<Result<IEnumerable<Invoice>, DbError>> ReadInvoicesAsync(InvoiceRequest invoiceRequest);
}