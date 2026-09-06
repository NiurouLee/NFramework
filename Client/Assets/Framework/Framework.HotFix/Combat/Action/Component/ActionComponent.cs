using System;

using NFramework.ModuleSystem;
namespace NFramework.ModuleSystem.Combat
{
    public class ActionComponent : Entity, IAwakeSystem<Type>
    {
        private Type _actionType;

        public void Awake(Type type)
        {
            _actionType = type;
        }
    }
}