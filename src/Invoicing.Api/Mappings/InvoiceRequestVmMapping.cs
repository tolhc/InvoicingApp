using Invoicing.Api.ViewModels;
using Invoicing.Core.Models;

namespace Invoicing.Api.Mappings;

public static class InvoiceRequestVmMapping
{
    public static InvoiceRequest ToInvoiceRequest(this InvoiceRequestVm invoiceRequestVm, Guid companyId)
    {
        return new InvoiceRequest(
            companyId,
            invoiceRequestVm.InvoiceId, 
            invoiceRequestVm.CounterPartyCompanyId,
            invoiceRequestVm.DateIssued);
    }
}