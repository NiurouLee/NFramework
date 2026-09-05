
using System.Collections.Generic;
using NFramework.Core.Live;
using NFramework.Module.EntityModule;

namespace NFramework.Module.Combat
{
    public partial class ItemAbility : Entity, IAbility, IAwakeSystem<System.Object>
    {
        public CombatEntity Owner => GetParent<CombatEntity>();
        public ItemConfigObject itemConfigObject;
        private List<StatusAbility> _statusList = new List<StatusAbility>();
        public void Awake(object a)
        {
            itemConfigObject = a as ItemConfigObject;
            AddComponent<AbilityEffectComponent, List<Effect>>(itemConfigObject.EffectList);
        }
        public void ActivateAbility()
        {
            this.Enable = true;
            if (itemConfigObject.EnableChildStatus)
            {
                foreach (var item in itemConfigObject.StatusList)
                {
                    var status = Owner.AttachStatus(item.StatusConfigObject.Id);
                    status.Creator = Owner;
                    status.isChildStatus = true;
                    status.childStatusData = item;
                    status.SetParams(item.ParamsDict);
                    status.ActivateAbility();
                    _statusList.Add(status);
                }
            }
        }

        public void EndAbility()
        {
            Enable = false;
            if (itemConfigObject.EnableChildStatus)
            {
                foreach (var item in _statusList)
                {
                    item.EndAbility();
                }
                _statusList.Clear();
            }
            Dispose();
        }

        public Entity CreateExecution()
        {
            return null;
        }

    }
}