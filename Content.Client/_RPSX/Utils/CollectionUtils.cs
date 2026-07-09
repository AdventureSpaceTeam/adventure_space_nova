using System.Linq;

namespace Content.Client.RPSX.Utils;

public static class CollectionUtils
{
    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source)
    {
        return source.Select((item, index) => (item, index));
    }
}
