using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace NFramework.ModuleSystem
{
    /// <summary>
    /// 扩展的Toggle组件，提供更多自定义功能和事件处理
    /// </summary>
    [AddComponentMenu("UI/UIToggle", 56)]
    public class UIToggle : Toggle, IUIInputComponent, IUISelectInputTrigger<UIToggle>
    {
        [Header("选中状态")]
        public GameObject ActiveNode;

        [Header("未选中状态")]
        public GameObject InactiveNode;

        [Header("文本设置")]
        [Tooltip("Toggle右侧的文本组件")]
        public TextMeshPro labelText;

        [Tooltip("文本内容")]
        public string text = "Toggle";

        [Tooltip("文本颜色")]
        public Color textColor = new Color(50f / 255f, 50f / 255f, 50f / 255f, 1f);

        [Header("状态资源切换")]
        [Tooltip("是否使用状态资源切换")]
        public bool useStateResources = false;

        [Tooltip("目标图片组件，用于切换Sprite")]
        public Image targetImage;

        [Tooltip("选中状态的Sprite")]
        public Sprite activeSprite;

        [Tooltip("未选中状态的Sprite")]
        public Sprite inactiveSprite;

        [Tooltip("是否切换颜色")]
        public bool useColorTransition = false;

        [Tooltip("选中状态的颜色")]
        public Color activeColor = new Color(0.2f, 0.7f, 0.2f, 1f);

        [Tooltip("未选中状态的颜色")]
        public Color inactiveColor = new Color(0.7f, 0.2f, 0.2f, 1f);

        [Header("扩展的绑定数据")]
        public string data;

        [Header("音效设置")]
        public bool playSoundOnClick = false;
        public string soundName = "toggle_click";

        /// <summary>
        /// 扩展的值变化事件，包含状态和绑定数据
        /// </summary>
        public Action<bool, string> onValueChangeWithData;

        /// <summary>
        /// 点击事件，无论状态如何都会触发
        /// </summary>
        public Action<UIToggle> onClick;

        /// <summary>
        /// 拦截点击事件，返回是否可以点击
        /// </summary>
        protected Func<bool> onCanClick;

        /// <summary>
        /// 点击失败事件
        /// </summary>
        protected Action onCanClickFailed;

        // 内部变量
        [NonSerialized]// 非序列化
        private bool _initialized = false;

        public event Action<UIToggle, bool> OnSelectEvent;

        public bool IsSelected { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            // 自动查找子节点
            if (this.transform.childCount > 0)
            {
                for (int i = 0; i < this.transform.childCount; i++)
                {
                    if (this.transform.GetChild(i).name.IndexOf("node_active", StringComparison.Ordinal) != -1)
                    {
                        ActiveNode = this.transform.GetChild(i).gameObject;
                    }

                    if (this.transform.GetChild(i).name.IndexOf("node_inactive", StringComparison.Ordinal) != -1)
                    {
                        InactiveNode = this.transform.GetChild(i).gameObject;
                    }

                    if (this.transform.GetChild(i).name.IndexOf("label", StringComparison.Ordinal) != -1)
                    {
                        TextMeshPro tmpText = this.transform.GetChild(i).GetComponent<TextMeshPro>();
                        if (tmpText != null)
                        {
                            labelText = tmpText;

                            // 更新文本内容和颜色
                            if (labelText != null)
                            {
                                labelText.text = text;
                                labelText.color = textColor;
                            }
                        }
                    }
                }
            }

            // 如果没有指定targetImage，尝试获取当前对象上的Image组件
            if (targetImage == null)
            {
                targetImage = GetComponent<Image>();
            }
        }
#endif

        protected override void Awake()
        {
            base.Awake();
            Initialize();
        }

        protected override void Start()
        {
            base.Start();

            if (!_initialized)
            {
                Initialize();
            }

            // 初始状态设置
            UpdateVisualState(isOn);
        }

        private void Initialize()
        {
            if (_initialized) return;

            // 添加值变化监听
            onValueChanged.AddListener(OnToggleValueChanged);

            // 如果没有指定targetImage，尝试获取当前对象上的Image组件
            if (targetImage == null && useStateResources)
            {
                targetImage = GetComponent<Image>();
            }

            // 设置文本内容和颜色
            if (labelText != null)
            {
                labelText.text = text;
                labelText.color = textColor;
                // 确保文本不可点击，不会遮挡Toggle的点击区域
                labelText.raycastTarget = false;
            }

            _initialized = true;
        }

        /// <summary>
        /// Toggle值变化时的处理
        /// </summary>
        protected virtual void OnToggleValueChanged(bool isOn)
        {
            UpdateVisualState(isOn);

            // 触发扩展事件
            onValueChangeWithData?.Invoke(isOn, data);
        }

        /// <summary>
        /// 更新视觉状态
        /// </summary>
        protected virtual void UpdateVisualState(bool isOn)
        {
            // 处理节点显示/隐藏
            UpdateNodesVisibility(isOn);

            // 处理状态资源切换
            if (useStateResources)
            {
                UpdateStateResources(isOn);
            }
        }

        /// <summary>
        /// 更新节点显示/隐藏
        /// </summary>
        protected virtual void UpdateNodesVisibility(bool isOn)
        {
            // 直接切换显示/隐藏
            if (ActiveNode != null)
            {
                ActiveNode.SetActive(isOn);
            }

            if (InactiveNode != null)
            {
                InactiveNode.SetActive(!isOn);
            }
        }

        /// <summary>
        /// 更新状态资源
        /// </summary>
        protected virtual void UpdateStateResources(bool isOn)
        {
            if (targetImage != null)
            {
                // 切换Sprite
                if (isOn && activeSprite != null)
                {
                    targetImage.sprite = activeSprite;
                }
                else if (!isOn && inactiveSprite != null)
                {
                    targetImage.sprite = inactiveSprite;
                }

                // 切换颜色
                if (useColorTransition)
                {
                    // 直接设置颜色
                    targetImage.color = isOn ? activeColor : inactiveColor;
                }
            }
        }

        /// <summary>
        /// 设置Toggle状态但不触发事件
        /// </summary>
        public void SetIsOnWithoutNotify(bool value)
        {
            base.SetIsOnWithoutNotify(value);
            UpdateVisualState(value);
        }

        /// <summary>
        /// 重写点击事件处理
        /// </summary>
        public override void OnPointerClick(PointerEventData eventData)
        {
            if (!onCanClick?.Invoke() ?? false)
            {
                onCanClickFailed?.Invoke();
                return;
            }
            base.OnPointerClick(eventData);

            // 播放音效
            if (playSoundOnClick && Application.isPlaying)
            {
                // 这里可以调用游戏中的音效系统播放音效
                // AudioManager.Instance.PlaySound(soundName);
                Debug.Log($"播放音效: {soundName}");
            }

            // 触发点击事件
            onClick?.Invoke(this);
        }

        /// <summary>
        /// 设置文本内容
        /// </summary>
        public void SetText(string newText)
        {
            text = newText;
            if (labelText != null)
            {
                labelText.text = newText;
            }
        }

        /// <summary>
        /// 设置文本颜色
        /// </summary>
        public void SetTextColor(Color color)
        {
            textColor = color;
            if (labelText != null)
            {
                labelText.color = color;
            }
        }

        /// <summary>
        /// 设置绑定数据
        /// </summary>
        public void SetData(string newData)
        {
            data = newData;
        }

        /// <summary>
        /// 设置状态资源
        /// </summary>
        public void SetStateResources(Sprite active, Sprite inactive)
        {
            activeSprite = active;
            inactiveSprite = inactive;
            useStateResources = true;

            // 立即更新视觉状态
            if (_initialized)
            {
                UpdateStateResources(isOn);
            }
        }

        /// <summary>
        /// 设置状态颜色
        /// </summary>
        public void SetStateColors(Color active, Color inactive)
        {
            activeColor = active;
            inactiveColor = inactive;
            useColorTransition = true;

            // 立即更新视觉状态
            if (_initialized)
            {
                UpdateStateResources(isOn);
            }
        }

        public void SetOnCanClick(Func<bool> onCanClick, Action onCanClickFailed)
        {
            this.onCanClick = onCanClick;
            this.onCanClickFailed = onCanClickFailed;
        }

        /// <summary>
        /// 重置组件状态
        /// </summary>
        public virtual void Reset()
        {
            isOn = false;
            UpdateVisualState(false);
        }

        public void Hide()
        {

        }

        public void Show()
        {

        }

        public void Release()
        {
        }
        public void Create()
        {
        }

        protected override void OnDestroy()
        {
            Release();
            base.OnDestroy();
        }

        public void OnSelectTrigger(bool isSelected)
        {
            throw new NotImplementedException();
        }

        public void UIComponentAwake()
        {
            throw new NotImplementedException();
        }

        public void UIComponentDestroy()
        {
            throw new NotImplementedException();
        }
    }
}