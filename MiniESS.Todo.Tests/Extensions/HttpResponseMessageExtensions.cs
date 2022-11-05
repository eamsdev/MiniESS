using Newtonsoft.Json;

namespace MiniESS.Todo.Tests.Extensions;

public static class HttpResponseMessageExtensions
{
    public static async Task<T> DeserializeContentAsync<T>(this HttpResponseMessage responseMessage)
    {
        var content = await responseMessage.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<T>(content);
        Assert.NotNull(result);
        
        return result!;
    }
}