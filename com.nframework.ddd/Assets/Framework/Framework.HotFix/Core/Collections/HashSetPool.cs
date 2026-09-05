using System;
using System.Collections;
using System.Collections.Generic;

namespace  NFramework.Core
{
    public static class HashSetPool
    {
        private static readonly Dictionary<Type, IList> _cache = new();
        private static readonly HashSet<int> _hash = new();
        private static readonly object _lock = new();

        public static HashSet<T> Alloc<T>()
        {
            lock (_lock)
            {
                var type = typeof(T);
                if (_cache.TryGetValue(type, out var list) && list.Count > 0)
                {
                    var result = list[^1];
                    list.RemoveAt(list.Count - 1);
                    var hash = result.GetHashCode();
                    _hash.Remove(hash);
                    return result as HashSet<T>;
                }

                return new HashSet<T>();
            }
        }

        public static bool Free<T>(HashSet<T> set)
        {
            if (set == null) return false;

            lock (_lock)
            {
                var hash = set.GetHashCode();
                if (_hash.Contains(hash)) return false;

                set.Clear();

                var type = typeof(T);
                if (!_cache.TryGetValue(type, out var cacheList))
                {
                    cacheList = new List<object>();
                    _cache.Add(type, cacheList);
                }

                cacheList.Add(set);
                _hash.Add(hash);
                return true;
            }
        }

        public static void ClearPool()
        {
            lock (_lock)
            {
                _cache.Clear();
                _hash.Clear();
            }
        }
    }
}