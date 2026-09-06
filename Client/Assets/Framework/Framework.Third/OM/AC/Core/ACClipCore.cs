using UnityEngine;

namespace OM.AC
{
    /// <summary>
    /// has the functionality of storing a value and setting it when the clip is played or previewed
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [System.Serializable]
    public abstract class ACClipCore<T> : ACClip where T : struct
    {
        [SerializeField] private ACClipInitializer<T> initializer;
        
        private T _storedValue;
        
        public override void OnTimelineStarted()
        {
            initializer.Initialize(() =>
            {
                SetValue(initializer.Value);
            });   
        }

        public override void OnPreviewModeChanged(bool previewMode)
        {
            // Store the value and set it when the preview mode is changed
            if(!CanBePlayedInPreviewMode()) return;
            if (previewMode)
            {
                _storedValue = GetCurrentValue();
                initializer.Initialize(() =>
                {
                    SetValue(initializer.Value);
                });   
            }
            else
            {
                SetValue(_storedValue);
            }
        }

        public override bool CanBePlayedInPreviewMode()
        {
            return true;
        }

        protected abstract T GetCurrentValue();
        protected abstract void SetValue(T newValue);

    }
}