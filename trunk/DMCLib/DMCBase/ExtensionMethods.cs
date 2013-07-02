using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DMCBase
{
    public static class ExtensionMethods
    {
        private static Random rand = new Random(DateTime.Now.Second);

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rand.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static int ToInt32(this byte[] buffer, int startIndex)
        {
            if ((buffer.Length - startIndex) < 4)
            {
                return 0;
            }
            
            int number = 0;

            number += buffer[startIndex + 0] << 24;
            number += buffer[startIndex + 1] << 16;
            number += buffer[startIndex + 2] << 8;
            number += buffer[startIndex + 3];

            return number;
        }

        public static byte[] ToByteArray(this int number)
        {
            return new byte[] 
                {
                    (byte)(number >> 24),
                    (byte)(number >> 16),
                    (byte)(number >> 08),
                    (byte)(number >> 00),
                };
        }

        public static object GetDefaultObject(this Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            else
            {
                return null;
            }
        }
    }
}
