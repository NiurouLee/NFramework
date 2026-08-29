using UnityEngine;

namespace OM.AC
{
    [System.Serializable]
    [ACClipCreate("RectTransform/Pivot", "Animate Pivot")]
    public class ClipRectTransformPivot : ACTweenClip<Vector2,RectTransform>
    {
        protected override Vector2 GetCurrentValue()
        {
            return target.pivot;
        }

        protected override void SetValue(Vector2 newValue)
        {
            target.pivot = newValue;
        }
        
        protected override void OnUpdate(ACEvaluateState state, float timelineTime, float clipTime,
            float normalizedClipTime, bool previewMode)
        {
            if(state != ACEvaluateState.Running) return;
            SetValue(Vector2.LerpUnclamped(CurrentFrom, to, ease.Lerp(normalizedClipTime)));
        }
    }
}