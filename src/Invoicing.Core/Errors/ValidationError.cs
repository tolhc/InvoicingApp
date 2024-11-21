using System.Net;

namespace Invoicing.Core.Errors;

public record struct ValidationError(string Description, HttpStatusCode StatusCode);