using Invoicing.Core.Models;

namespace Invoicing.Application.Interfaces;

public interface IInvoiceRepository
{
    Task<Invoice> CreateInvoiceAsync(Invoice invoice);
    Task<IEnumerable<Invoice>> ReadInvoicesAsync(InvoiceRequest invoiceRequest);
}