using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NFramework.ModuleSystem
{
    /// <summary>
    /// 行为：UI元素收集和管理逻辑
    /// </summary>
    public static class UIFacadeInspectorUIElementBehavior
    {
        /// <summary>
        /// 获取可选的 UI 组件候选：GameObject 上满足准入规则的组件（当前为任意组件，排除 UIFacade），
        /// includeChildren 为 true 时包含其子对象
        /// </summary>
        public static Component[] GetUIComponentCandidates(GameObject gameObject, bool includeChildren = true)
        {
            if (gameObject == null)
            {
                return System.Array.Empty<Component>();
            }

            Component[] components = includeChildren
                ? gameObject.GetComponentsInChildren<Component>(true)
                : gameObject.GetComponents<Component>();

            return components
                .Where(c => UIFacadeElementRules.CanAddToElementList(c))
                .ToArray();
        }

        /// <summary>
        /// 获取可选的 UI 组件候选：UIFacade 自身及其子对象上满足准入规则的组件
        /// </summary>
        public static Component[] GetUIComponentCandidates(UIFacade facade)
        {
            return GetUIComponentCandidates(facade != null ? facade.gameObject : null);
        }

        /// <summary>
        /// 将一组组件批量加入 UI 元素列表（已存在的跳过），返回实际新增数量
        /// </summary>
        public static int AddUIComponentCandidates(UIFacade facade, IEnumerable<Component> components)
        {
            if (facade == null || components == null)
            {
                return 0;
            }

            if (facade.m_UIElements == null)
            {
                facade.m_UIElements = new List<UIFacade.UIElement>();
            }

            int addedCount = 0;
            foreach (var component in components)
            {
                if (component == null) continue;

                bool exists = facade.m_UIElements.Any(element => element != null && element.Component == (UnityEngine.Object)component);
                if (exists) continue;

                facade.m_UIElements.Add(new UIFacade.UIElement
                {
                    Name = component.gameObject.name,
                    Component = component,
                    Desc = ""
                });
                addedCount++;
            }

            if (addedCount > 0)
            {
                EditorUtility.SetDirty(facade);
            }

            return addedCount;
        }

        /// <summary>
        /// 自动收集子对象中的UI组件
        /// </summary>
        public static void AutoCollectChildComponents(UIFacade facade)
        {
            if (facade == null || facade.gameObject == null) return;

            // 收集所有子对象中满足准入规则的组件
            int addedCount = AddUIComponentCandidates(facade, GetUIComponentCandidates(facade));

            EditorUtility.SetDirty(facade);

            if (addedCount > 0)
            {
                Debug.Log($"[UIFacade] 已自动收集 {addedCount} 个UI组件");
            }
            else
            {
                Debug.Log("[UIFacade] 没有找到新的UI组件");
            }
        }

        /// <summary>
        /// 清空所有UI元素
        /// </summary>
        public static void ClearUIElements(UIFacade facade)
        {
            if (EditorUtility.DisplayDialog("确认", "确定要清空所有UI元素吗？", "确定", "取消"))
            {
                if (facade.m_UIElements != null)
                {
                    facade.m_UIElements.Clear();
                }
                EditorUtility.SetDirty(facade);
            }
        }

        /// <summary>
        /// 添加单个UI元素
        /// </summary>
        public static void AddUIElement(UIFacade facade, UIFacade.UIElement element)
        {
            if (facade.m_UIElements == null)
            {
                facade.m_UIElements = new List<UIFacade.UIElement>();
            }

            facade.m_UIElements.Add(element);
            EditorUtility.SetDirty(facade);
        }

        /// <summary>
        /// 移除UI元素
        /// </summary>
        public static void RemoveUIElement(UIFacade facade, UIFacade.UIElement element)
        {
            if (facade.m_UIElements != null)
            {
                facade.m_UIElements.Remove(element);
                EditorUtility.SetDirty(facade);
            }
        }

        /// <summary>
        /// 验证UI元素配置
        /// </summary>
        public static bool ValidateUIElements(UIFacade facade, out string errorMessage)
        {
            errorMessage = "";

            if (facade.m_UIElements == null || facade.m_UIElements.Count == 0)
            {
                errorMessage = "没有配置任何UI元素";
                return false;
            }

            var duplicateNames = facade.m_UIElements
                .Where(e => e != null && !string.IsNullOrEmpty(e.Name))
                .GroupBy(e => e.Name)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateNames.Any())
            {
                errorMessage = $"发现重复的元素名称: {string.Join(", ", duplicateNames)}";
                return false;
            }

            var nullComponents = facade.m_UIElements
                .Where(e => e != null && e.Component == null)
                .ToList();

            if (nullComponents.Any())
            {
                errorMessage = $"发现 {nullComponents.Count} 个空组件引用";
                return false;
            }

            return true;
        }

        /// <summary>
        /// 验证完整配置（包括UI元素和其他配置）
        /// </summary>
        public static void ValidateConfiguration(UIFacade facade)
        {
            List<string> issues = new List<string>();

            // 验证基本配置
            if (string.IsNullOrEmpty(facade.m_ModuleName))
                issues.Add("模块名称不能为空");

            if (string.IsNullOrEmpty(facade.m_UIName))
                issues.Add("UI名称不能为空");

            if (string.IsNullOrEmpty(facade.m_ScriptName))
                issues.Add("脚本名称不能为空");

            // 验证UI元素
            if (!ValidateUIElements(facade, out string uiElementError))
            {
                issues.Add(uiElementError);
            }

            if (issues.Any())
            {
                EditorUtility.DisplayDialog("配置验证",
                    $"发现以下问题:\n{string.Join("\n", issues)}", "确定");
            }
            else
            {
                Debug.Log("[UIFacade] 配置验证通过");
            }
        }
    }
}
