using Invoicing.Api.ViewModels;
using Invoicing.Core.Models;

namespace Invoicing.Api.Mappings;

public static class InvoiceVmMappings
{
    public static InvoiceVm ToInvoiceVm(this Invoice invoice)
    {
        return new InvoiceVm(
            invoice.InvoiceId.ToString(),
            invoice.DateIssued.ToString("O"),
            invoice.NetAmount,
            invoice.VatAmount,
            invoice.TotalAmount,
            invoice.Description,
            invoice.IssuerCompanyId.ToString(),
            invoice.ReceiverCompanyId.ToString());
    }
    
    public static Invoice ToInvoice(this InvoiceVm invoiceVm)
    {
        //TODO: add checks
        return new Invoice(
            Guid.Parse(invoiceVm.InvoiceId),
            DateTime.Parse(invoiceVm.DateIssued),
            invoiceVm.NetAmount,
            invoiceVm.VatAmount,
            invoiceVm.TotalAmount,
            invoiceVm.Description,
            Guid.Parse(invoiceVm.IssuerCompanyId),
            Guid.Parse(invoiceVm.ReceiverCompanyId));
    }
}