using Invoicing.Api.ViewModels;
using Invoicing.Core.Models;

namespace Invoicing.Api.Mappings;

public static class InvoiceVmMappings
{
    public static InvoiceVm ToInvoiceVm(this Invoice invoice)
    {
        return new InvoiceVm(
            invoice.InvoiceId,
            invoice.DateIssued,
            invoice.NetAmount,
            invoice.VatAmount,
            invoice.TotalAmount,
            invoice.Description,
            invoice.IssuerCompanyId,
            invoice.ReceiverCompanyId);
    }
    
    public static Invoice ToInvoice(this InvoiceVm invoiceVm)
    {
        //TODO: add checks
        return new Invoice(
            invoiceVm.InvoiceId,
            invoiceVm.DateIssued,
            invoiceVm.NetAmount,
            invoiceVm.VatAmount,
            invoiceVm.TotalAmount,
            invoiceVm.Description,
            invoiceVm.IssuerCompanyId,
            invoiceVm.ReceiverCompanyId);
    }
}