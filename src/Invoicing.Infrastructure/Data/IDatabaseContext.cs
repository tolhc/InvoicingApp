namespace Invoicing.Infrastructure.Data;

public interface IDatabaseContext
{
    Task<int> ExecuteAsync(string query, object parameters);
    Task<IEnumerable<T>> QueryAsync<T>(string query, object parameters);
}