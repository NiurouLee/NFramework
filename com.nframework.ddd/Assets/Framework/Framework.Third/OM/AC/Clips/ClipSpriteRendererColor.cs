using UnityEngine;

namespace OM.AC
{
    [System.Serializable]
    [ACClipCreate("SpriteRenderer/Color", "SpriteRenderer Color")]
    public class ClipSpriteRendererColor : ACTweenClip<Color,SpriteRenderer>
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