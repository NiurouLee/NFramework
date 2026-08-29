using System;
using System.Collections.Generic;

namespace  NFramework.Core
{
    /// <summary>
    /// 栈字典，本质是引用一个字典对象
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public struct StructDictionary<TKey, TValue> : IDisposable
    {
        private Dictionary<TKey, TValue> _dict;

        public StructDictionary(int capacity = 0)
        {
            _dict = DictionaryPool.Alloc<TKey, TValue>();
        }

        public void Dispose()
        {
            if (_dict != null)
            {
                _dict.Dispose();
                _dict = null;
            }
        }

        public void Add(TKey key, TValue value) => _dict.Add(key, value);
        public bool ContainsKey(TKey key) => _dict.ContainsKey(key);
        public bool Remove(TKey key) => _dict.Remove(key);
        public void Clear() => _dict.Clear();
        public int Count => _dict.Count;

        public TValue this[TKey key]
        {
            get => _dict[key];
            set => _dict[key] = value;
        }
    }

    public static class DicExtensions
    {
        public static void Dispose<K, V>(this Dictionary<K, V> inDic)
        {
            if (inDic == null)
            {
                return;
            }

            DictionaryPool.Free(inDic);
        }
    }
}