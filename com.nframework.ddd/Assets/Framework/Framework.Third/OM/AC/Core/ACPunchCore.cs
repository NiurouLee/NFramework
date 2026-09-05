using OM.Shared;
using UnityEngine;

namespace OM.AC
{
    /// <summary>
    /// Core class for all punch clips
    /// </summary>
    /// <typeparam name="TType">The type of the Value such as Vector3, float...</typeparam>
    /// <typeparam name="TTarget">the type of the target such as Transform, Image, CanvasGroup...</typeparam>
    [System.Serializable]
    public abstract class ACPunchCore<TType,TTarget> : ACClipCore<TType> where TType : struct where TTarget : Component
    {
        [SerializeField,CheckForNull] protected TTarget target;
        [SerializeField] protected TType startValue;
        [SerializeField] protected TType punchStrength;
        [SerializeField] protected int vibrato = 10;
        [SerializeField] protected float elasticity = 1f;
        
        public override void OnClipAddedToACAnimator(ACAnimator animator)
        {
            if (target == null) target = animator.GetComponent<TTarget>();
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