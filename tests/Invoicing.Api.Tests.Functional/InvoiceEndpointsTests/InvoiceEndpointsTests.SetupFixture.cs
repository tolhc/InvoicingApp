using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Invoicing.Application.Interfaces;
using Invoicing.Core.Errors;
using Invoicing.Core.Models;
using Invoicing.Core.Results;
using Invoicing.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace Invoicing.Api.Tests.Functional.InvoiceEndpointsTests;


public partial class InvoiceEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly Mock<IDatabaseContext> _dbContextMock;
    
    private readonly StringContent _defaultPostedInvoiceVmContent;
    private readonly string _defaultPostedInvoiceVmString;

    public InvoiceEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _dbContextMock = new Mock<IDatabaseContext>();

        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IDatabaseContext>(); // Remove existing repository
                services.AddSingleton<IDatabaseContext>(_dbContextMock.Object); // Add the mocked one
            });
        }).CreateClient();
        
        
        _defaultPostedInvoiceVmString = File.ReadAllText("InvoiceEndpointsTests/InvoiceVmSample.json");
        _defaultPostedInvoiceVmContent = new StringContent(_defaultPostedInvoiceVmString, Encoding.UTF8, "application/json");
        
        var token = _client.GetDemoToken("BD2717F2-BAE0-43CA-AC43-43F48E6A1397").GetAwaiter().GetResult();
        _client.DefaultRequestHeaders.Authorization = null;
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
    
    

    // [Fact]
    // public async Task GetSentInvoices_Returns_NotFound_When_No_Invoice()
    // {
    //     // Arrange
    //     _invoiceRepositoryMock.Setup(repo => repo.ReadInvoicesAsync(It.IsAny<InvoiceRequest>()))
    //         .ReturnsAsync(Result<IEnumerable<Invoice>, DbError>.Success(Enumerable.Empty<Invoice>()));
    //
    //     // Act
    //     var response = await _client.GetAsync("/invoice/sent");
    //
    //     // Assert
    //     Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    // }
    
}