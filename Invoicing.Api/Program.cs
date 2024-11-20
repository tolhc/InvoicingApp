using Invoicing.Api.Mappings;
using Invoicing.Api.ViewModels;
using Invoicing.Application;
using Invoicing.Application.Interfaces;
using Invoicing.Infrastructure;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// builder.Services.AddAuthentication().AddJwtBearer();

builder.Services.AddApplication();
builder.Services.AddInfrastructure();

var app = builder.Build();

// app.UseAuthentication();
// app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/invoice", async ([FromBody] InvoiceVm invoiceVm, IInvoiceService invoiceService) =>
{
    var invoice = invoiceVm.ToInvoice();
    var result = await invoiceService.CreateInvoiceAsync(invoice);
    return Results.Ok(result.ToInvoiceVm());
    
}).Produces<InvoiceVm>();

app.MapGet("/invoice/sent", async ([AsParameters] InvoiceRequestVm invoiceRequestVm, IInvoiceService invoiceService) =>
{
    var companyId = Guid.NewGuid(); //TODO: take from claims
    var request = invoiceRequestVm.ToInvoiceRequest(companyId);
    var result = await invoiceService.GetSentInvoicesAsync(request);
    if (result.Count == 0)
    {
        return Results.NotFound("No sent invoices found");
    }

    var resultVm = result.Select(i => i.ToInvoiceVm());
    return Results.Ok(resultVm);
    
}).Produces<IEnumerable<InvoiceVm>>();

app.MapGet("/invoice/received", async ([AsParameters] InvoiceRequestVm invoiceRequestVm, IInvoiceService invoiceService) =>
{
    var companyId = Guid.NewGuid(); //TODO: take from claims
    
    var request = invoiceRequestVm.ToInvoiceRequest(companyId);
    var result = await invoiceService.GetReceivedInvoicesAsync(request);
    
    if (result.Count == 0)
    {
        return Results.NotFound("No received invoices found");
    }

    var resultVm = result.Select(i => i.ToInvoiceVm());
    return Results.Ok(resultVm);
    
}).Produces<IEnumerable<InvoiceVm>>();


app.Run();
