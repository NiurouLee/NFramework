
using NFramework.Core.Live;
using NFramework.Module.EntityModule;
using NFramework.Module.EventModule;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace NFramework.Module.Combat
{
    public class ActionPoint
    {
        private List<Action<Entity>> _listenerList = new List<Action<Entity>>();

        public void AddListener(Action<Entity> listener)
        {
            _listenerList.Add(listener);
        }

        public void RemoveListener(Action<Entity> listener)
        {
            _listenerList.Remove(listener);
        }
        public void TriggerActionPoint(Entity inActionExecution)
        {
            if (inActionExecution == null)
            {
                return;
            }
            if (_listenerList.Count == 0)
            {
                return;
            }

            for (int i = _listenerList.Count - 1; i >= 0; i--)
            {
                _listenerList[i](inActionExecution);
            }
        }
    }

    public class ActionPointComponent : Entity
    {
        private Dictionary<ActionPointType, ActionPoint> _actionPointDict = new Dictionary<ActionPointType, ActionPoint>();

        public void AddListener(ActionPointType actionPointType, Action<Entity> action)
        {
            if (!_actionPointDict.ContainsKey(actionPointType))
            {
                _actionPointDict.Add(actionPointType, new ActionPoint());
            }
            _actionPointDict[actionPointType].AddListener(action);
        }

        public void RemoveListener(ActionPointType actionPointType, Action<Entity> action)
        {
            if (_actionPointDict.ContainsKey(actionPointType))
            {
                _actionPointDict[actionPointType].RemoveListener(action);
            }
        }

        public ActionPoint GetActionPoint(ActionPointType actionPointType)
        {
            if (_actionPointDict.TryGetValue(actionPointType, out var actionPoint))
            {
                return actionPoint;
            }
            return null;
        }

        public void TriggerActionPoint(ActionPointType actionPointType, Entity actionExecution)
        {
            if (_actionPointDict.TryGetValue(actionPointType, out var actionPoint))
            {
                actionPoint.TriggerActionPoint(actionExecution);
            }
        }
    }

}