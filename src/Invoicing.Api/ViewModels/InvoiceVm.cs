using System.Text.Json.Serialization;

namespace Invoicing.Api.ViewModels;

public record InvoiceVm(
    [property: JsonPropertyName("invoice_id")] Guid? Id,
    [property: JsonPropertyName("date_issued")] DateTime DateIssued,
    [property: JsonPropertyName("net_amount")] float NetAmount,
    [property: JsonPropertyName("vat_amount")] float VatAmount,
    [property: JsonPropertyName("total_amount")] float TotalAmount,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("company_id")] Guid IssuerCompanyId,
    [property: JsonPropertyName("counter_party_company_id")] Guid ReceiverCompanyId);