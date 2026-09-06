using System.Collections.Generic;
using UnityEngine;

namespace NFramework.ModuleSystem
{
    // 自定义的 Image 组件，支持多个 Sprite 引用
    [AddComponentMenu("UI/MultiSpriteImage(多状态Image)", 51)]
    public class UIMultiSpriteImage : NImage
    {
        public bool debug = false;
        public bool DefaultActive = true;

        [SerializeField]
        // 用于保存多个 Sprite 引用的数组
        public List<Sprite> sprites = new List<Sprite>();


        [SerializeField]
        int _defaultSpriteIndex = 0;
        public int currentSpriteIndex
        {
            private set
            {
                if (value >= sprites.Count)
                {
                    _defaultSpriteIndex = sprites.Count - 1;
                }
                else if (value <= 0)
                {
                    _defaultSpriteIndex = 0;
                }
                else
                {
                    _defaultSpriteIndex = value;
                }

            }
            get
            {
                return _defaultSpriteIndex;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            // 在 Start 方法中设置默认索引的 Sprite
            SwitchToSprite(_defaultSpriteIndex);
        }

        // 切换到下一个 Sprite
        public void SwitchToNextSprite()
        {
            // 检查数组是否为空
            if (sprites == null || sprites.Count == 0)
            {
                Debug.LogError("Sprites数组为空！");
                return;
            }

            // 切换到下一个索引
            currentSpriteIndex = (currentSpriteIndex + 1) % sprites.Count;

            // 更新显示的 Sprite
            base.sprite = sprites[currentSpriteIndex];
        }

        public new Sprite sprite
        {
            set
            {; }
            get { return null; }
        }

        // 切换到指定索引的 Sprite
        public void SwitchToSprite(int index)
        {
            // 检查数组是否为空
            if (sprites == null || sprites.Count == 0)
            {
                return;
            }

            // 检查索引是否合法
            if (index < 0 || index >= sprites.Count)
            {
                return;
            }

            // 更新当前索引并显示对应的 Sprite
            if (DefaultActive)
                this.gameObject.SetActive(true);
            currentSpriteIndex = index;
            base.sprite = sprites[currentSpriteIndex];
        }
        //
        public void Hide()
        {
            this.gameObject.SetActive(false);
        }
#if UNITY_EDITOR        // 在修改 Source Image 属性时调用
        protected override void OnValidate()
        {
            base.OnValidate();
            if (sprites.Count == 0)
            {
                base.sprite = null;
            }
            else
            {
                currentSpriteIndex = Mathf.Clamp(_defaultSpriteIndex, 0, sprites.Count - 1);
                base.sprite = sprites[currentSpriteIndex];

            }

        }
#endif
        public void Release()
        {
            _defaultSpriteIndex = 0;
            sprites.Clear();
        }
    }
}
