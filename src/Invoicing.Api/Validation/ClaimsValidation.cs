using System.Net;
using System.Security.Claims;
using Invoicing.Api.Authentication;
using Invoicing.Core.Errors;
using Invoicing.Core.Results;

namespace Invoicing.Api.Validation;

public static class ClaimsValidation
{
    public static Result<Guid, ValidationError> TryGetValidCompanyId(this ClaimsPrincipal user)
    {
        var companyIdString = user.FindFirst(KnownClaimTypes.CompanyId)?.Value;

        if (string.IsNullOrEmpty(companyIdString))
        {
            return new ValidationError("No companyId specified", HttpStatusCode.Forbidden);
        }

        if (!Guid.TryParse(companyIdString, out var companyId))
        {
            return new ValidationError("Invalid companyId specified", HttpStatusCode.Forbidden);
        }

        return companyId;
    }
}