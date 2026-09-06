using System.Collections.Generic;
using System.Reflection;
using OM.Shared;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace OM.AC.Editor
{
    /// <summary>
    /// Timeline Clip Handles the Dragging and the Drawing of the Clip
    /// </summary>
    public class ACTimelineClip : VisualElement,IACUpdateable,IACDraggable,IACClickable
    {
        public ACTimeline Timeline { get; }
        public ACClip Clip { get; }
        public int Index => Timeline.Animator.GetClips().IndexOf(Clip);

        private readonly ACTimelineClipRightHandle _rightHandle;
        private readonly ACTimelineClipLeftHandle _leftHandle;
        private readonly VisualElement _colorLine;
        private readonly VisualElement _selectionBorder;
        private readonly VisualElement _snapLine;
        private readonly Label _nameLabel;
        private readonly VisualElement _warningIcon;
        
        private bool _useSnapping = true;
        private bool _isDragging;
        private Vector2 _startPos;
        private readonly List<ACClip> _allClips = new List<ACClip>();
        private readonly List<FieldInfo> _fieldsWithCheckForNullAttribute = new List<FieldInfo>();

        
        public ACTimelineClip(ACTimeline timeline, ACClip clip)
        {
            Timeline = timeline;
            Clip = clip;

            foreach (var fieldInfo in Clip.GetType().GetFields(BindingFlags.Default | BindingFlags.Instance | BindingFlags.NonPublic))
            {
                var checkForNullAttribute = fieldInfo.GetCustomAttribute<CheckForNullAttribute>();
                if (checkForNullAttribute != null)
                {
                    _fieldsWithCheckForNullAttribute.Add(fieldInfo);
                }
            }

            this.RegisterCallback<DragUpdatedEvent>((e) =>
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                e.StopPropagation();
            });
            this.RegisterCallback<DragPerformEvent>((e) =>
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                
                var draggedObject = DragAndDrop.objectReferences[0];
                if (draggedObject == null || !(draggedObject is GameObject))
                {
                    e.StopPropagation();
                    return;
                }
                Undo.RecordObject(timeline.Animator,"Set Target");
                Clip.SetTarget(draggedObject as GameObject);
                
                DragAndDrop.AcceptDrag();
                e.StopPropagation();
            });

            
            AddToClassList("timeline-clip");
            
            _colorLine = new VisualElement();
            _colorLine.pickingMode = PickingMode.Ignore;
            _colorLine.AddToClassList("clip-color-line");
            Add(_colorLine);
            
            _nameLabel = new Label(Clip.Name);
            _nameLabel.AddToClassList("clip-name");
            _nameLabel.pickingMode = PickingMode.Ignore;
            Add(_nameLabel);

            _rightHandle = new ACTimelineClipRightHandle(this);
            _rightHandle.style.right = -5;
            Add(_rightHandle);
            
            _leftHandle = new ACTimelineClipLeftHandle(this);
            _leftHandle.style.left = -5;
            Add(_leftHandle);
            
            _selectionBorder = new VisualElement();
            _selectionBorder.pickingMode = PickingMode.Ignore;
            _selectionBorder.AddToClassList("clip-selection-border");
            Add(_selectionBorder);
            
            _snapLine = new VisualElement();
            _snapLine.pickingMode = PickingMode.Ignore;
            _snapLine.AddToClassList("clip-snap-line");
            Timeline.TopContainer.Add(_snapLine);
            
            
            _warningIcon = new VisualElement();
            _warningIcon.pickingMode = PickingMode.Ignore;
            _warningIcon.style.backgroundImage = (StyleBackground)EditorGUIUtility.IconContent("console.warnicon").image;
            _warningIcon.AddToClassList("clip-warning-icon");
            Add(_warningIcon);
            
            if (Clip.GetTarget() != null)
            {
                var highlightButton = new Button(() =>
                {
                    EditorGUIUtility.PingObject(Clip.GetTarget());
                });
                highlightButton.text = "*";
                highlightButton.AddToClassList("clip-highlight-button");
                Add(highlightButton);
            }
        }

        public void Update()
        {
            var isValid = CheckIfClipIsValid();

            var styleBackgroundColor = Clip.Color;
            styleBackgroundColor.a = (Clip.Enabled && !isValid)? 1f : .2f;
            _colorLine.style.backgroundColor = styleBackgroundColor;
            
            //_colorLine.style.backgroundColor = Clip.Color;
            _nameLabel.text = Clip.Name;
            
            var width = Clip.Duration * Timeline.GetWidthPerSecond();
            style.width = width;
            style.left = Clip.StartAt * Timeline.GetWidthPerSecond();
            if (!_isDragging)
            {
                style.top = Timeline.GetClipYPosition(Index);
            }
            _nameLabel.style.color = (Clip.Enabled && !isValid)? Color.white : Color.gray;
            _selectionBorder.style.opacity = Timeline.AnimatorEditor.SelectedClip == Clip ? 1 : 0;
            _warningIcon.style.opacity = isValid ? 1 : 0;
        }

        private void RefreshAllClips()
        {
            _allClips.Clear();
            _allClips.AddRange(Timeline.Animator.GetClips());
            _allClips[Index] = null;
        } 

        public void StartDrag(Vector2 mousePosition)
        {
            _isDragging = true;
            _startPos = layout.position;
            BringToFront();
            AddToClassList("timeline-clip-dragging");
            
            RefreshAllClips();
            _snapLine.style.top = Timeline.ContentSection.layout.position.y;
            _snapLine.style.height = Timeline.ContentSection.layout.height;

            _useSnapping = ACSettings.GetOrCreateSettings().Snapping;
        }

        public bool Drag(Vector2 delta, Vector2 mousePosition)
        {
            var newPos = _startPos + delta;
            newPos.x = Mathf.Clamp(newPos.x,0f, Timeline.ContentSection.layout.width - layout.width);
            newPos.y = Mathf.Clamp(newPos.y, 0f, Timeline.ContentSection.layout.height - layout.height);


            if (_useSnapping)
            {
                var hasSnapping = false;
                var snapLinePos = new Vector2(0f,Timeline.GetClipYPosition(Timeline.GetClipIndex(newPos.y)) + ACTimeline.TimelineClipHeight);
                
                for (var i = 0; i < _allClips.Count; i++)
                {
                    var clip = _allClips[i];
                    if (clip == null) continue;
                    
                    if (IsWithinRange(newPos.x, Timeline.GetWidthPerSecond() * clip.StartAt, 5f))
                    {
                        newPos.x = Timeline.GetWidthPerSecond() * clip.StartAt;
                        hasSnapping = true;
                        snapLinePos.x = newPos.x;
                        if(Index < i) snapLinePos.y += ACTimeline.TimelineClipHeight;


                        break;
                    }

                    if (IsWithinRange(newPos.x + layout.width, Timeline.GetWidthPerSecond() * clip.GetEndTime(), 5f))
                    {
                        newPos.x = Timeline.GetWidthPerSecond() * clip.GetEndTime() - layout.width;
                        hasSnapping = true;
                        snapLinePos.x = newPos.x + layout.width;
                        if(Index < i) snapLinePos.y += ACTimeline.TimelineClipHeight;
                        break;
                    }

                    if (IsWithinRange(newPos.x, Timeline.GetWidthPerSecond() * clip.GetEndTime(), 5f))
                    {
                        newPos.x = Timeline.GetWidthPerSecond() * clip.GetEndTime();
                        hasSnapping = true;
                        snapLinePos.x = newPos.x;
                        if(Index < i) snapLinePos.y += ACTimeline.TimelineClipHeight;
                        break;
                    }

                    if (IsWithinRange(newPos.x + layout.width, Timeline.GetWidthPerSecond() * clip.StartAt, 5f))
                    {
                        newPos.x = Timeline.GetWidthPerSecond() * clip.StartAt - layout.width;
                        hasSnapping = true;
                        snapLinePos.x = newPos.x + layout.width;
                        if(Index < i) snapLinePos.y += ACTimeline.TimelineClipHeight;
                        break;
                    }
                }

                if (hasSnapping)
                {
                    _snapLine.style.left = snapLinePos.x;
                    _snapLine.style.opacity = 1;
                }
                else
                {
                    _snapLine.style.opacity = 0;
                }
            }
            
            
            style.left = newPos.x;
            style.top = newPos.y;
            
            Undo.RecordObject(Timeline.Animator, $"Drag Clip {Clip.Name}");
            Clip.StartAt = Mathf.Min(newPos.x * Timeline.GetSecondsPerWidth(), Timeline.Animator.FullDuration - Clip.Duration);
            
            var newIndex = Timeline.GetClipIndex(newPos.y);
            if (newIndex != Index)
            {
                Timeline.AnimatorEditor.serializedObject.Update();
                if (Timeline.AnimatorEditor.serializedObject.FindProperty("clips").MoveArrayElement(Index, newIndex))
                {
                    Undo.IncrementCurrentGroup();
                    Timeline.AnimatorEditor.serializedObject.ApplyModifiedProperties();
                }
                RefreshAllClips();
            }
            
            return false;
        }
        
        public void EndDrag(Vector2 delta, Vector2 mousePosition)
        {
            _isDragging = false;
            RemoveFromClassList("timeline-clip-dragging");
            _snapLine.style.opacity = 0;
            Timeline.AnimatorEditor.serializedObject.ApplyModifiedProperties();
        }

        public void Click(MouseButton mouseButton)
        {
            if (mouseButton == MouseButton.LeftMouse)
            {
                if (Timeline.AnimatorEditor.SelectedClip != Clip)
                {
                    Timeline.AnimatorEditor.SetSelectedClip(Clip);
                }
                else
                {
                    Timeline.AnimatorEditor.SetSelectedClip(null);
                }
            }
            else if(mouseButton == MouseButton.RightMouse)
            {
                ShowContextMenu();
            }
        }

        private void ShowContextMenu()
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Delete"),false,() =>
            {
                Timeline.ContentSection.DeleteClip(Clip);
            });
            menu.AddItem(new GUIContent("Duplicate"),false,() =>
            {
                Timeline.ContentSection.DuplicateClip(this);
            });
            menu.AddItem(new GUIContent("Copy"),false,() =>
            {
                Timeline.AnimatorEditor.CopyClip(Clip);
            });
            menu.AddItem(new GUIContent("Focus"),false, () =>
            {
                Undo.RecordObject(Timeline.Animator,"Focus");
                foreach (var clip in Timeline.Animator.GetClips())
                {
                    if (clip == Clip)
                    {
                        clip.Enabled = true;
                        continue;
                    }
                    clip.Enabled = false;
                }
            });
            menu.AddItem(new GUIContent("UnFocus"),false, () =>
            {
                Undo.RecordObject(Timeline.Animator,"UnFocus");
                foreach (var clip in Timeline.Animator.GetClips())
                {
                    clip.Enabled = true;
                }
            });
            menu.AddItem(new GUIContent("Toggle Enabled"),Clip.Enabled,() =>
            {
                Undo.RecordObject(Timeline.Animator,"Toggle Enabled");
                Clip.Enabled = !Clip.Enabled;
            });

            menu.ShowAsContext();
        }

        public static bool IsWithinRange(float value, float target, float range)
        {
            return Mathf.Abs(value - target) <= range;
        }
        
        public bool CheckIfClipIsValid()
        {
            if (Clip == null)
            {
                return false;
            }
            var hasNull = false;
            foreach (var fieldInfo in _fieldsWithCheckForNullAttribute)
            {
                var checkForNullAttribute = fieldInfo.GetCustomAttribute<CheckForNullAttribute>();
                if (checkForNullAttribute != null)
                {
                    var value = (Object)fieldInfo.GetValue(Clip);
                    if (value == null)
                    {
                        hasNull = true;
                        break;
                    }
                }
            }
            
            return hasNull;
        }
    }
}