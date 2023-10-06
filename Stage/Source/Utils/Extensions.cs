using System;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Collections.Generic;

namespace Stage.Utils
{
    internal static class Extensions
    {
        public static byte[] Bytes(this string str)
        {
            return Encoding.ASCII.GetBytes(str);
        }

        public static T Average<T>(this ICollection<T> collection) where T : IFloatingPoint<T>
        {
            T total = T.Zero;

            foreach (T t in collection)
            {
                total += t;
            }

            T count = (T)Convert.ChangeType(collection.Count, typeof(T));
            return total / count;
        }

        public static string ToCollectionString<T>(this ICollection<T> collection)
        {
            string result = "[ ";

            result += collection.ElementAt(0);

            int i = 0;
            foreach (T t in collection)
            {
                if (i == 0)
                {
                    i++;
                    continue;
                }

                result += ",";
                result += t.ToString();

                i++;
            }

            result += " ]";

            return result;
        }
    }
}
