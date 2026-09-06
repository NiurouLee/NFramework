using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace OM.AC.Editor
{
    /// <summary>
    /// The timeline Footer Section, contains the buttons
    /// </summary>
    public class ACTimelineFooterSection : VisualElement,IACUpdateable
    {
        private readonly ACTimeline _timeline;
        private readonly VisualElement _rightContainer;
        private readonly VisualElement _leftContainer;
        private readonly Button _pasteButton;
        
        public ACTimelineFooterSection(ACTimeline timeline)
        {
            _timeline = timeline;
            AddToClassList("footer-section");
            
            _leftContainer = new VisualElement();
            _leftContainer.AddToClassList("footer-container");
            Add(_leftContainer);

            var addButton = new Button(() =>
            {
                var guiToScreenPoint = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
                SearchWindow.Open(new SearchWindowContext(guiToScreenPoint), _timeline.SearchWindow);
                
            });
            addButton.text = "+";
            addButton.AddToClassList("btn");
            _leftContainer.Add(addButton);
            
            
            _pasteButton = new Button(() =>
            {
                if (_timeline.AnimatorEditor.TryGetClipFromCopy(out var result))
                {
                    _timeline.AddClip(result);
                }
            });
            _pasteButton.text = "Paste";
            _pasteButton.AddToClassList("btn");
            _leftContainer.Add(_pasteButton);
            
            
            _rightContainer = new VisualElement();
            _rightContainer.AddToClassList("footer-container");
            Add(_rightContainer);
            
            var copyButton = new Button(() =>
            {
                _timeline.AnimatorEditor.CopyClip(_timeline.AnimatorEditor.SelectedClip);
            });
            copyButton.text = "Copy";
            copyButton.AddToClassList("btn");
            _rightContainer.Add(copyButton);
            
            var duplicateButton = new Button(() =>
            {
                _timeline.ContentSection.DuplicateClip(_timeline.AnimatorEditor.SelectedClip);
            });
            duplicateButton.text = "Duplicate";
            duplicateButton.AddToClassList("btn");
            _rightContainer.Add(duplicateButton);
            
            var deleteButton = new Button(() =>
            {
                _timeline.ContentSection.DeleteClip(_timeline.AnimatorEditor.SelectedClip);
            });
            deleteButton.text = "Delete";
            deleteButton.AddToClassList("btn");
            _rightContainer.Add(deleteButton);
        }

        public void Update()
        {
            _pasteButton.style.opacity = string.IsNullOrEmpty(ACAnimatorEditor.CopiedClipJson)? 0 : 1;
            _rightContainer.style.opacity = _timeline.AnimatorEditor.SelectedClip == null ? 0 : 1;
        }
    }
}