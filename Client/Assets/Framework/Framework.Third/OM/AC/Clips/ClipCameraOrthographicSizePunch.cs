using UnityEngine;

namespace OM.AC
{
    [System.Serializable]
    [ACClipCreate("Camera/Orthographic Size Punch", "Camera Orthographic Size Punch")]
    public class ClipCameraOrthographicSizePunch : ACPunchCore<float,Camera>
    {
        protected override float GetCurrentValue()
        {
            return target.orthographicSize;
        }

        protected override void SetValue(float newValue)
        {
            target.orthographicSize = newValue;
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