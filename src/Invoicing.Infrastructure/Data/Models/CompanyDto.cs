namespace Invoicing.Infrastructure.Data.Models;

public record struct CompanyDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
}
