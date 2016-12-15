using System.IO;
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
    }
}