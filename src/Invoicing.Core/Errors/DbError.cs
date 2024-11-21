namespace Invoicing.Core.Errors;

public record struct DbError(string Description, ErrorCode ErrorCode);