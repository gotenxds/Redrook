using System.Collections.Generic;
using System.Linq;

namespace Utils
{
    public static class ListUtils
    {
        public static List<List<T>> Split<T>(this IList<T> source, int chunks)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunks)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }
    }
}