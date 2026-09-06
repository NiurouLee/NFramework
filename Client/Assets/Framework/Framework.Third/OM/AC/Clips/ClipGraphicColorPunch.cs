using UnityEngine;
using UnityEngine.UI;

namespace OM.AC
{
    [System.Serializable]
    [ACClipCreate("RectTransform/Graphic Color Punch", "Graphic Color Punch")]
    public class ClipGraphicColorPunch : ACPunchCore<Color,Graphic>
    {
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
            var damper = 1f - Mathf.Clamp(2f * normalizedClipTime - 1f, 0f, 1f);
            var bounce = Mathf.Sin(normalizedClipTime * Mathf.PI * vibrato) * elasticity;
            var newPosition = punchStrength * (damper * bounce);
            SetValue(startValue + newPosition);
        }
    }
}