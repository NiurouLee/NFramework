using TMPro;
using UnityEngine;

namespace OM.AC
{
    [System.Serializable]
    [ACClipCreate("Text Mesh Pro/Size", "Animate Text Size")]
    public class ClipTMPSize : ACTweenClip<float,TMP_Text>
    {
        protected override float GetCurrentValue()
        {
            return target.fontSize;
        }

        protected override void SetValue(float newValue)
        {
            target.fontSize = newValue;
        }
        
        protected override void OnUpdate(ACEvaluateState state, float timelineTime, float clipTime,
            float normalizedClipTime, bool previewMode)
        {
            if(state != ACEvaluateState.Running) return;
            SetValue(Mathf.LerpUnclamped(CurrentFrom, to, ease.Lerp(normalizedClipTime)));
        }
    }
}