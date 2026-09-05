using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NFramework.Core.Live;
using NFramework.Module.EntityModule;

namespace NFramework.Module.Combat
{
    public partial class StatusAbility : Entity, IAbility, IAwakeSystem<StatusConfigObject>, IAwakeSystem<System.Object>
    {
        public bool Enable { get; set; }
        public CombatEntity Owner => GetParent<CombatEntity>();
        public CombatEntity Creator;

        public StatusConfigObject StatusConfigObject;
        public Dictionary<string, string> paramsDict;
        public bool isChildStatus;
        public int duration;
        public ChildStatus childStatusData;
        public List<StatusAbility> _statusList = new List<StatusAbility>();

        public void Awake(object obj)
        {
            this.StatusConfigObject = obj as StatusConfigObject;
            this.Awake(StatusConfigObject);
        }

        public void Awake(StatusConfigObject statusConfigObject)
        {
            this.StatusConfigObject = statusConfigObject;
            if (statusConfigObject.EffectList.Count > 0)
            {
                AddComponent<AbilityEffectComponent, List<Effect>>(statusConfigObject.EffectList);
            }
        }

        public void SetParams(Dictionary<string, string> paramsDict)
        {
            this.paramsDict = (Dictionary<string, string>)Clone(paramsDict);
            this.paramsDict.Add("自身生命值", Owner.GetComponent<AttributeComponent>().HealthPoint.Value.ToString());
            this.paramsDict.Add("自身攻击力", Owner.GetComponent<AttributeComponent>().Attack.Value.ToString());
        }

        public object Clone(object obj)
        {
            MemoryStream memoryStream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(memoryStream, obj);
            memoryStream.Position = 0;
            return formatter.Deserialize(memoryStream);
        }
        public void ActivateAbility()
        {
            Enable = true;
            GetComponent<AbilityEffectComponent>().EnableEffect();
            if (StatusConfigObject.EnableChildStatus)
            {
                foreach (var childStatus in StatusConfigObject.StatusList)
                {
                    var status = Owner.AttachStatus(childStatus.StatusConfigObject.Id);
                    status.Creator = Creator;
                    status.isChildStatus = true;
                    status.childStatusData = childStatus;
                    status.SetParams(childStatusData.ParamsDict);
                    status.ActivateAbility();
                    _statusList.Add(status);
                }
            }
        }

        public void EndAbility()
        {
            Enable = false;
            if (StatusConfigObject.EnableChildStatus)
            {
                foreach (var status in _statusList)
                {
                    status.EndAbility();
                }
                _statusList.Clear();
            }
            foreach (var effect in StatusConfigObject.EffectList)
            {
                if (!effect.Enabled)
                {
                    continue;
                }
            }
            Owner.OnStatueRemove(this);
            Dispose();
        }

        public Entity CreateExecution()
        {
            return null;
        }

    }
}