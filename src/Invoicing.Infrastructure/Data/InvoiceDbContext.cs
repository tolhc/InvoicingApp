using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Invoicing.Infrastructure.Data;

public class InvoiceDbContext : IDatabaseContext
{
    private readonly string _invoicingDbConnectionString;

    public InvoiceDbContext(IConfiguration configuration)
    {
        _invoicingDbConnectionString = configuration.GetConnectionString("InvoicingDb") ??
                                       throw new NullReferenceException(
                                           "The InvoicingDb connection string is missing.");
    }

    public async Task<int> ExecuteAsync(string query, object parameters)
    {
        await using var connection = new SqlConnection(_invoicingDbConnectionString);
        return await connection.ExecuteAsync(query, parameters);
    }

    public async Task<IEnumerable<T>> QueryAsync<T>(string query, object parameters)
    {
        await using var connection = new SqlConnection(_invoicingDbConnectionString);
        return await connection.QueryAsync<T>(query, parameters);
    }
}
