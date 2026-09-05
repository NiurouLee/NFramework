using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace OM.AC.Editor
{
    /// <summary>
    /// Timeline Header Section, contains the preview button and the timeline length field
    /// </summary>
    public class ACTimelineHeaderSection : VisualElement,IACUpdateable
    {
        private readonly ACTimeline _timeline;
        private readonly Button _previewButton;

        public ACTimelineHeaderSection(ACTimeline timeline)
        {
            _timeline = timeline;
            AddToClassList("header-section");
                        
            var label = new Label("Timeline");
            Add(label);
            
            _previewButton = new Button(() =>
            {
                if(Application.isPlaying) return;
                if (ACAnimatorEditor.PreviewInstance == _timeline.AnimatorEditor)
                {
                    _timeline.AnimatorEditor.SetPreviewInstance(null);
                }
                else
                {
                    _timeline.AnimatorEditor.SetPreviewInstance(_timeline.AnimatorEditor);
                    _timeline.Animator.Evaluate(_timeline.Animator.TimelineTime,true);
                }
            });
            _previewButton.text = "Preview";
            _previewButton.AddToClassList("preview-button");
            _previewButton.AddToClassList("btn");
            Add(_previewButton);

            var timelineLengthProp = _timeline.AnimatorEditor.serializedObject.FindProperty("fullDuration");
            
            var floatField = new FloatField("Timeline Length");
            floatField.AddToClassList("timeline-length-field");
            floatField.label = " ";
            floatField.Bind(_timeline.AnimatorEditor.serializedObject);
            floatField.BindProperty(timelineLengthProp);
            floatField.RegisterValueChangedCallback((e) =>
            {
                floatField.value = Mathf.Max(e.newValue, .1f); 
            });
            Add(floatField);
        }

        public void Update()
        {
            var b = !_timeline.AnimatorEditor.IsPreviewInstance();
            _previewButton.text = b ? "Preview" : "Stop Preview";
            if (b)
            {
                _previewButton.RemoveFromClassList("preview-btn-on");
            }
            else
            {
                _previewButton.AddToClassList("preview-btn-on");
            }
        }
    }
}