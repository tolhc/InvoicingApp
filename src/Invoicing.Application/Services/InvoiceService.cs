using Invoicing.Application.Interfaces;
using Invoicing.Core.Models;

namespace Invoicing.Application.Services;

public class InvoiceService : IInvoiceService
{
    private readonly IInvoiceRepository _repository;

    public InvoiceService(IInvoiceRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<Invoice>> GetSentInvoicesAsync(InvoiceRequest invoiceRequest)
    {
        var invoices = await _repository.ReadInvoicesAsync(invoiceRequest);
        return invoices.ToList().AsReadOnly();
    }

    public async Task<IReadOnlyCollection<Invoice>> GetReceivedInvoicesAsync(InvoiceRequest invoiceRequest)
    {
        var invoices = await _repository.ReadInvoicesAsync(invoiceRequest with
        {
            CompanyId = invoiceRequest.CounterPartyCompanyId, 
            CounterPartyCompanyId = invoiceRequest.CompanyId
        });
        return invoices.ToList().AsReadOnly();
    }

    public async Task<Invoice> CreateInvoiceAsync(Invoice invoice)
    {
        return await _repository.CreateInvoiceAsync(invoice);
    }
}