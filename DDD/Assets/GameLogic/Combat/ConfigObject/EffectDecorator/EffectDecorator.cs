using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NFramework.Module.Combat
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class EffectDecorateAttribute : Attribute
    {
        private string _label;
        private int _order;
        public EffectDecorateAttribute(string label, int order)
        {
            _label = label;
            _order = order;
        }

        public string Label => _label;
        public int Order => _order;
    }

    public class EffectDecorator
    {
        [HideInInspector]
        public virtual string Label => "Effect";

        [ToggleGroup("Enabled", "$Label")]
        public bool Enabled;


    }
}