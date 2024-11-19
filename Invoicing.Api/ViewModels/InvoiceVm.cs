using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Invoicing.Api.ViewModels;

public record InvoiceVm(
    [property: JsonPropertyName("invoice_id")] string InvoiceId,
    [property: JsonPropertyName("date_issued")] string DateIssued,
    [property: JsonPropertyName("net_amount")] float NetAmount,
    [property: JsonPropertyName("vat_amount")] float VatAmount,
    [property: JsonPropertyName("total_amount")] float TotalAmount,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("company_id")] string IssuerCompanyId,
    [property: JsonPropertyName("counter_party_company_id")] string ReceiverCompanyId);