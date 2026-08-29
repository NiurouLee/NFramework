using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace OM.AC.Editor
{
    public class TimelineShortcutsManipulator : Manipulator
    {
        private readonly Dictionary<KeyCode,Action> _actions;

        public TimelineShortcutsManipulator(Dictionary<KeyCode, Action> actions)
        {
            _actions = actions;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            this.target.RegisterCallback<KeyDownEvent>(this.OnKeyDown);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            this.target.UnregisterCallback<KeyDownEvent>(this.OnKeyDown);
        }
        
        private void OnKeyDown(KeyDownEvent evt)
        {
            foreach (var pair in _actions)
            {
                if (pair.Key == evt.keyCode)
                {
                    pair.Value.Invoke();
                }
            }
        }
    }
}