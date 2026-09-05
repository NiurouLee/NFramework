using System;
using System.Collections.Generic;
using UnityEngine;

namespace  NFramework.Core
{
    /// <summary>
    /// Least Recently Used
    /// </summary>
    public class LRU<TKey, TValue>
    {
        public delegate bool OnRemoveEntry(TKey key, TValue value);

        private readonly Dictionary<TKey, Node> entries;
        private readonly int capacity;
        private Node head;
        private Node tail;
        public OnRemoveEntry onRemoveEntry;

        private class Node
        {
            public Node Next { get; set; }
            public Node Previous { get; set; }
            public TKey Key { get; set; }
            public TValue Value { get; set; }
        }

        public LRU(int capacity = 16)
        {
            if (capacity <= 0)
                throw new ArgumentOutOfRangeException(
                    "capacity",
                    "Capacity should be greater than zero");
            this.capacity = capacity;
            entries = DictionaryPool.Alloc<TKey, Node>();
            head = null;
        }

        public void Set(TKey key, TValue value)
        {
            Node entry;
            if (!entries.TryGetValue(key, out entry))
            {
                entry = new Node { Key = key, Value = value };
                entries.Add(key, entry);
            }
            else
            {
                entry.Value = value;
            }

            MoveToHead(entry);

            if (tail == null)
            {
                tail = head;
                return;
            }

            Node node = tail;
            int exceedNum = entries.Count - capacity;
            while ((exceedNum--) > 0 && (node != null))
            {
                if (onRemoveEntry != null)
                {
                    try
                    {
                        if (onRemoveEntry(node.Key, node.Value))
                        {
                            entries.Remove(node.Key);

                            Node prev = node.Previous;
                            if (prev != null)
                            {
                                prev.Next = node.Next;
                            }

                            if (node.Next != null)
                            {
                                node.Next.Previous = prev;
                            }

                            node.Previous = null;
                            node.Next = null;
                            if (node == tail)
                            {
                                tail = prev;
                            }

                            node = prev;
                        }
                        else
                        {
                            node = node.Previous;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
                else
                {
                    entries.Remove(tail.Key);
                    tail = tail.Previous;
                    if (tail != null) tail.Next = null;
                }
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            value = default(TValue);
            Node entry;
            if (!entries.TryGetValue(key, out entry)) return false;
            MoveToHead(entry);
            value = entry.Value;
            return true;
        }

        public bool ContainsValue(TKey key)
        {
            return entries.ContainsKey(key);
        }

        public List<TValue> GetValues()
        {
            List<TValue> values = new List<TValue>(entries.Count);
            foreach (Node node in entries.Values)
            {
                values.Add(node.Value);
            }

            return values;
        }

#if DEBUG_UIMANAGER
    public void Print ()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        Node ptr = head;
        while(ptr != null)
        {
            sb.Append(ptr.Value.ToString());
            if (ptr.Next != null)
                sb.Append(" > ");
            ptr = ptr.Next;
        }
        Debug.Log("LRU dialogs(auto remove) : " + sb.ToString());
    }
#endif

        public bool Remove(TKey key)
        {
            Node node;
            if (!entries.TryGetValue(key, out node))
                return false;

            entries.Remove(key);

            Node prev = node.Previous;
            Node next = node.Next;
            if (prev != null)
            {
                prev.Next = next;
            }
            else
            {
                head = next;
            }

            if (next != null)
            {
                next.Previous = prev;
            }
            else
            {
                tail = next;
            }

            if (tail == null)
            {
                tail = head;
            }

            node.Previous = null;
            node.Next = null;
            return true;
        }

        public void Clear()
        {
            Node entry = head;
            while (entry != null)
            {
                entry.Value = default(TValue);
                entry = entry.Next;
            }
            entries.Clear();
            DictionaryPool.Free(entries);
            head = tail = null;
        }

        private void MoveToHead(Node entry)
        {
            if (entry == head || entry == null) return;

            var next = entry.Next;
            var previous = entry.Previous;

            if (next != null) next.Previous = entry.Previous;
            if (previous != null) previous.Next = entry.Next;

            entry.Previous = null;
            entry.Next = head;

            if (head != null) head.Previous = entry;
            head = entry;

            if (tail == entry) tail = previous;
        }
    }
}
