using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace WebIDLCollector.Utilities
{
    public static class Extensions
    {
        /// <summary>Performs an exact clone of all members of the specified item</summary>
        /// <typeparam name="T">The type of the item to clone</typeparam>
        /// <param name="item">The item to copy/clone</param>
        /// <returns>An exact deep clone of the original object</returns>
        public static T DeepClone<T>(this T item)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, item);
                stream.Position = 0;
                return (T)formatter.Deserialize(stream);
            }
        }

        /// <summary>Determies if an IColleciton is null or empty</summary>
        /// <param name="item">The item to check</param>
        /// <returns>true if null or empty; false otherwise</returns>
        public static bool IsNullOrEmpty(this ICollection item)
        {
            return item == null || item.Count == 0;
        }

        /// <summary>Does a merge of two IEnumerable lists if not null; otherwise just sets the list to the second list</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static IEnumerable<T> Merge<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            if (first != null)
            {
                if (second != null)
                {
                    first = first.Union(second);
                }
            }
            else
            {
                first = second;
            }

            return first;
        }

        public static SortedDictionary<T1, T2> ToSortedDictionary<T1, T2>(this IEnumerable<T2> source, Func<T2, T1> keySelector)
        {
            return new SortedDictionary<T1, T2>(source.ToDictionary(keySelector));
        }
    }
}