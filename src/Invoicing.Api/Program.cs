using System.Security.Claims;
using Invoicing.Api.Authentication;
using Invoicing.Api.Mappings;
using Invoicing.Api.Swagger;
using Invoicing.Api.ViewModels;
using Invoicing.Application;
using Invoicing.Application.Interfaces;
using Invoicing.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.AddSwaggerWithAuthentication());


builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddInvoicingApiJwtBearer(builder.Configuration.GetSection(nameof(JwtConfig)).Get<JwtConfig>()!);
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("invoice_creation", policy => policy.RequireRole(KnownRoles.User));

builder.Services.AddSingleton<DemoTokenGeneratorService>();

builder.Services.AddApplication();
builder.Services.AddInfrastructure();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/invoice",
    async ([FromBody] InvoiceVm invoiceVm, IInvoiceService invoiceService, ClaimsPrincipal user) =>
    {
        //TODO: add validator with result 
        var companyIdString = user.FindFirst(KnownClaimTypes.CompanyId)?.Value;

        if (string.IsNullOrEmpty(companyIdString))
        {
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status403Forbidden,
                Detail = "No companyId specified",
            };
            return Results.Problem(problemDetails.Detail, statusCode: problemDetails.Status);
            
        }

        if (!Guid.TryParse(companyIdString, out var companyId))
        {
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status403Forbidden,
                Detail = "Invalid companyId specified",
            };
            return Results.Problem(problemDetails.Detail, statusCode: problemDetails.Status);
        }

        if (companyId != invoiceVm.IssuerCompanyId)
        {
            return Results.BadRequest("Issuer company id is different that the authorized one");
        }
        
        var invoice = invoiceVm.ToInvoice();
        var result = await invoiceService.CreateInvoiceAsync(invoice);
        return Results.Ok(result.ToInvoiceVm());

    }).Produces<InvoiceVm>().RequireAuthorization("invoice_creation");

app.MapGet("/invoice/sent", async ([AsParameters] InvoiceRequestVm invoiceRequestVm, IInvoiceService invoiceService, ClaimsPrincipal user) =>
{
    //TODO: add validator with result 
    var companyIdString = user.FindFirst(KnownClaimTypes.CompanyId)?.Value;

    if (string.IsNullOrEmpty(companyIdString))
    {
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status403Forbidden,
            Detail = "No companyId specified",
        };
        return Results.Problem(problemDetails.Detail, statusCode: problemDetails.Status);
    }

    if (!Guid.TryParse(companyIdString, out var companyId))
    {
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status403Forbidden,
            Detail = "Invalid companyId specified",
        };
        return Results.Problem(problemDetails.Detail, statusCode: problemDetails.Status);
    }
    
    var request = invoiceRequestVm.ToInvoiceRequest(companyId);
    var result = await invoiceService.GetSentInvoicesAsync(request);
    if (result.Count == 0)
    {
        return Results.NotFound("No sent invoices found");
    }

    var resultVm = result.Select(i => i.ToInvoiceVm());
    return Results.Ok(resultVm);
    
}).Produces<IEnumerable<InvoiceVm>>().RequireAuthorization();

app.MapGet("/invoice/received", async ([AsParameters] InvoiceRequestVm invoiceRequestVm, IInvoiceService invoiceService, ClaimsPrincipal user) =>
{
    //TODO: add validator with result 
    var companyIdString = user.FindFirst(KnownClaimTypes.CompanyId)?.Value;

    if (string.IsNullOrEmpty(companyIdString))
    {
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status403Forbidden,
            Detail = "No companyId specified",
        };
        return Results.Problem(problemDetails.Detail, statusCode: problemDetails.Status);
            
    }

    if (!Guid.TryParse(companyIdString, out var companyId))
    {
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status403Forbidden,
            Detail = "Invalid companyId specified",
        };
        return Results.Problem(problemDetails.Detail, statusCode: problemDetails.Status);
    }
    
    var request = invoiceRequestVm.ToInvoiceRequest(companyId);
    var result = await invoiceService.GetReceivedInvoicesAsync(request);
    
    if (result.Count == 0)
    {
        return Results.NotFound("No received invoices found");
    }

    var resultVm = result.Select(i => i.ToInvoiceVm());
    return Results.Ok(resultVm);
    
}).Produces<IEnumerable<InvoiceVm>>().RequireAuthorization();

app.MapGet("/demotoken", (DemoTokenGeneratorService tokenGeneratorService, [FromQuery] Guid companyId, [FromQuery] string role = "User") =>
{
    var token = tokenGeneratorService.GenerateToken(companyId, role);
    return Results.Ok(token);
}).Produces<string>();

app.Run();
