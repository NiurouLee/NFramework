using System;
using System.Collections;
using System.Collections.Generic;

namespace  NFramework.Core
{
    public static class ListPool
    {
        private static Dictionary<Type, IList> _cache = new Dictionary<Type, IList>();
        private static HashSet<int> _hash = new HashSet<int>();

        public static List<T> Alloc<T>()
        {
            var type = typeof(T);
            if (_cache.TryGetValue(type, out var list) && list.Count > 0)
            {
                var result = list[^1];
                list.RemoveAt(list.Count);
                var hash = result.GetHashCode();
                _hash.Remove(hash);
                var result1 = result as List<T>;
                return result1;
            }

            return new List<T>();
        }

        public static bool Free<T>(List<T> inList)
        {
            var hash = inList.GetHashCode();
            if (_hash.Contains(hash))
            {
                return false;
            }

            inList.Clear();
            var c = inList.Capacity;
            if (c >= 16)
            {
                inList.TrimExcess();
            }

            var type = typeof(T);
            if (_cache.TryGetValue(type, out var list))
            {
                list.Add(inList);
            }

            list = new List<T>();
            _cache.Add(type, list);
            list.Add(inList);
            return true;
        }
    }


    public static class ListEx
    {
        public static void Dispose<T>(this List<T> inList)
        {
            ListPool.Free(inList);
        }
    }
}
