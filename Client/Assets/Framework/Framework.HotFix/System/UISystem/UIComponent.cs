using System;
using System.Collections.Generic;
using NFramework.Core;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif
using UnityEngine;

namespace NFramework.ModuleSystem
{
    /// <summary>
    /// 编辑器配置用的 UIComponent（仅在编辑器中使用）
    /// 运行时会被转换为轻量级数据结构以减少内存占用
    /// </summary>
    [Serializable]
    public class UIComponent
    {
#if UNITY_EDITOR
        [HideInInspector]
        public bool IsExpanded = true; // 添加折叠状态

        [HorizontalGroup("Element")]
        [LabelText("名称")]
        [Required("名称不能为空")]
        [ValidateInput("ValidateName", "名称不能包含特殊字符")]
#endif
        public string Name;
        
#if UNITY_EDITOR
        [HorizontalGroup("Element")]
        [LabelText("组件")]
        [Required("组件不能为空")]
        [AssetsOnly]
#endif
        public Component Component;
        
#if UNITY_EDITOR
        [HorizontalGroup("Element")]
        [LabelText("默认状态")]
        [EnumToggleButtons]
#endif
        public ElementActiveDefault ActiveDefault;
        
        // 存储选中的 Input 类型（用于自动生成 BindInput，仅在编辑器中使用）
#if UNITY_EDITOR
        [HideInInspector]
#endif
        public List<PointerEventType> SelectedInputTypes = new List<PointerEventType>();
        
#if UNITY_EDITOR
        [ShowInInspector, ReadOnly]
        [LabelText("组件类型")]
        private string ComponentType => Component != null ? Component.GetType().Name : "无";
#endif
        
        private bool ValidateName(string name)
        {
            if (string.IsNullOrEmpty(name)) return false;
            return !name.Contains(" ") && !name.Contains("/") && !name.Contains("\\");
        }
    }
}

