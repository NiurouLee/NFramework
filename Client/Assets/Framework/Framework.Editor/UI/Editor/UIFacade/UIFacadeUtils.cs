using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NFramework.ModuleSystem
{
    /// <summary>
    /// UIFacade 工具类，提供编辑器相关的辅助方法
    /// </summary>
    public static class UIFacadeUtils
    {
#if UNITY_EDITOR
        /// <summary>
        /// 检查UI元素名称是否唯一
        /// </summary>
        /// <param name="target">目标UIFacade对象</param>
        /// <param name="index">当前元素的索引</param>
        /// <param name="name">要检查的名称</param>
        /// <returns>如果名称唯一返回true，否则返回false</returns>
        public static bool CheckName(UnityEngine.Object target, int index, string name)
        {
            UIFacade _uiFacade = (UIFacade)target;
            if (_uiFacade == null || _uiFacade.m_UIElements == null) return true;
            
            for (int i = 0; i < _uiFacade.m_UIElements.Count; i++)
            {
                if (i == index)
                {
                    continue;
                }
                var element = _uiFacade.m_UIElements[i];
                if (element != null && element.Name == name)
                {
                    return false;
                }
            }
            return true;
        }
#endif
    }
}