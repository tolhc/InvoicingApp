using Invoicing.Application.Interfaces;
using Invoicing.Core.Models;

namespace Invoicing.Application.Services;

public class InvoiceService : IInvoiceService
{
    public Task<IReadOnlyCollection<Invoice>> GetSentInvoicesAsync(InvoiceRequest invoiceRequest)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyCollection<Invoice>> GetReceivedInvoicesAsync(InvoiceRequest invoiceRequest)
    {
        throw new NotImplementedException();
    }

    public Task<Invoice> SaveInvoiceAsync(Invoice invoice)
    {
        throw new NotImplementedException();
    }
}