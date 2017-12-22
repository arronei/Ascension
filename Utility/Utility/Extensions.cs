using System;
using System.Collections.Generic;
using System.Linq;

namespace MS.Internal.Utility
{
    public static class Extensions
    {
        public static IEnumerable<T> OrderBySequence<T, TId>(this IEnumerable<T> source, IEnumerable<TId> order, Func<T, TId> idSelector)
        {
            var lookup = source.ToLookup(idSelector, t => t);
            foreach (var id in order)
            {
                foreach (var t in lookup[id])
                {
                    yield return t;
                }
            }
        }

        public static IEnumerable<string> OrderBySequence(this IEnumerable<string> source, IEnumerable<string> order)
        {
            var lookup = source.ToLookup(a => a, t => t);
            foreach (var item in order)
            {
                foreach (var t in lookup.Where(b => b.Any(c => c.Contains(item))).Select(a=>a.Key))
                {
                    yield return t;
                }
            }
        }
    }
}