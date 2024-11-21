namespace Invoicing.Infrastructure.Data.Models;

public record struct InvoiceDto
{
    public Guid Id { get; init; }
    public DateTime DateIssued { get; init; }
    public float NetAmount { get; init; }
    public float VatAmount { get; init; }
    public float TotalAmount { get; init; }
    public string Description { get; init; }
    public Guid CompanyId { get; init; }
    public Guid CounterPartyCompanyId { get; init; }
}