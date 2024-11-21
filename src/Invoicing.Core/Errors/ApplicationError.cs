using System.Net;

namespace Invoicing.Core.Errors;

public record struct ApplicationError(string Description, HttpStatusCode StatusCode);
