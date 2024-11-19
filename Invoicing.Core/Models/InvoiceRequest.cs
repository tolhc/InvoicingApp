namespace Invoicing.Core.Models;

public record InvoiceRequest(Guid CompanyId, Guid? InvoiceId, Guid? CounterPartyCompanyId, DateTime? DateIssued);