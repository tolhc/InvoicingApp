namespace Invoicing.Core.Models;

public record Company(Guid Id, string Name, ICollection<Guid> Users);