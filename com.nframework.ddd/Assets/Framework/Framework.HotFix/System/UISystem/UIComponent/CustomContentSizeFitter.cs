using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    internal static class SetPropertyUtility
    {
        public static bool SetColor(ref Color currentValue, Color newValue)
        {
            if (currentValue.r == newValue.r && currentValue.g == newValue.g && currentValue.b == newValue.b &&
                currentValue.a == newValue.a)
                return false;

            currentValue = newValue;
            return true;
        }

        public static bool SetStruct<T>(ref T currentValue, T newValue) where T : struct
        {
            if (EqualityComparer<T>.Default.Equals(currentValue, newValue))
                return false;

            currentValue = newValue;
            return true;
        }

        public static bool SetClass<T>(ref T currentValue, T newValue) where T : class
        {
            if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))
                return false;

            currentValue = newValue;
            return true;
        }
    }

    [AddComponentMenu("Layout/Custom Content Size Fitter", 141)]
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    /// <summary>
    /// Resizes a RectTransform to fit the size of its content.
    /// </summary>
    /// <remarks>
    /// The ContentSizeFitter can be used on GameObjects that have one or more ILayoutElement components, such as Text, Image, HorizontalLayoutGroup, VerticalLayoutGroup, and GridLayoutGroup.
    /// </remarks>
    public class CustomContentSizeFitter : UIBehaviour, ILayoutSelfController
    {
        [SerializeField] protected ContentSizeFitter.FitMode m_HorizontalFit = ContentSizeFitter.FitMode.Unconstrained;

        /// <summary>
        /// The fit mode to use to determine the width.
        /// </summary>
        public ContentSizeFitter.FitMode horizontalFit
        {
            get { return m_HorizontalFit; }
            set
            {
                if (SetPropertyUtility.SetStruct(ref m_HorizontalFit, value)) SetDirty();
            }
        }

        [SerializeField] protected ContentSizeFitter.FitMode m_VerticalFit = ContentSizeFitter.FitMode.Unconstrained;

        /// <summary>
        /// The fit mode to use to determine the height.
        /// </summary>
        public ContentSizeFitter.FitMode verticalFit
        {
            get { return m_VerticalFit; }
            set
            {
                if (SetPropertyUtility.SetStruct(ref m_VerticalFit, value)) SetDirty();
            }
        }


        [System.NonSerialized] private RectTransform m_Rect;

        private RectTransform rectTransform
        {
            get
            {
                if (m_Rect == null)
                    m_Rect = GetComponent<RectTransform>();
                return m_Rect;
            }
        }

        // field is never assigned warning
#pragma warning disable 649
        private DrivenRectTransformTracker m_Tracker;
#pragma warning restore 649

        protected CustomContentSizeFitter()
        {
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }

        protected override void OnDisable()
        {
            m_Tracker.Clear();
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
            base.OnDisable();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            SetDirty();
        }

        private void HandleSelfFittingAlongAxis(int axis)
        {
            ContentSizeFitter.FitMode fitting = (axis == 0 ? horizontalFit : verticalFit);
            if (fitting == ContentSizeFitter.FitMode.Unconstrained)
            {
                // Keep a reference to the tracked transform, but don't control its properties:
                m_Tracker.Add(this, rectTransform, DrivenTransformProperties.None);
                return;
            }

            m_Tracker.Add(this, rectTransform,
                (axis == 0 ? DrivenTransformProperties.SizeDeltaX : DrivenTransformProperties.SizeDeltaY));

            // Set size to min or preferred size
            if (fitting == ContentSizeFitter.FitMode.MinSize)
                rectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis)axis,
                    LayoutUtility.GetMinSize(m_Rect, axis));
            else
                rectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis)axis, GetPreferredWidth(axis));
        }

        [Header("最大宽度 0 则不限制")]
        public float MaxWidth;
        [Header("最大高度 0 则不限制")]
        public float MaxHeight;
        public float GetPreferredWidth(int axis)
        {
            //return axis == 0 ? GetPreferredWidth(rect) : GetPreferredHeight(rect);
            float _size = LayoutUtility.GetPreferredSize(m_Rect, axis);
            float _max = axis == 0 ? MaxWidth : MaxHeight;
            if (_size >= _max&&_max!= 0)
            {
                return _max;
            }
            return _size;
        }


        /// <summary>
        /// Calculate and apply the horizontal component of the size to the RectTransform
        /// </summary>
        public virtual void SetLayoutHorizontal()
        {
            m_Tracker.Clear();
            HandleSelfFittingAlongAxis(0);
        }

        /// <summary>
        /// Calculate and apply the vertical component of the size to the RectTransform
        /// </summary>
        public virtual void SetLayoutVertical()
        {
            HandleSelfFittingAlongAxis(1);
        }

        protected void SetDirty()
        {
            if (!IsActive())
                return;

            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            SetDirty();
        }

#endif
    }
}