namespace Invoicing.Infrastructure.Data.Models;

public record struct UserDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public Guid CompanyId { get; init; }
}
