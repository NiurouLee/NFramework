using System;
using NFramework.Core;

namespace NFramework.ModuleSystem
{
    /// <summary>
    /// ViewComponentRecords，负责管理 View 上的组件，专门负责代理 View 处理各种组件
    /// </summary>
    public class ViewComponentRecords : IAwakeSystem<View>, IDestroySystem, IFreeToPool
    {
        public View View { get; private set; }

        public UnOrderMultiMapLink<Type, ViewComponent> Records { get; private set; }

        public void Awake(View a)
        {
            this.View = a;
            if (this.Records == null)
            {
                this.Records = new UnOrderMultiMapLink<Type, ViewComponent>();
            }
        }

        /// <summary>
        /// 按类型查找组件：传入名称时精确匹配，不传名称时返回该类型的第一个组件
        /// </summary>
        public T Get<T>(string inName = null) where T : ViewComponent
        {
            if (this.Records == null)
            {
                return null;
            }

            if (this.Records.TryGetValue(typeof(T), out var range))
            {
                var current = range.First;
                while (current != null && current != range.Terminal)
                {
                    if (string.IsNullOrEmpty(inName) || current.Value.Name == inName)
                    {
                        return current.Value as T;
                    }

                    current = current.Next;
                }
            }

            return null;
        }

        /// <summary>
        /// 获取或创建组件：已存在同类型（同名）则复用，否则从对象池分配并挂到 View 上
        /// </summary>
        public T Add<T>(string inName = null) where T : ViewComponent, new()
        {
            var component = this.Get<T>(inName);
            if (component != null)
            {
                return component;
            }

            component = this.View.GetSystem<ObjectPoolSystem>().Alloc<T>();
            component.Awake(this.View, inName);
            this.Records.Add(typeof(T), component);
            return component;
        }

        /// <summary>
        /// 注册一个已创建的组件（同类型同实例已存在时返回 false）
        /// </summary>
        public bool TryAdd(Type inType, ViewComponent inComponent)
        {
            if (inType == null || inComponent == null || this.Records == null)
            {
                return false;
            }

            if (this.Records.Contains(inType, inComponent))
            {
                return false;
            }

            this.Records.Add(inType, inComponent);
            return true;
        }

        public bool Has<T>() where T : ViewComponent
        {
            return this.Records != null && this.Records.Contains(typeof(T));
        }

        public bool TryRemove<T>(T inComponent) where T : ViewComponent
        {
            if (inComponent == null || this.Records == null)
            {
                return false;
            }

            return this.Records.TryRemove(typeof(T), inComponent, out _);
        }

        public void Destroy()
        {
            if (this.Records == null)
            {
                return;
            }

            var pool = this.View != null ? this.View.GetSystem<ObjectPoolSystem>() : null;
            foreach (var pair in this.Records)
            {
                var current = pair.Value.First;
                while (current != null && current != pair.Value.Terminal)
                {
                    var component = current.Value;
                    component.Destroy();
                    pool?.Free(component);
                    current = current.Next;
                }
            }

            this.Records.Clear();
            this.Records = null;
            this.View = null;
        }

        public void FreeToPool()
        {
            // 组件已由 Destroy 释放；这里只做对象池复用前的引用复位
            this.View = null;
            this.Records = null;
        }
    }
}
