using System;
using System.Collections.Generic;
using System.Linq;

namespace TextGen
{
    public static class EnumerableExtensions
    {
        public static Dictionary<TOut, int> CountItems<T, TOut>(this IEnumerable<T> enumerable, Func<T, TOut> selector)
        {
            return enumerable.GroupBy(selector, _ => 1).ToDictionary(g => g.Key, g => g.Count());
        }
    }
}