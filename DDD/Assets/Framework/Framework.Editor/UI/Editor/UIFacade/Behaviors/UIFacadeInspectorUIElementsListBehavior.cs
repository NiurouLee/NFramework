using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NFramework.ModuleSystem
{
    /// <summary>
    /// 行为：UI元素列表管理逻辑
    /// </summary>
    public static class UIFacadeInspectorUIElementsListBehavior
    {
        /// <summary>
        /// 添加新的UI元素
        /// </summary>
        public static void AddNewUIElement(UIFacade facade)
        {
            if (facade.m_UIElements == null)
            {
                facade.m_UIElements = new List<UIFacade.UIElement>();
            }

            var newElement = new UIFacade.UIElement();
            facade.m_UIElements.Add(newElement);
            EditorUtility.SetDirty(facade);
        }

        /// <summary>
        /// 删除UI元素
        /// </summary>
        public static void RemoveUIElement(UIFacade facade, int index)
        {
            if (facade.m_UIElements == null || index < 0 || index >= facade.m_UIElements.Count)
                return;

            facade.m_UIElements.RemoveAt(index);
            EditorUtility.SetDirty(facade);
        }

        /// <summary>
        /// 复制 UI 元素：在源元素下方插入一份副本，名称自动生成唯一副本名
        /// </summary>
        public static void DuplicateUIElement(UIFacade facade, int index)
        {
            if (facade.m_UIElements == null || index < 0 || index >= facade.m_UIElements.Count)
                return;

            var source = facade.m_UIElements[index];
            if (source == null)
                return;

            var copy = new UIFacade.UIElement
            {
                Name = GetUniqueCopyName(facade, source.Name),
                Component = source.Component,
                Desc = source.Desc,
                GenerateClickEvent = source.GenerateClickEvent
            };

            facade.m_UIElements.Insert(index + 1, copy);
            EditorUtility.SetDirty(facade);
        }

        private static string GetUniqueCopyName(UIFacade facade, string sourceName)
        {
            string baseName = string.IsNullOrEmpty(sourceName) ? "UIElement" : sourceName;
            string candidate = $"{baseName}_Copy";
            int suffix = 2;
            while (facade.m_UIElements.Any(element => element != null && element.Name == candidate))
            {
                candidate = $"{baseName}_Copy{suffix++}";
            }

            return candidate;
        }

        /// <summary>
        /// 刷新UI元素列表，清理无效元素
        /// </summary>
        public static void RefreshUIElements(UIFacade facade)
        {
            if (facade.m_UIElements == null)
            {
                facade.m_UIElements = new List<UIFacade.UIElement>();
            }

            // 清理无效的元素（组件为空的）
            for (int i = facade.m_UIElements.Count - 1; i >= 0; i--)
            {
                if (facade.m_UIElements[i] == null || facade.m_UIElements[i].Component == null)
                {
                    facade.m_UIElements.RemoveAt(i);
                }
            }

            EditorUtility.SetDirty(facade);
        }

        /// <summary>
        /// 更新UI元素名称
        /// </summary>
        public static bool UpdateUIElementName(UIFacade facade, int index, string newName)
        {
            if (facade.m_UIElements == null || index < 0 || index >= facade.m_UIElements.Count)
                return false;

            if (UIFacadeUtils.CheckName(facade, index, newName))
            {
                facade.m_UIElements[index].Name = newName;
                EditorUtility.SetDirty(facade);
                return true;
            }
            else
            {
                EditorUtility.DisplayDialog("错误", "元素名称重复或无效！", "确定");
                return false;
            }
        }

        /// <summary>
        /// 更新UI元素组件
        /// </summary>
        public static bool UpdateUIElementComponent(UIFacade facade, UIFacade.UIElement element, Component newComponent)
        {
            if (element == null) return false;

            if (newComponent == null)
            {
                element.Component = null;
                if (string.IsNullOrEmpty(element.Name))
                    element.Name = "";
                EditorUtility.SetDirty(facade);
                return true;
            }

            // 走统一的准入规则（当前支持任意组件类型，仅排除 UIFacade）
            if (UIFacadeElementRules.CanAddToElementList(newComponent))
            {
                element.Component = newComponent;
                if (string.IsNullOrEmpty(element.Name))
                    element.Name = newComponent.gameObject.name;
                EditorUtility.SetDirty(facade);
                return true;
            }

            string reason = UIFacadeElementRules.GetInvalidMessage(newComponent);
            EditorUtility.DisplayDialog("错误",
                string.IsNullOrEmpty(reason) ? "组件不符合加入规则！" : $"{reason}！", "确定");
            return false;
        }

        /// <summary>
        /// 更新UI元素描述
        /// </summary>
        public static void UpdateUIElementDesc(UIFacade facade, UIFacade.UIElement element, string newDesc)
        {
            if (element == null) return;

            element.Desc = newDesc;
            EditorUtility.SetDirty(facade);
        }

        /// <summary>
        /// 检查元素是否匹配搜索条件
        /// </summary>
        public static bool MatchSearchFilter(UIFacade.UIElement element, string searchFilter)
        {
            // 如果没有搜索条件，显示所有元素
            if (string.IsNullOrEmpty(searchFilter))
                return true;

            if (element == null)
                return false;

            string filter = searchFilter.ToLower();

            // 搜索元素名称（模糊匹配）
            if (!string.IsNullOrEmpty(element.Name))
            {
                if (element.Name.ToLower().Contains(filter))
                    return true;
            }

            // 搜索组件类型名称（模糊匹配）
            if (element.Component != null)
            {
                string typeName = element.Component.GetType().Name.ToLower();
                if (typeName.Contains(filter))
                    return true;
            }

            return false;
        }
    }
}
