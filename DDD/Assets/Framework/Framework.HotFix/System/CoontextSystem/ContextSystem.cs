using System;
using System.Collections.Generic;

namespace NFramework.ModuleSystem
{
    /// <summary>
    /// ContextSystem，统一管理全局上下文 Context。
    /// 每种 Context 类型全局唯一一份，按需创建。
    /// </summary>
    public class ContextSystem : FrameworkSystemModuleBase
    {
        private readonly Dictionary<Type, Context> m_Contexts = new Dictionary<Type, Context>();

        /// <summary>当前激活的 Context（可为 null）</summary>
        public Context CurrentContext { get; private set; }

        /// <summary>获取或创建指定类型的 Context（每种类型全局唯一一份）</summary>
        public T GetContext<T>() where T : Context
        {
            Type type = typeof(T);
            if (m_Contexts.TryGetValue(type, out var context))
            {
                return (T)context;
            }

            context = CreateContext(type);
            return (T)context;
        }

        /// <summary>创建并注册一个 Context（已存在则直接返回已有实例）</summary>
        public Context CreateContext(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (m_Contexts.TryGetValue(type, out var exist))
            {
                return exist;
            }

            if (!typeof(Context).IsAssignableFrom(type))
            {
                throw new ArgumentException($"type {type} is not a Context", nameof(type));
            }

            var context = Activator.CreateInstance(type) as Context;
            context.Awake();

            m_Contexts.Add(type, context);
            if (CurrentContext == null)
            {
                CurrentContext = context;
            }

            return context;
        }

        public T CreateContext<T>() where T : Context
        {
            return (T)CreateContext(typeof(T));
        }

        public bool HasContext<T>() where T : Context
        {
            return m_Contexts.ContainsKey(typeof(T));
        }

        public bool HasContext(Type type)
        {
            return m_Contexts.ContainsKey(type);
        }

        public void SetCurrentContext(Context context)
        {
            CurrentContext = context;
        }

        public bool RemoveContext<T>() where T : Context
        {
            return RemoveContext(typeof(T));
        }

        public bool RemoveContext(Type type)
        {
            if (!m_Contexts.TryGetValue(type, out var context))
            {
                return false;
            }

            m_Contexts.Remove(type);
            if (ReferenceEquals(CurrentContext, context))
            {
                CurrentContext = null;
            }

            context.Destroy();
            return true;
        }

        public override void Destroy()
        {
            foreach (var context in m_Contexts.Values)
            {
                context.Destroy();
            }

            m_Contexts.Clear();
            CurrentContext = null;
        }
    }
}
