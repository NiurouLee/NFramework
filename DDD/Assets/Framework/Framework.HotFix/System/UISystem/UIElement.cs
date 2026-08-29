
using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NFramework.ModuleSystem
{

    [Flags]
    public enum ElementActiveDefault : byte
    {
        Default,
        Active,
        DeActive,
    }

    [Serializable]
    public class UIElement
    {
        [HorizontalGroup("Element")]
        [LabelText("名称")]
        [Required("名称不能为空")]
        [ValidateInput("ValidateName", "名称不能包含特殊字符")]
        public string Name;
        
        [HorizontalGroup("Element")]
        [LabelText("组件")]
        [Required("组件不能为空")]
        [AssetsOnly]
        public Component Component;
        
        [HorizontalGroup("Element")]
        [LabelText("默认状态")]
        [EnumToggleButtons]
        public ElementActiveDefault ActiveDefault;
        
        [ShowInInspector, ReadOnly]
        [LabelText("组件类型")]
        private string ComponentType => Component != null ? Component.GetType().Name : "无";
        
        private bool ValidateName(string name)
        {
            if (string.IsNullOrEmpty(name)) return false;
            return !name.Contains(" ") && !name.Contains("/") && !name.Contains("\\");
        }
    }
}