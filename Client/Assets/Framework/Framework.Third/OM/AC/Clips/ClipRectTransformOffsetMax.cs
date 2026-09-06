using UnityEngine;

namespace OM.AC
{
    [System.Serializable]
    [ACClipCreate("RectTransform/Offset Max", "Offset Max")]
    public class ClipRectTransformOffsetMax : ACTweenClip<Vector2,RectTransform>
    {
        protected override Vector2 GetCurrentValue()
        {
            return target.offsetMax;
        }

        protected override void SetValue(Vector2 newValue)
        {
            target.offsetMax = newValue;
        }
        
        protected override void OnUpdate(ACEvaluateState state, float timelineTime, float clipTime,
            float normalizedClipTime, bool previewMode)
        {
            if(state != ACEvaluateState.Running) return;
            SetValue(Vector2.LerpUnclamped(CurrentFrom, to, ease.Lerp(normalizedClipTime)));
        }
    }
}