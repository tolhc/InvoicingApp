using Invoicing.Core.Models;
using Invoicing.Infrastructure.Data.Models;

namespace Invoicing.Infrastructure.Data.Mappings;

public static class InvoiceVmMappings
{
    public static InvoiceDto ToInvoiceDto(this Invoice invoice)
    {
        return new InvoiceDto
        {
            Id = invoice.Id,
            DateIssued = invoice.DateIssued,
            NetAmount = invoice.NetAmount,
            VatAmount = invoice.VatAmount,
            TotalAmount = invoice.TotalAmount,
            Description = invoice.Description,
            CompanyId = invoice.IssuerCompanyId,
            CounterPartyCompanyId = invoice.ReceiverCompanyId
        };
    }

    public static Invoice ToInvoice(this InvoiceDto invoice)
    {
        return new Invoice(
            invoice.Id,
            invoice.DateIssued,
            invoice.NetAmount,
            invoice.VatAmount,
            invoice.TotalAmount,
            invoice.Description,
            invoice.CompanyId,
            invoice.CounterPartyCompanyId);
    }
}
