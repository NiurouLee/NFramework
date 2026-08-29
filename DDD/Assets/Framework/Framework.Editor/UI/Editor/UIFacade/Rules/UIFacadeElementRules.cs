using UnityEngine;

namespace NFramework.ModuleSystem
{
    /// <summary>
    /// UIFacade 元素列表的准入规则：统一控制哪些组件可以加入/显示在 UI 元素列表中。
    /// 目前先支持任意组件类型，仅排除 UIFacade 自身；后续要加类型白名单、接口限制时改这里即可。
    /// </summary>
    public static class UIFacadeElementRules
    {
        /// <summary>判断组件是否允许加入 UI 元素列表</summary>
        public static bool CanAddToElementList(Component component)
        {
            if (component == null)
            {
                return false;
            }

            // 排除 UIFacade 自身，避免把门面组件自己加进列表
            return !(component is UIFacade);
        }

        /// <summary>获取不能加入时的原因（可为空字符串）</summary>
        public static string GetInvalidMessage(Component component)
        {
            if (component == null)
            {
                return "组件为空";
            }

            if (component is UIFacade)
            {
                return "不能添加 UIFacade 组件自身";
            }

            return "";
        }
    }
}
