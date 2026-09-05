using System;
using System.Collections;
using System.Collections.Generic;

namespace NFramework.Core
{
    public struct StructList<T> : IDisposable, IList<T>
    {
        private List<T> _list;

        public StructList(T item1)
        {
            _list = ListPool.Alloc<T>();
            _list.Add(item1);
        }

        public StructList(IList<T> items)
        {
            _list = ListPool.Alloc<T>();
            foreach (var item in items)
            {
                _list.Add(item);
            }
        }

        public StructList(T item1, T item2)
        {
            _list = ListPool.Alloc<T>();
            _list.Add(item1);
            _list.Add(item2);
        }

        public void Dispose()
        {
            _list.Dispose();
        }

        public void AddRange(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                _list.Add(item);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public void Add(T item)
        {
            _list.Add(item);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return _list.Remove(item);
        }

        public int Count => _list.Count;
        public bool IsReadOnly => ((IList)_list).IsReadOnly;

        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            _list.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        public T this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }
    }
}