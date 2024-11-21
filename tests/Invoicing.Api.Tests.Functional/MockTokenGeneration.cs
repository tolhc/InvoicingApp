using System.Text;
using System.Text.Json;

namespace Invoicing.Api.Tests.Functional;

public static class MockTokenGeneration
{
    public static async Task<string> GetDemoToken(this HttpClient httpClient, string? companyId, string? userRole = "User")
    {

        var queryParams = new QueryStringBuilder()
            .Add("companyId", companyId)
            .Add("role", userRole)
            .Build();

        var requestUri = "/demotoken" + queryParams;

        var response = await httpClient.GetAsync(requestUri);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStreamAsync();
        return (await JsonSerializer.DeserializeAsync<string>(result))!;
    }


    public class QueryStringBuilder
    {
        private readonly Dictionary<string, string> _parameters = new();

        public QueryStringBuilder Add(string key, string? value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _parameters[key] = value;
            }
            return this;
        }

        public string Build()
        {
            if (_parameters.Count == 0)
            {
                return string.Empty;
            }

            var queryString = string.Join("&", _parameters.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
            return $"?{queryString}";
        }
    }
}
