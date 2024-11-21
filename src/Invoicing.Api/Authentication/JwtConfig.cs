﻿namespace Invoicing.Api.Authentication;

public record JwtConfig
{
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public required string Key { get; init; }
}