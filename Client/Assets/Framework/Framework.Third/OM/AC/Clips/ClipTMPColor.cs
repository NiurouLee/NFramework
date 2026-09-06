using TMPro;
using UnityEngine;

namespace OM.AC
{
    [System.Serializable]
    [ACClipCreate("Text Mesh Pro/Color", "Animate Text Color")]
    public class ClipTMPColor : ACTweenClip<Color, TMP_Text>
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