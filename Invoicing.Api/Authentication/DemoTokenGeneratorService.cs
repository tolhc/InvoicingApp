﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Invoicing.Api.Authentication;

public class DemoTokenGeneratorService
{
    private readonly JwtConfig _jwtConfig;

    public DemoTokenGeneratorService(IOptions<JwtConfig> jwtConfig)
    {
        _jwtConfig = jwtConfig.Value;
    }
    
    public string GenerateToken(Guid companyId, string role = KnownRoles.User)
    {
        var claims = new[]
        {
            new Claim(KnownClaimTypes.CompanyId, companyId.ToString()),
            new Claim(ClaimTypes.Role, role)
        };
    
        var token = new JwtSecurityToken(
            issuer: _jwtConfig.Issuer,
            audience: _jwtConfig.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(60),
            notBefore: DateTime.UtcNow,
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Key)), 
                SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}