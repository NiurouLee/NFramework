using UnityEngine;

namespace OM.AC
{
    [System.Serializable]
    [ACClipCreate("Transform/Scale Punch", "Animate Scale Punch")]
    public class ClipTransformScalePunch : ACPunchCore<Vector3,Transform>
    {
        protected override Vector3 GetCurrentValue()
        {
            return target.localScale;
        }

        protected override void SetValue(Vector3 newValue)
        {
            target.localScale = newValue;
        }
        
        protected override void OnUpdate(ACEvaluateState state, float timelineTime, float clipTime,
            float normalizedClipTime, bool previewMode)
        {
            if(state != ACEvaluateState.Running) return;
            var damper = 1f - Mathf.Clamp(2f * normalizedClipTime - 1f, 0f, 1f);
            var bounce = Mathf.Sin(normalizedClipTime * Mathf.PI * vibrato) * elasticity;
            var newPosition = punchStrength * (damper * bounce);
            SetValue(startValue + newPosition);
        }
    }
}