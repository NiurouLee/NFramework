using System.Collections.Generic;
using Framework.ModuleSystem;
using NFramework.Core;
using UnityEngine;

namespace NFramework.ModuleSystem
{
    /// <summary>
    /// 序列化时把编辑器列表里的 Unity 对象引用写入磁盘（预制体 YAML 里为 GUID + fileID），
    /// 反序列化后还原为 <see cref="Object"/> 数组供运行时代码按索引访问（支持任意组件类型）。
    /// </summary>
    public class UIFacade : MonoBehaviour, IUIComponent, ISerializationCallbackReceiver
    {
        public string ID;

        /// <summary>
        /// 与 <c>m_UIElements</c> 顺序一致；Unity 按 Object 引用序列化（资源用 GUID，预制体内用 fileID）。
        /// </summary>
        [SerializeField, HideInInspector] private Object[] m_SerializedIUIComponentObjects;

        [System.NonSerialized] public Object[] Components;

        public BitField16 State = new BitField16(0);
        public bool IsAwake => State.Has(0);
        public bool IsDestroy => State.Has(1);

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (m_UIElements == null || m_UIElements.Count == 0)
            {
                m_SerializedIUIComponentObjects = System.Array.Empty<Object>();
                return;
            }

            var arr = new Object[m_UIElements.Count];
            for (int i = 0; i < m_UIElements.Count; i++)
                arr[i] = m_UIElements[i]?.Component;
            m_SerializedIUIComponentObjects = arr;
#endif
        }

        public void OnAfterDeserialize()
        {
            RebuildComponentsFromSerializedObjects();
        }

        private void OnEnable()
        {
            RebuildComponentsFromSerializedObjects();
        }

        private void RebuildComponentsFromSerializedObjects()
        {
            if (m_SerializedIUIComponentObjects == null || m_SerializedIUIComponentObjects.Length == 0)
            {
                Components = System.Array.Empty<Object>();
                return;
            }

            Components = m_SerializedIUIComponentObjects;
        }

        public T Cast<T>(int index) where T : class
        {
            if (Components == null || Components.Length == 0)
                RebuildComponentsFromSerializedObjects();

            if (Components == null || index < 0 || index >= Components.Length)
            {
                Debug.LogError($"Index out of range: {index}", this.gameObject);
                return default(T);
            }

            return (T)(object)Components[index];
        }


        /// <summary>
        /// 只在编辑器下存储这个信息，运行时没用
        /// </summary>
#if UNITY_EDITOR
        [System.Serializable]
        public class UIElement
        {
            //组件名称，用于生成字段或者方法
            public string Name;

            //组件
            public UnityEngine.Object Component;

            // 描述，用于生成备注
            public string Desc;

            // 是否生成点击事件（仅对可交互组件有意义）
            public bool GenerateClickEvent = true;
        }

        [SerializeField, HideInInspector] public List<UIElement> m_UIElements = new List<UIElement>();

        public void AddUIElement(UIElement inUIElement)
        {
            this.m_UIElements.Add(inUIElement);
        }

        public void RemoveUIElement(UIElement inUIElement)
        {
            this.m_UIElements.Remove(inUIElement);
        }

        public void ClearUIElements()
        {
            this.m_UIElements.Clear();
        }

        // 编辑器配置数据（序列化保存，避免每次打开丢失）
        [SerializeField, HideInInspector] public string m_ModuleName = "";

        [SerializeField, HideInInspector] public string m_SubModuleName = "";

        [SerializeField, HideInInspector] public string m_UIName = "";

        [SerializeField, HideInInspector] public bool m_EnableSubModule = false;

        [SerializeField, HideInInspector] public string m_ScriptName = ""; // 自动生成的脚本名称，用于ViewConfig的ID

#endif

        public void Visible()
        {
            this.gameObject?.SetActive(true);
        }

        public void NotVisible()
        {
            this.gameObject?.SetActive(false);
        }

        public void UIComponentAwake()
        {
            if (IsAwake)
            {
                return;
            }

            State.Learn(0);
            foreach (var component in Components)
            {
                if (component is IUIComponent uiComponent)
                {
                    uiComponent.UIComponentAwake();
                }
            }
        }

        public void UIComponentDestroy()
        {
            if (IsDestroy)
            {
                return;
            }

            State.Learn(1);
            foreach (var component in Components)
            {
                if (component is IUIComponent uiComponent)
                {
                    uiComponent.UIComponentDestroy();
                }
            }
        }

        public IWindowOpenAnim GetOpenAnimComponent()
        {
            return this.gameObject.GetComponent<IWindowOpenAnim>();
        }

        public IwindowCloseAnim GetCloseAnimComponent()
        {
            return this.gameObject.GetComponent<IwindowCloseAnim>();
        }

    }
}