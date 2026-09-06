using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace OM.AC.Editor
{
    /// <summary>
    /// the Timeline Content Section stores all the timeline Clips
    /// </summary>
    public class ACTimelineContentSection : VisualElement,IACUpdateable,IACClickable
    {
        private readonly ACTimeline _timeline;
        private readonly List<ACTimelineClip> _timelineClips = new List<ACTimelineClip>();
        
        public VisualElement BackgroundContainer { get; }
        
        public ACTimelineContentSection(ACTimeline timeline)
        {
            _timeline = timeline;
            AddToClassList("content-section");

            BackgroundContainer = new VisualElement();
            BackgroundContainer.name = "background-container";
            BackgroundContainer.pickingMode = PickingMode.Ignore;
            Add(BackgroundContainer);
        }

        
        public void DestroyAndInstantiateClips(bool force = false)
        {
            if (!force && _timelineClips.Count == _timeline.Animator.GetClips().Count)
            {
                return;
            }
            foreach (var timelineClip in _timelineClips)
            {
                timelineClip.RemoveFromHierarchy();
            }
            _timelineClips.Clear();
            BackgroundContainer.Clear();

            for (var i = 0; i < _timeline.Animator.GetClips().Count; i++)
            {
                var newClip = _timeline.Animator.GetClips()[i];
                if (newClip == null) continue;
                AddTimelineClip(newClip);
            }
            Update();
        }

        public void AddTimelineClip(ACClip newClip)
        {
            var row = new VisualElement();
            row.pickingMode = PickingMode.Ignore;
            row.AddToClassList("timeline-row");
            BackgroundContainer.Add(row);
                
            var timelineClip = new ACTimelineClip(_timeline, newClip);
            timelineClip.Update();
            _timelineClips.Add(timelineClip);
            Add(timelineClip);
            timelineClip.style.opacity = 1;
        }
        
        public void Update()
        {
            foreach (var timelineClip in _timelineClips)
            {
                timelineClip.Update();
            }
        }

        public void Click(MouseButton mouseButton)
        {
            if (mouseButton == MouseButton.LeftMouse)
            {
                _timeline.AnimatorEditor.SetSelectedClip(null);
            }
            else if (mouseButton == MouseButton.RightMouse)
            {
                ShowContextMenu();
            }
        }
        
        private void ShowContextMenu()
        {
            var menu = new GenericMenu();
            var guiToScreenPoint = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
            menu.AddItem(new GUIContent("Add Clip"), false, () =>
            {
                SearchWindow.Open(new SearchWindowContext(guiToScreenPoint), _timeline.SearchWindow);
            });
            if (!string.IsNullOrEmpty(ACAnimatorEditor.CopiedClipJson))
            {
                menu.AddItem(new GUIContent("Paste Clip"), false, TryPasteCopiedClip);
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Paste Clip"),false);
            }
            
            menu.AddItem(new GUIContent("Trim Timeline"), false, () =>
            {
                var timelineLastClip = _timeline.Animator.GetTimelineLastClip();
                if (timelineLastClip == null) return;
                Undo.RecordObject(_timeline.Animator,"Trim Timeline");
                _timeline.Animator.FullDuration = timelineLastClip.GetEndTime();
            });
            menu.ShowAsContext();
        }

        public void DeleteClip(ACClip clip)
        {
            Undo.RecordObject(_timeline.Animator,"Delete Clip");
            if (_timeline.AnimatorEditor.IsPreviewInstance())
            {
                clip.OnPreviewModeChanged(false);
            }
            _timeline.Animator.RemoveClip(clip);
            var firstOrDefault = _timelineClips.FirstOrDefault(x=>x.Clip == clip);
            if (firstOrDefault != null)
            {
                _timelineClips.Remove(firstOrDefault);
                firstOrDefault.RemoveFromHierarchy();
            }
            BackgroundContainer.RemoveAt(BackgroundContainer.childCount-1);
            
            if(_timeline.AnimatorEditor.SelectedClip == clip) _timeline.AnimatorEditor.SetSelectedClip(null);
        }
        
        public ACTimelineClip GetTimelineClip(ACClip clip)
        {
            return _timelineClips.FirstOrDefault(x => x.Clip == clip);
        }

        public void DuplicateClip(ACTimelineClip timelineClip)
        {
            var clip = timelineClip.Clip;
            Undo.RecordObject(_timeline.Animator,"Duplicate Clip");
            var newClip = clip.Clone<ACClip>();
            newClip.Name = $"{newClip.Name} (Copy)";
            _timeline.Animator.GetClips().Insert(timelineClip.Index + 1,newClip);
            AddTimelineClip(newClip);
        }

        public void DuplicateClip(ACClip clip)
        {
            var timelineClip = GetTimelineClip(clip);
            if (timelineClip != null)
            {
                DuplicateClip(timelineClip);
            }
        }

        public void TryPasteCopiedClip()
        {
            if (_timeline.AnimatorEditor.TryGetClipFromCopy(out var result))
            {
                _timeline.AddClip(result);
            }
        }
    }
}