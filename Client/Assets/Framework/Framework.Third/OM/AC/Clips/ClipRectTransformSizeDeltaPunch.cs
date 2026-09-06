using UnityEngine;

namespace OM.AC
{
    [System.Serializable]
    [ACClipCreate("RectTransform/Size Delta Punch", "Size Delta Punch")]
    public class ClipRectTransformSizeDeltaPunch : ACPunchCore<Vector2,RectTransform>
    {
        protected override Vector2 GetCurrentValue()
        {
            return target.sizeDelta;
        }

        protected override void SetValue(Vector2 newValue)
        {
            target.sizeDelta = newValue;
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