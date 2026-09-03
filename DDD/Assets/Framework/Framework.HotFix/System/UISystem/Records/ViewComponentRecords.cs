using System;
using System.Collections.Generic;

namespace NFramework.ModuleSystem
{
    /// <summary>
    /// ViewComponentRecords，负责管理 View 上的组件，专门负责代理 View 处理各种组件。
    /// 存储结构为 Type -> Name -> Component：同一类型可以通过不同 Name 保存多个组件实例，
    /// 名字为 null/空串 时作为该类型的默认（未命名）组件，默认组件同类型仍只保留一个。
    /// </summary>
    public class ViewComponentRecords : IAwakeSystem<View>, IDestroySystem, IFreeToPool
    {
        public View View { get; private set; }

        /// <summary>Type -> (Name -> Component)</summary>
        private Dictionary<Type, Dictionary<string, ViewComponent>> m_Records;

        public void Awake(View inView)
        {
            this.View = inView;
            if (this.m_Records == null)
            {
                this.m_Records = new Dictionary<Type, Dictionary<string, ViewComponent>>();
            }
        }

        private static string NormalizeName(string inName)
        {
            return inName ?? string.Empty;
        }

        private Dictionary<string, ViewComponent> GetBucket(Type inType)
        {
            if (this.m_Records == null || inType == null)
            {
                return null;
            }

            return this.m_Records.TryGetValue(inType, out var bucket) ? bucket : null;
        }

        private Dictionary<string, ViewComponent> GetOrCreateBucket(Type inType)
        {
            if (this.m_Records == null)
            {
                this.m_Records = new Dictionary<Type, Dictionary<string, ViewComponent>>();
            }

            if (!this.m_Records.TryGetValue(inType, out var bucket))
            {
                bucket = new Dictionary<string, ViewComponent>();
                this.m_Records.Add(inType, bucket);
            }

            return bucket;
        }

        private T GetExact<T>(string inName) where T : ViewComponent
        {
            var bucket = this.GetBucket(typeof(T));
            if (bucket == null)
            {
                return null;
            }

            var key = NormalizeName(inName);
            return bucket.TryGetValue(key, out var component) && component is T t ? t : null;
        }

        /// <summary>
        /// 按类型+名字获取组件。
        /// 不传名字时先取默认（未命名）组件；没有默认组件时取该类型的第一个组件，兼容旧的“每类型单组件”用法。
        /// </summary>
        public T Get<T>(string inName = null) where T : ViewComponent
        {
            var bucket = this.GetBucket(typeof(T));
            if (bucket == null || bucket.Count == 0)
            {
                return null;
            }

            var key = NormalizeName(inName);
            if (bucket.TryGetValue(key, out var component) && component is T exact)
            {
                return exact;
            }

            // 旧接口 Get<T>() 不传名字时返回同类型第一个组件，保留这个兼容行为
            if (string.IsNullOrEmpty(inName))
            {
                foreach (var kv in bucket)
                {
                    if (kv.Value is T first)
                    {
                        return first;
                    }
                }
            }

            return null;
        }

        /// <summary>获取同类型下全部组件（不同 Name 的实例）</summary>
        public T[] GetAll<T>() where T : ViewComponent
        {
            var bucket = this.GetBucket(typeof(T));
            if (bucket == null || bucket.Count == 0)
            {
                return Array.Empty<T>();
            }

            var list = new List<T>(bucket.Count);
            foreach (var component in bucket.Values)
            {
                if (component is T t)
                {
                    list.Add(t);
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// 获取或创建组件：同类型同 Name 已存在则复用，否则从对象池分配并挂到 View 上。
        /// </summary>
        public T Add<T>(string inName = null) where T : ViewComponent, new()
        {
            var existing = this.GetExact<T>(inName);
            if (existing != null)
            {
                return existing;
            }

            var bucket = this.GetOrCreateBucket(typeof(T));
            var component = this.View.GetSystem<ObjectPoolSystem>().Alloc<T>();
            component.Awake(this.View, inName);
            bucket[NormalizeName(inName)] = component;
            return component;
        }

        /// <summary>
        /// 注册一个外部创建/传入的组件，并把它挂到当前 View 上。
        /// 同类型同 Name 已存在时抛异常；组件尚未 Awake 时由这里负责 Awake。
        /// </summary>
        public T Add<T>(T inComponent, string inName = null) where T : ViewComponent
        {
            if (inComponent == null)
            {
                throw new ArgumentNullException(nameof(inComponent));
            }

            var bucket = this.GetOrCreateBucket(typeof(T));
            var key = NormalizeName(inName);
            if (bucket.TryGetValue(key, out var existing))
            {
                if (ReferenceEquals(existing, inComponent))
                {
                    return inComponent;
                }

                throw new Exception(
                    $"ViewComponentRecords Add duplicate, Type:{typeof(T).Name}, Name:{inName}");
            }

            if (inComponent.View != null)
            {
                if (inComponent.View != this.View)
                {
                    throw new Exception(
                        $"ViewComponentRecords Add component already on another View, Type:{typeof(T).Name}, Name:{inName}");
                }

                if (inComponent.Name != key)
                {
                    throw new Exception(
                        $"ViewComponentRecords Add component already awake with Name:{inComponent.Name}, can not rename to:{inName}");
                }
            }
            else
            {
                inComponent.Awake(this.View, inName);
            }

            bucket[key] = inComponent;
            return inComponent;
        }

        /// <summary>Add 的不抛异常版本</summary>
        public bool TryAdd<T>(T inComponent, string inName = null) where T : ViewComponent
        {
            if (inComponent == null)
            {
                return false;
            }

            var bucket = this.GetOrCreateBucket(typeof(T));
            var key = NormalizeName(inName);
            if (bucket.TryGetValue(key, out var existing))
            {
                return ReferenceEquals(existing, inComponent);
            }

            if (inComponent.View != null && inComponent.View != this.View)
            {
                return false;
            }

            if (inComponent.View != null && inComponent.Name != key)
            {
                return false;
            }

            if (inComponent.View == null)
            {
                inComponent.Awake(this.View, inName);
            }

            bucket[key] = inComponent;
            return true;
        }

        /// <summary>
        /// 检查是否存在某类型组件。
        /// 不传名字只判断类型下是否有组件；传名字判断该名字是否存在。
        /// </summary>
        public bool Has<T>(string inName = null) where T : ViewComponent
        {
            var bucket = this.GetBucket(typeof(T));
            if (bucket == null || bucket.Count == 0)
            {
                return false;
            }

            return string.IsNullOrEmpty(inName) || bucket.ContainsKey(NormalizeName(inName));
        }

        /// <summary>移除指定名字的组件（仅从记录中移除，不调用 Destroy）</summary>
        public bool TryRemove<T>(string inName) where T : ViewComponent
        {
            var bucket = this.GetBucket(typeof(T));
            return bucket != null && bucket.Remove(NormalizeName(inName));
        }

        /// <summary>移除指定组件实例（仅从记录中移除，不调用 Destroy）</summary>
        public bool TryRemove<T>(T inComponent) where T : ViewComponent
        {
            if (inComponent == null)
            {
                return false;
            }

            var bucket = this.GetBucket(typeof(T));
            if (bucket == null)
            {
                return false;
            }

            foreach (var kv in bucket)
            {
                if (ReferenceEquals(kv.Value, inComponent))
                {
                    return bucket.Remove(kv.Key);
                }
            }

            return false;
        }

        /// <summary>移除某类型下的全部组件</summary>
        public bool RemoveAll<T>() where T : ViewComponent
        {
            return this.m_Records != null && this.m_Records.Remove(typeof(T));
        }

        public void Destroy()
        {
            if (this.m_Records == null)
            {
                return;
            }

            var pool = this.View != null ? this.View.GetSystem<ObjectPoolSystem>() : null;
            foreach (var bucket in this.m_Records.Values)
            {
                foreach (var component in bucket.Values)
                {
                    component.Destroy();
                    pool?.Free(component);
                }
            }

            this.m_Records.Clear();
            this.m_Records = null;
            this.View = null;
        }

        public void FreeToPool()
        {
            // 组件已由 Destroy 释放；这里只做对象池复用前的引用复位
            this.View = null;
            this.m_Records = null;
        }
    }
}
