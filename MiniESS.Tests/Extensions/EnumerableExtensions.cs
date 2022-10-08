using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniESS.Tests.Extensions;

public static class EnumerableExtensions
{
    public static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(this IEnumerable<T> enumerable)
    {
        foreach(var item in enumerable)
        {
            yield return await Task.FromResult(item);
        }
    } 
}