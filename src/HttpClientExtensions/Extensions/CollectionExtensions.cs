using System.Collections.Generic;
using System.Linq;

namespace HttpClientExtensions.Extensions;

internal static class CollectionExtensions
{
    internal static bool IsNullOrEmpty<T>(this IEnumerable<T>? collection)
    {
        return collection == null || !collection.Any();
    }
}
