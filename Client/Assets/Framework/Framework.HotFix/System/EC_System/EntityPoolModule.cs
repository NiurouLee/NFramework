using System;

namespace NFramework.ModuleSystem
{
    public class EntityPoolSystem : FrameworkSystemModuleBase
    {
        public T Fetch<T>() where T : Entity, new()
        {
            return new T();
        }

        public Entity Fetch(Type type)
        {
            return Activator.CreateInstance(type) as Entity;
        }

        public void Recycle(Entity entity)
        {
        }
    }
}