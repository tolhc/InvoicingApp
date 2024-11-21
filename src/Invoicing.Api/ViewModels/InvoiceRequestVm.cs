using Microsoft.AspNetCore.Mvc;

namespace Invoicing.Api.ViewModels;

public record InvoiceRequestVm(
    [property: FromQuery(Name = "invoice_id")]
    Guid? InvoiceId,
    [property: FromQuery(Name = "counter_party_company")]
    Guid? CounterPartyCompanyId,
    [property: FromQuery(Name = "date_issued")]
    DateTime? DateIssued);