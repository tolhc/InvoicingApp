using System.Security.Claims;
using Invoicing.Api.Authentication;
using Invoicing.Api.Mappings;
using Invoicing.Api.Swagger;
using Invoicing.Api.Validation;
using Invoicing.Api.ViewModels;
using Invoicing.Application;
using Invoicing.Application.Interfaces;
using Invoicing.Core.Errors;
using Invoicing.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.HttpResults;
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
        var validationResult = user.TryGetValidCompanyId();
    
        if (validationResult.IsFailure)
        {
            return Results.Problem(new ProblemDetails
            {
                Status = (int)validationResult.Error.StatusCode,
                Detail = validationResult.Error.Description,
            }); 
        }
    
        var companyId = validationResult.Value;

        if (companyId != invoiceVm.IssuerCompanyId) // This is a bit of an assumption that the company can only post issued invoices
        {
            return Results.BadRequest("Issuer company id is different that the authorized one");
        }
        
        var invoice = invoiceVm.ToInvoice();
        var result = await invoiceService.CreateInvoiceAsync(invoice);
        
        if (result.IsFailure)
        {
            return Results.Problem(new ProblemDetails()
            {
                Status = (int)result.Error.StatusCode,
                Detail = result.Error.Description,
            });
        }

        return Results.Created($"invoices/sent?invoice_id={result.Value.InvoiceId}", result.Value);

    }).Produces<InvoiceVm>().RequireAuthorization("invoice_creation");

app.MapGet("/invoice/sent", async ([AsParameters] InvoiceRequestVm invoiceRequestVm, IInvoiceService invoiceService, ClaimsPrincipal user) =>
{
    //TODO: add validator with result 
    var validationResult = user.TryGetValidCompanyId();
    
    if (validationResult.IsFailure)
    {
        return Results.Problem(new ProblemDetails
        {
            Status = (int)validationResult.Error.StatusCode,
            Detail = validationResult.Error.Description,
        }); 
    }
    
    var companyId = validationResult.Value;
    
    var request = invoiceRequestVm.ToInvoiceRequest(companyId);
    var result = await invoiceService.GetSentInvoicesAsync(request);
    
    if (result.IsFailure)
    {
        return Results.Problem(new ProblemDetails()
        {
            Status = (int)result.Error.StatusCode,
            Detail = result.Error.Description,
        });
    }
    
    if (result.Value.Count == 0)
    {
        return Results.NotFound("No sent invoices found");
    }

    var resultVm = result.Value.Select(i => i.ToInvoiceVm());
    return Results.Ok(resultVm);
    
}).Produces<IEnumerable<InvoiceVm>>().RequireAuthorization();

app.MapGet("/invoice/received", async ([AsParameters] InvoiceRequestVm invoiceRequestVm, IInvoiceService invoiceService, ClaimsPrincipal user) =>
{
    var validationResult = user.TryGetValidCompanyId();
    
    if (validationResult.IsFailure)
    {
        return Results.Problem(new ProblemDetails
        {
            Status = (int)validationResult.Error.StatusCode,
            Detail = validationResult.Error.Description,
        }); 
    }
    
    var companyId = validationResult.Value;
    
    var request = invoiceRequestVm.ToInvoiceRequest(companyId);
    var result = await invoiceService.GetReceivedInvoicesAsync(request);
    
    if (result.IsFailure)
    {
        return Results.Problem(new ProblemDetails()
        {
            Status = (int)result.Error.StatusCode,
            Detail = result.Error.Description,
        });
    }

    var resultVm = result.Value.Select(i => i.ToInvoiceVm());
    return Results.Ok(resultVm);
    
}).Produces<IEnumerable<InvoiceVm>>().RequireAuthorization();

app.MapGet("/demotoken", (DemoTokenGeneratorService tokenGeneratorService, [FromQuery] Guid companyId, [FromQuery] string role = "User") =>
{
    var token = tokenGeneratorService.GenerateToken(companyId, role);
    return Results.Ok(token);
}).Produces<string>();

app.Run();
