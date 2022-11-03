using System.Net.Mime;
using System.Text;
using Newtonsoft.Json;

namespace MiniESS.Todo.Tests.Utils;

public static class HttpClientExtensions
{
    public static async Task<HttpResponseMessage> GetRouteAsync(this HttpClient httpClient, string route)
    {
        return await httpClient.GetAsync($"{httpClient.BaseAddress}{route}");
    }
    
    public static async Task<HttpResponseMessage> DeleteRouteAsync(this HttpClient httpClient, string route)
    {
        return await httpClient.DeleteAsync($"{httpClient.BaseAddress}{route}");
    }
    
    public static async Task<HttpResponseMessage> PutRouteAsJsonAsync(this HttpClient httpClient, string route, object? content)
    {
        return await httpClient.PutAsync($"{httpClient.BaseAddress}{route}", new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, MediaTypeNames.Application.Json));
    }
    
    public static async Task<HttpResponseMessage> PostRouteAsJsonAsync(this HttpClient httpClient, string route, object? content)
    {
        return await httpClient.PostAsync($"{httpClient.BaseAddress}{route}", new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, MediaTypeNames.Application.Json));
    }
}