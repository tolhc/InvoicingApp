using Invoicing.Application.Interfaces;
using Invoicing.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Invoicing.Application;

public static class ServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IInvoiceService, InvoiceService>();
    }
}
