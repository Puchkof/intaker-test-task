using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Intaker.TaskManager.Api.Routes;
using Microsoft.Extensions.DependencyInjection;

namespace Intaker.TaskManager.IntegrationTests;

public abstract class IntegrationTestBase : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    protected readonly HttpClient Client;
    protected readonly CustomWebApplicationFactory Factory;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    protected IntegrationTestBase(CustomWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    protected async Task<T?> GetAsync<T>(string endpoint)
    {
        var response = await Client.GetAsync(endpoint);
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadFromJsonAsync<T>(JsonOptions);
    }

    protected async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
    {
        var content = new StringContent(
            JsonSerializer.Serialize(data), 
            Encoding.UTF8, 
            "application/json");
            
        var response = await Client.PostAsync(endpoint, content);
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadFromJsonAsync<TResponse>(JsonOptions);
    }

    protected async Task<TResponse?> PatchAsync<TRequest, TResponse>(string endpoint, TRequest data)
    {
        var content = new StringContent(
            JsonSerializer.Serialize(data), 
            Encoding.UTF8, 
            "application/json");
            
        var response = await Client.PatchAsync(endpoint, content);
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadFromJsonAsync<TResponse>(JsonOptions);
    }
}