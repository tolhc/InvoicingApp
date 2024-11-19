using Invoicing.Core.Models;

namespace Invoicing.Application.Interfaces;

public interface IInvoiceService
{
    Task<IReadOnlyCollection<Invoice>> GetSentInvoicesAsync(InvoiceRequest invoiceRequest);
    Task<IReadOnlyCollection<Invoice>> GetReceivedInvoicesAsync(InvoiceRequest invoiceRequest);
    Task<Invoice> SaveInvoiceAsync(Invoice invoice);
}