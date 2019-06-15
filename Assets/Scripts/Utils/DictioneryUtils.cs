using System.Collections.Generic;
using System.Linq;

namespace Utils
{
    public static class DictioneryUtils
    {
        public static Dictionary<K, V>[] Split<K, V>(this Dictionary<K, V> dict, int chunks)
        {
            return
                dict .Select((kvp, n) => new { kvp, k = n % chunks })
                    .GroupBy(x => x.k, x => x.kvp)
                    .Select(x => x.ToDictionary(y => y.Key, y => y.Value))
                    .ToArray();
        }
    }
}