using OM.Ease;
using UnityEngine;

namespace OM.AC
{
    public enum ACFromType
    {
        CustomFrom,
        CurrentValue
    }
    /// <summary>
    /// base class for all tween clips
    /// </summary>
    /// <typeparam name="TType">The type of the Value such as Vector3, float...</typeparam>
    /// <typeparam name="TTarget">the type of the target such as Transform, Image, CanvasGroup...</typeparam>
    [System.Serializable]
    public abstract class ACTweenClip<TType,TTarget> : ACClipCore<TType> where TType : struct where TTarget : Component
    {
        [SerializeField,CheckForNull] protected TTarget target;
        [SerializeField] protected EaseData ease;
        [SerializeField] protected ACFromType fromType = ACFromType.CustomFrom;
        [SerializeField] protected TType from;
        [SerializeField] protected TType to;
        
        protected TType CurrentFrom { get; set; }

        public override void OnClipAddedToACAnimator(ACAnimator animator)
        {
            if(target == null) target = animator.GetComponent<TTarget>();
        }

        protected override void OnEnter()
        {
            CurrentFrom = fromType == ACFromType.CustomFrom ? from : GetCurrentValue();
        }

        public override bool IsValid()
        {
            return target != null;
        }
        
        public override Component GetTarget()
        {
            return target;
        }

        public override void SetTarget(GameObject newTarget)
        {
            this.target = newTarget.GetComponent<TTarget>();
        }
    }
}