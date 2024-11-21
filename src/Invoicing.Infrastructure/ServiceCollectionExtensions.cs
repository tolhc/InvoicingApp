using Invoicing.Application.Interfaces;
using Invoicing.Infrastructure.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Invoicing.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
    }
}