using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace CommandEngine
{
    public static class LinqExtensions
    {
        /// <returns>Returns the last object in <paramref name="array"/>. Throws an exception if <paramref name="array"/> is null or empty</returns>
        public static T Last<T>(this T[] array) => array[array.Length - 1];
        /// <returns>Returns the last object in <paramref name="list"/>. Throws an exception if <paramref name="list"/> is null or empty</returns>
        public static T Last<T>(this IList<T> list) => list[list.Count - 1];

        /// <returns>Returns the last index for <paramref name="array"/></returns>
        public static int LastIndex<T>(this T[] array) => array.Length - 1;
        /// <returns>Returns the last index for <paramref name="list"/></returns>
        public static int LastIndex<T>(this IList<T> list) => list.Count - 1;

        /// <summary>Sets the last index for <paramref name="array"/> to <paramref name="value"/></summary>
        public static void SetLast<T>(this T[] array, T value)
        {
            array[array.Length - 1] = value;
        }
        /// <summary>Sets the last index for <paramref name="list"/> to <paramref name="value"/></summary>
        public static void SetLast<T>(this IList<T> list, T value)
        {
            list[list.Count - 1] = value;
        }

        public static void Add<TKey, TValue>(this Dictionary<TKey, TValue> dict, KeyValuePair<TKey, TValue> kvp)
        {
            dict.Add(kvp.Key, kvp.Value);
        }

        public static T RandomItem<T>(this T[] array) => array[CommandEngine.Random.NextInt(array.Length)];
        public static T RandomItem<T>(this IList<T> list) => list[CommandEngine.Random.NextInt(list.Count)];
        public static T RandomItem<T>(this IEnumerable<T> enumerable) => enumerable.ElementAt(CommandEngine.Random.NextInt(enumerable.Count()));
        public static T RandomItem<T>(this IEnumerable<T> enumerabe, Func<T, bool> predicate) => enumerabe.Where(predicate).RandomItem();

        public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> dict, IEnumerable<TKey> keys, TValue value = default(TValue))
        {
            foreach (var key in keys)
                dict.Add(key, value);
        }
        public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> dict, IEnumerable<TKey> keys, IEnumerable<TValue> values)
        {
            int length = keys.Count();

            if (length != values.Count())
                throw new ArgumentException("There must be equal amounts of keys and values.");

            foreach ((TKey First, TValue Second) pair in keys.Zip(values))
            {
                dict.Add(pair.First, pair.Second);
            }
        }

        /// <summary>
        /// Adds a key value pair, or updates it to the passed value.
        /// </summary>
        /// <param name="key">The key associated with the value being added or updated.</param>
        /// <param name="value">The value that will be added or replacing the existing one</param>
        /// <returns>true if the value was added or updated, false otherwise. </returns>
        public static bool TryAddOrUpdate<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if (!dict.TryAdd(key, value))
                return dict.TryUpdate(key, value, dict[key]);
            return dict.TryAdd(key, value);
        }

        public static bool Contains<T>(this T[] array, Func<T, bool> predicate)
        {
            for (int i = 0; i < array.Length; i++)
                if (predicate.Invoke(array[i]))
                    return true;
            return false;
        }
        public static bool Contains<T>(this IList<T> list, Func<T, bool> predicate)
        {
            for (int i = 0; i < list.Count; i++)
                if (predicate.Invoke(list[i]))
                    return true;
            return false;
        }
        public static bool Contains<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
        {
            foreach (var item in enumerable)
                if (predicate.Invoke(item))
                    return true;
            return false;
        }

        public static bool TryFirst<T>(this T[] array, Func<T, bool> predicate, out T firstItem)
        {
            for (int i = 0; i < array.Length; i++)
                if (predicate.Invoke(array[i]))
                {
                    firstItem = array[i];
                    return true;
                }
            firstItem = default(T);
            return false;
        }
        public static bool TryFirst<T>(this IList<T> list, Func<T, bool> predicate, out T firstItem)
        {
            for (int i = 0; i < list.Count; i++)
                if (predicate.Invoke(list[i]))
                {
                    firstItem = list[i];
                    return true;
                }
            firstItem = default(T);
            return false;
        }
        public static bool TryFirst<T>(this IEnumerable<T> enumberable, Func<T, bool> predicate, out T firstItem)
        {
            foreach(var item in enumberable)
                if(predicate.Invoke(item))
                {
                    firstItem = item;
                    return true;
                }
            firstItem = default(T);
            return false;
        }

        public static T Least<T>(this T[] array) where T : IComparable<T>
        {
            if (!array.Any())
                return default;

            T smallest = array[0];
            for (int i = 1; i < array.Length; ++i)
                if (array[i].CompareTo(smallest) < 0)
                    smallest = array[i];
            return smallest;
        }
        public static T Least<T>(this IList<T> list) where T:IComparable<T>
        {
            if (!list.Any())
                return default;

            T smallest = list[0];
            for(int i = 1; i < list.Count; ++i)
                if (list[i].CompareTo(smallest) < 0)
                    smallest = list[i];
            return smallest;
        }
        public static T Least<T>(this IEnumerable<T> enumerable) where T : IComparable<T>
        {
            if (!enumerable.Any())
                return default;

            T smallest = enumerable.First();
            foreach (var item in enumerable)
                if (item.CompareTo(smallest) < 0)
                    smallest = item;
            return smallest;
        }

        public static T Least<T>(this T[] array, Comparison<T> c)
        {
            if (!array.Any())
                return default;

            T smallest = array[0];
            for (int i = 1; i < array.Length; ++i)
                if (c.Invoke(array[i], smallest) < 0)
                    smallest = array[i];
            return smallest;
        }
        public static T Least<T>(this IList<T> list, Comparison<T> c)
        {
            if (!list.Any())
                return default;

            T smallest = list[0];
            for (int i = 1; i < list.Count; ++i)
                if (c.Invoke(list[i], smallest) < 0)
                    smallest = list[i];
            return smallest;
        }
        public static T Least<T>(this IEnumerable<T> enumerable, Comparison<T> c)
        {
            if (!enumerable.Any())
                return default;

            T smallest = enumerable.First();
            foreach (var item in enumerable)
                if (c.Invoke(item, smallest) < 0)
                    smallest = item;
            return smallest;
        }

        public static T Greatest<T>(this T[] array) where T : IComparable<T>
        {
            if (!array.Any())
                return default;

            T biggest = array[0];
            for (int i = 1; i < array.Length; ++i)
                if (array[i].CompareTo(biggest) > 0)
                    biggest = array[i];
            return biggest;
        }
        public static T Greatest<T>(this IList<T> list) where T : IComparable<T>
        {
            if (!list.Any())
                return default;

            T biggest = list[0];
            for (int i = 1; i < list.Count; ++i)
                if (list[i].CompareTo(biggest) > 0)
                    biggest = list[i];
            return biggest;
        }
        public static T Greatest<T>(this IEnumerable<T> enumerable) where T : IComparable<T>
        {
            if (!enumerable.Any())
                return default;

            T biggest = enumerable.First();
            foreach (var item in enumerable)
                if (item.CompareTo(biggest) > 0)
                    biggest = item;
            return biggest;
        }

        public static T Greatest<T>(this T[] array, Comparison<T> c)
        {
            if (!array.Any())
                return default;

            T biggest = array[0];
            for (int i = 1; i < array.Length; ++i)
                if (c.Invoke(array[i], biggest) > 0)
                    biggest = array[i];
            return biggest;
        }
        public static T Greatest<T>(this IList<T> list, Comparison<T> c)
        {
            if (!list.Any())
                return default;

            T biggest = list[0];
            for (int i = 1; i < list.Count; ++i)
                if (c.Invoke(list[i], biggest) > 0)
                    biggest = list[i];
            return biggest;
        }
        public static T Greatest<T>(this IEnumerable<T> enumerable, Comparison<T> c)
        {
            if (!enumerable.Any())
                return default;

            T biggest = enumerable.First();
            foreach (var item in enumerable)
                if (c.Invoke(item, biggest) > 0)
                    biggest = item;
            return biggest;
        }

        /// <summary>Create a string with a <paramref name="seperator"/> between the items./// </summary>
        /// <returns>A string with <paramref name="seperator"/> between the items present in <paramref name="enumerable"/></returns>
        public static string ToString<T>(this IEnumerable<T> enumerable, string seperator)
        {
            int count = enumerable.Count();
            string first = enumerable.FirstOrDefault().ToString();
            // Since we cant know the total length we need, just assume that first is the average, and use it to allocate capacity.
            StringBuilder builder = new StringBuilder((count * seperator.Length) + (first.Length * count));

            builder.Append(first);
            foreach (var item in enumerable.Skip(1))
            {
                builder.Append($"{seperator}{item}");
            }
            return builder.ToString();
        }
        /// <summary>Create a string with a <paramref name="seperator"/> between the items./// </summary>
        /// <param name="toString">Function used to convert items to strings.</param>
        /// <returns>A string with <paramref name="seperator"/> between the items present in <paramref name="enumerable"/></returns>
        public static string ToString<T>(this IEnumerable<T> enumerable, Func<T, string> toString, string seperator)
        {
            int count = enumerable.Count();
            string first = toString(enumerable.FirstOrDefault());
            // Since we cant know the total length we need, just assume that first is the average, and use it to allocate capacity.
            StringBuilder builder = new StringBuilder((count * seperator.Length) + (first.Length * count));

            builder.Append(first);
            foreach (var item in enumerable.Skip(1))
            {
                builder.Append($"{seperator}{toString(item)}");
            }
            return builder.ToString();
        }

        public static void EnqueueAll<T>(this Queue<T> q, IEnumerable<T> enumerable)
        {
            foreach (var v in enumerable)
                q.Enqueue(v);
        }
    }
}