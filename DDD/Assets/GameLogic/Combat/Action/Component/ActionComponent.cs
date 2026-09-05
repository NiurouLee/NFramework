using System;
using NFramework.Core.Live;
using NFramework.Module.EntityModule;
namespace NFramework.Module.Combat
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