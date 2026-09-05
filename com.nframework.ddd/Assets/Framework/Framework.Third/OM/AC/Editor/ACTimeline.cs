using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace OM.AC.Editor
{
    /// <summary>
    /// The Actual Timeline of the ACAnimator
    /// </summary>
    public class ACTimeline : VisualElement,IACUpdateable
    {
        public const float TimelineClipHeight = 30;
        public const float TimelineClipSpace = 5;

        public ACAnimator Animator { get; }
        public ACAnimatorEditor AnimatorEditor { get; }

        public ACTimelineHeaderSection Header {get; private set; }
        public ACTimelineTimeSection TimeSection {get; private set; }
        public ACTimelineContentSection ContentSection {get; private set; }
        public ACTimelineFooterSection Footer {get; private set; }
        public VisualElement TopContainer {get; private set; }
        public ACAnimatorSearchWindow SearchWindow { get; }

        private readonly List<IACUpdateable> _updatables = new List<IACUpdateable>();
        
        public ACTimeline(ACAnimator animator, ACAnimatorEditor animatorEditor)
        {
            Animator = animator;
            AnimatorEditor = animatorEditor;
            styleSheets.Add(Resources.Load<StyleSheet>("ACTimeline"));
            AddToClassList("timeline");
            
            this.AddManipulator(new ACTimelineManipulator(this));
            
            var keys = new Dictionary<KeyCode, Action>
            {
                { KeyCode.Space, () =>
                    {
                        var guiToScreenPoint = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
                        UnityEditor.Experimental.GraphView.SearchWindow.Open(new SearchWindowContext(guiToScreenPoint), SearchWindow);
                    } 
                },
                { KeyCode.Delete, () =>
                    {
                        var clip = AnimatorEditor.GetSelectedClip();
                        if(clip == null) return;
                        ContentSection.DeleteClip(clip);
                    } 
                },
                {KeyCode.C, () =>
                    {
                        var clip = AnimatorEditor.GetSelectedClip();
                        if(clip == null) return;
                        AnimatorEditor.CopyClip(clip);
                    }
                },
                {KeyCode.D, () =>
                    {
                        var clip = AnimatorEditor.GetSelectedClip();
                        if(clip == null) return;
                        ContentSection.DuplicateClip(clip);
                    }
                },
                {KeyCode.V, () =>
                {
                    ContentSection.TryPasteCopiedClip();
                }},
            };
            this.AddManipulator(new TimelineShortcutsManipulator(keys));
            
            focusable = true;

            
            SearchWindow = ScriptableObject.CreateInstance<ACAnimatorSearchWindow>();
            SearchWindow.Initialize(this);
            
            DrawFull();
        }
        
        private void DrawFull()
        {
            DrawTopContainer();
            DrawHeader();
            DrawTimeSection();
            DrawContentSection();
            DrawFooter();
            TopContainer.BringToFront();
        }

        private void DrawHeader()
        {
            Header = new ACTimelineHeaderSection(this);
            _updatables.Add(Header);
            Add(Header);

        }

        private void DrawTimeSection()
        {
            TimeSection = new ACTimelineTimeSection(this);
            _updatables.Add(TimeSection);
            Add(TimeSection);
        }

        private void DrawContentSection()
        {
            ContentSection = new ACTimelineContentSection(this);
            _updatables.Add(ContentSection);
            Add(ContentSection);
            ContentSection.DestroyAndInstantiateClips();
        }

        private void DrawFooter()
        {
            Footer = new ACTimelineFooterSection(this);
            _updatables.Add(Footer);
            Add(Footer);
        }

        private void DrawTopContainer()
        {
            TopContainer = new VisualElement();
            TopContainer.pickingMode = PickingMode.Ignore;
            TopContainer.AddToClassList("top-container");
            Add(TopContainer);
            
            
        }

        public void Update()
        {
            foreach (var updateable in _updatables)
            {
                updateable.Update();
            }
        }
        
        public float GetClipYPosition(int index)
        {
            return index * (ACTimeline.TimelineClipHeight + ACTimeline.TimelineClipSpace) + ACTimeline.TimelineClipSpace * .5f;
        }
        
        public int GetClipIndex(float yPosition)
        {
            return Mathf.FloorToInt((yPosition + ACTimeline.TimelineClipHeight * .5f) / (ACTimeline.TimelineClipHeight + ACTimeline.TimelineClipSpace)) ;
        }
        
        public float GetTimelineWidth()
        {
            return ContentSection.layout.width;
        }
        
        public float GetWidthPerSecond()
        {
            return GetTimelineWidth() / Animator.FullDuration;
        }
        
        public float GetWidthPerSecond(float width)
        {
            return width / Animator.FullDuration;
        }
        
        public float GetSecondsPerWidth()
        {
            return Animator.FullDuration / GetTimelineWidth();
        }
        
        public float GetSecondsPerWidth(float width)
        {
            return Animator.FullDuration / width;
        }

        public void CreateNewClip(Type clipType, string clipName)
        {
            var clipInstance = (ACClip)Activator.CreateInstance(clipType);
            clipInstance.Name = clipName;
            clipInstance.StartAt = 0f;
            Undo.RecordObject(Animator,"Add Clip");
            Animator.AddClip(clipInstance);
            var color = ACSettings.GetOrCreateSettings().GetRandomColor();
            clipInstance.Color = color;
            ContentSection.AddTimelineClip(clipInstance);
            AnimatorEditor.SetSelectedClip(clipInstance);
        }
        
        public void AddClip(ACClip clip)
        {
            Undo.RecordObject(Animator,"Add Clip");
            Animator.AddClip(clip);
            ContentSection.AddTimelineClip(clip);
        }
        
        

    }
}