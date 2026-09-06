using System.Collections.Generic;
using   NFramework;
using NFramework.ModuleSystem;

namespace  NFramework.Core
{
    public interface IRecordSet<T>
    {
        public HashSet<T> Records { get; set; }
    }

    public interface IRecordMap<K, V>
    {
        public Dictionary<K, V> Records { get; set; }
    }


    /// <summary>
    /// 只记录
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract partial class BaseRecordSet<T> : IRecordSet<T>, IAwakeSystem, IDestroySystem
    {
        public HashSet<T> Records { get; set; }

        public void Awake()
        {
            if (this.Records == null)
            {
                this.Records = HashSetPool.Alloc<T>();
            }
            this.OnAwake();
        }

        protected virtual void OnAwake()
        {
        }

        public bool Has(T inT)
        {
            return this.Records.Contains(inT);
        }
        public int Count()
        {
            return this.Records.Count;
        }

        public bool TryGet(T inT, out T outT)
        {
            return this.Records.TryGetValue(inT, out outT);
        }

        public bool TryAdd(T inT)
        {
            if (this.Records.Contains(inT))
            {
                return false;
            }

            this.Records.Add(inT);
            return true;
        }

        public bool TryRemove(T inT)
        {
            if (this.Records.Contains(inT))
            {
                this.Records.Remove(inT);
                return true;
            }

            return false;
        }

        public void Destroy()
        {
            this.OnDestroy();
            if (this.Records != null)
            {
                this.Records.Clear();
                HashSetPool.Free(this.Records);
                this.Records = null;
            }
        }
        protected virtual void OnDestroy()
        {
        }
    }


    public class BaseRecordMap<K, V> : IRecordMap<K, V>, IAwakeSystem, IDestroySystem
    {
        public Dictionary<K, V> Records { get; set; }

        public void Awake()
        {
            if (this.Records == null)
            {
                this.Records = DictionaryPool.Alloc<K, V>();
            }
            this.OnAwake();
        }
        protected virtual void OnAwake()
        {
        }

        public void TryAdd(K inK, V inV)
        {
            this.Records.Add(inK, inV);
        }
        public bool TryRemove(K inK)
        {
            return this.Records.Remove(inK);
        }

        public bool TryGet(K inK, out V outV)
        {
            return this.Records.TryGetValue(inK, out outV);
        }

        public int Count()
        {
            return this.Records.Count;
        }

        public bool Has(K inK)
        {
            return this.Records.ContainsKey(inK);
        }

        public void Destroy()
        {
            this.OnDestroy();
            if (this.Records != null)
            {
                this.Records.Clear();
                DictionaryPool.Free(this.Records);
                this.Records = null;
            }
        }
        protected virtual void OnDestroy()
        {
        }
    }
}