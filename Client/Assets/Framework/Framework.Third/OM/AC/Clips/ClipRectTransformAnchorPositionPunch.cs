using UnityEngine;

namespace OM.AC
{
    [System.Serializable]
    [ACClipCreate("RectTransform/Anchor Position Punch", "Anchor Position Punch")]
    public class ClipRectTransformAnchorPositionPunch : ACPunchCore<Vector3,RectTransform>
    {
        protected override Vector3 GetCurrentValue()
        {
            return target.anchoredPosition3D;
        }

        protected override void SetValue(Vector3 newValue)
        {
            target.anchoredPosition3D = newValue;
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