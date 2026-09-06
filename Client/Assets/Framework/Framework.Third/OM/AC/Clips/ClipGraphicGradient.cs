using OM.Shared;
using UnityEngine;
using UnityEngine.UI;

namespace OM.AC
{
    [System.Serializable]
    [ACClipCreate("Graphic/Gradient", "Graphic Gradient")]
    public class ClipGraphicGradient : ACClipCore<Color>
    {
        [SerializeField,CheckForNull] private Graphic target;
        [SerializeField] private Gradient gradient;
        
        public override bool IsValid()
        {
            return target != null;
        }

        protected override Color GetCurrentValue()
        {
            return target.color;
        }

        protected override void SetValue(Color newValue)
        {
            target.color = newValue;
        }
        
        protected override void OnUpdate(ACEvaluateState state, float timelineTime, float clipTime,
            float normalizedClipTime, bool previewMode)
        {
            if(state != ACEvaluateState.Running) return;
            SetValue(gradient.Evaluate(normalizedClipTime));
        }
        
        public override void OnClipAddedToACAnimator(ACAnimator animator)
        {
            if(target == null) target = animator.GetComponent<Graphic>();
        }
        
        public override Component GetTarget()
        {
            return target;
        }
        
        public override void SetTarget(GameObject newTarget)
        {
            this.target = newTarget.GetComponent<Graphic>();
        }
    }
}