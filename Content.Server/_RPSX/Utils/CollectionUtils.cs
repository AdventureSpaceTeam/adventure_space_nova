using System.Linq;

namespace Content.Server.RPSX.Utils;

public static class CollectionUtils
{
    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source)
    {
        return source.Select((item, index) => (item, index));
    }

    public static bool ContainsAny<T>(this ICollection<T> source, IEnumerable<T> targets)
    {
        return source.Any(targets.Contains);
    }

    public static bool ContainsAny(this string source, IEnumerable<string> targets)
    {
        return targets.Any(source.Contains);
    }
}
