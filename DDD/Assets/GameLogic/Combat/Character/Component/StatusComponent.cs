using System.Collections.Generic;
using NFramework.Core.Collections;
using NFramework.Module.EntityModule;
using NFramework.Module.ResModule;

namespace NFramework.Module.Combat
{
    public class StatusComponent : Entity
    {
        public CombatEntity Combat => GetParent<CombatEntity>();
        public List<StatusAbility> statusList = new List<StatusAbility>();

        public UnOrderMultiMapVector<int, StatusAbility> statusDict = new UnOrderMultiMapVector<int, StatusAbility>();

        public StatusAbility AttachStatus(int StatusId)
        {
            StatusConfigObject statusConfigObject = null;//= NFROOT.Instance.GetModule<ResM>().Load<StatusConfigObject>(string.Empty);
            if (statusConfigObject == null)
            {
                return null;
            }

            var status = Combat.AttachAbility<StatusAbility>(statusConfigObject);
            if (!statusDict.ContainsKey(status.StatusConfigObject.Id))
            {
                statusDict.Add(status.StatusConfigObject.Id, new List<StatusAbility>());

            }
            statusDict[status.StatusConfigObject.Id].Add(status);
            statusList.Add(status);
            return status;
        }

        public StatusAbility GetStatus(int statusID, int index = 0)
        {
            if (HasStatus(statusID))
            {
                return statusDict[statusID][index];
            }
            return null;
        }

        public void OnStatusRemove(StatusAbility inStatusAbility)
        {
            statusDict[inStatusAbility.StatusConfigObject.Id].Remove(inStatusAbility);
            statusList.Remove(inStatusAbility);
        }
        public bool HasStatus(int statusID)
        {
            return statusDict.ContainsKey(statusID);
        }

        public void OnStatuesChanged(StatusAbility statusAbility)
        {
            var tempActionControl = ActionControlType.None;
            foreach (var item in Combat.Children.Values)
            {
                if (item is StatusAbility status)
                {
                    if (!status.Enable)
                    {
                        continue;
                    }
                    foreach (var effect in status.GetComponent<AbilityEffectComponent>().AbilityEffectList)
                    {
                        var actionControlComponent = effect.GetComponent<AbilityEffectActionControlComponent>();
                        if (actionControlComponent != null)
                        {
                            tempActionControl = tempActionControl | actionControlComponent.ActionControlEffect.ActionControlType;
                        }
                    }
                }
            }
            Combat.ActionControlType = tempActionControl;
            var moveForbid = Combat.ActionControlType.HasFlag(ActionControlType.MoveForbid);
            Combat.GetComponent<MotionComponent>().SetEnable(!moveForbid);
        }
    }
}