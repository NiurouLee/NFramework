using System;
using System.Collections.Generic;

namespace NFramework.ModuleSystem
{
    /// <summary>
    /// WorldSystem，管理所有 World。
    /// World 是 Entity 的 ROOT，所有 Entity 最终都会挂在一个 World 下。
    /// </summary>
    public class WorldSystem : FrameworkSystemModuleBase
    {
        private readonly Dictionary<Type, World> m_Worlds = new Dictionary<Type, World>();


        /// <summary>获取或创建指定类型的 World（每种类型全局唯一一份）</summary>
        public T GetWorld<T>() where T : World
        {
            Type type = typeof(T);
            if (m_Worlds.TryGetValue(type, out var world))
            {
                return (T)world;
            }

            world = CreateWorld(type);
            return (T)world;
        }

        /// <summary>创建并注册一个 World（已存在则直接返回已有实例）</summary>
        public World CreateWorld(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (m_Worlds.TryGetValue(type, out var exist))
            {
                return exist;
            }

            if (!typeof(World).IsAssignableFrom(type))
            {
                throw new ArgumentException($"type {type} is not a World", nameof(type));
            }

            var world = Entity.Create(type) as World;
            GetSystem<EntitySystem>().Awake(world);
            GetSystem<EntitySystem>().Start(world);

            m_Worlds.Add(type, world);

            return world;
        }

        public T CreateWorld<T>() where T : World
        {
            return (T)CreateWorld(typeof(T));
        }

        public bool HasWorld<T>() where T : World
        {
            return m_Worlds.ContainsKey(typeof(T));
        }

        public bool HasWorld(Type type)
        {
            return m_Worlds.ContainsKey(type);
        }

  

        public bool RemoveWorld<T>() where T : World
        {
            return RemoveWorld(typeof(T));
        }

        public bool RemoveWorld(Type type)
        {
            if (!m_Worlds.TryGetValue(type, out var world))
            {
                return false;
            }

            m_Worlds.Remove(type);
            GetSystem<EntitySystem>().Destroy(world);
            return true;
        }

        public override void Destroy()
        {
            foreach (var world in m_Worlds.Values)
            {
                GetSystem<EntitySystem>().Destroy(world);
            }

            m_Worlds.Clear();
        }
    }
}
