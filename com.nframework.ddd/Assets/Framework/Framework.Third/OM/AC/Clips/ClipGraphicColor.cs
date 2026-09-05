using UnityEngine;
using UnityEngine.UI;

namespace OM.AC
{
    [System.Serializable]
    [ACClipCreate("Graphic/Color", "Graphic Color")]
    public class ClipGraphicColor : ACTweenClip<Color,Graphic>
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
            SetValue(Color.LerpUnclamped(CurrentFrom, to, ease.Lerp(normalizedClipTime)));
        }
    }
}