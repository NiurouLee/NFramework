using System;
using System.Collections;
using System.Collections.Generic;

namespace  NFramework.Core
{
    public static class DictionaryPool
    {
        private static readonly Dictionary<Type, IList> _cache = new();
        private static readonly HashSet<int> _hash = new();
        private static readonly object _lock = new();

        public static Dictionary<TKey, TValue> Alloc<TKey, TValue>()
        {
            lock (_lock)
            {
                var type = typeof(KeyValuePair<TKey, TValue>);
                if (_cache.TryGetValue(type, out var list) && list.Count > 0)
                {
                    var result = list[^1];
                    list.RemoveAt(list.Count - 1);
                    var hash = result.GetHashCode();
                    _hash.Remove(hash);
                    return result as Dictionary<TKey, TValue>;
                }

                return new Dictionary<TKey, TValue>();
            }
        }

        public static bool Free<TKey, TValue>(Dictionary<TKey, TValue> dict)
        {
            if (dict == null) return false;

            lock (_lock)
            {
                var hash = dict.GetHashCode();
                if (_hash.Contains(hash)) return false;

                dict.Clear();

                var type = typeof(KeyValuePair<TKey, TValue>);
                if (!_cache.TryGetValue(type, out var cacheList))
                {
                    cacheList = new List<object>();
                    _cache.Add(type, cacheList);
                }

                cacheList.Add(dict);
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

