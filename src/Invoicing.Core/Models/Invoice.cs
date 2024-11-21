namespace Invoicing.Core.Models;

public record Invoice(
    Guid InvoiceId,
    DateTime DateIssued,
    float NetAmount,
    float VatAmount,
    float TotalAmount,
    string Description,
    Guid IssuerCompanyId,
    Guid ReceiverCompanyId);