using UnityEngine;

namespace OM.AC
{
    [System.Serializable]
    [ACClipCreate("Canvas/Canvas Group Alpha", "Canvas Group Alpha")]
    public class ClipCanvasGroup : ACTweenClip<float,CanvasGroup>
    {
        protected override float GetCurrentValue()
        {
            return target.alpha;
        }

        protected override void SetValue(float newValue)
        {
            target.alpha = newValue;
        }
        
        protected override void OnUpdate(ACEvaluateState state, float timelineTime, float clipTime,
            float normalizedClipTime, bool previewMode)
        {
            if(state != ACEvaluateState.Running) return;
            SetValue(Mathf.LerpUnclamped(CurrentFrom, to, ease.Lerp(normalizedClipTime)));
        }
    }
}