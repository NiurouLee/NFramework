using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OM.Editor;
using OM.Shared;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace OM.AC.Editor
{
    [CustomEditor(typeof(ACAnimator),true)]
    public class ACAnimatorEditor : OMBaseEditor
    {
        public static List<ACClip> AllClipsInProject { get; private set; }
        
        public static string CopiedClipJson { get; set; }
        public static Type CopiedClipType { get; set; }
        public static ACAnimatorEditor PreviewInstance { get; private set; }
        
        public ACAnimator Animator { get; private set; }
        public ACTimeline Timeline { get; private set; }
        public ACClip SelectedClip { get; private set; }

        private List<IACUpdateable> _updateables = new List<IACUpdateable>();
        
        private void OnEnable()
        {
            Animator = target as ACAnimator;
            Animator.TimelineTime = 0;
            if(AllClipsInProject == null || AllClipsInProject.Count == 0) AllClipsInProject = GetAllClips();
            SetSelectedClip(null);
            EditorApplication.update += Update;
            Undo.undoRedoPerformed += UndoRedoPerformed;
            EditorApplication.playModeStateChanged += OnplayModeStateChanged;
            EditorApplication.quitting += OnEditorApplicationQuitting;

            Update();
        }

        private void OnDisable()
        {
            SetPreviewInstance(null);
            EditorApplication.update -= Update;
            Undo.undoRedoPerformed -= UndoRedoPerformed;
            EditorApplication.playModeStateChanged -= OnplayModeStateChanged;
            EditorApplication.quitting -= OnEditorApplicationQuitting;
        }
        
        /// <summary>
        /// On Undo Redo Performed
        /// </summary>
        private void UndoRedoPerformed()
        {
            Timeline.ContentSection.DestroyAndInstantiateClips();
        }

        /// <summary>
        /// On Play Mode State Changed
        /// </summary>
        /// <param name="playModeState"></param>
        private void OnplayModeStateChanged(PlayModeStateChange playModeState)
        {
            if (playModeState == PlayModeStateChange.ExitingEditMode)
            {
                if (PreviewInstance != null)
                {
                    SetPreviewInstance(null);
                }
            }
        }
        
        /// <summary>
        /// On the Editor Application Quitting
        /// </summary>
        private void OnEditorApplicationQuitting()
        {
            SetPreviewInstance(null);
        }

        /// <summary>
        /// On Editor Update 
        /// </summary>
        private void Update()
        {
            foreach (var updateable in _updateables)
            {
                updateable.Update();
            }
        }
        
        /// <summary>
        /// Draw The inspector
        /// </summary>
        /// <param name="root"></param>
        protected override void DrawInspector(VisualElement root)
        {
            base.DrawInspector(root);
            root.styleSheets.Add(Resources.Load<StyleSheet>("ACAnimator"));
            DrawFields();
            DrawTimeline();
            DrawInspector();
            DrawButtonsSection();
        }

        /// <summary>
        /// Draw All the Fields of the ACAnimator
        /// </summary>
        private void DrawFields()
        {
            var fullDurationProp = serializedObject.FindProperty("fullDuration");
            if (fullDurationProp != null) Root.Add(new PropertyField(fullDurationProp));
            
            var speedProp = serializedObject.FindProperty("speed");
            if (speedProp != null) Root.Add(new PropertyField(speedProp));
            
            var timeIndependentProp = serializedObject.FindProperty("timeIndependent");
            if (timeIndependentProp != null) Root.Add(new PropertyField(timeIndependentProp));
            
            var playOnEnableProp = serializedObject.FindProperty("playOnEnable");
            if (playOnEnableProp != null) Root.Add(new PropertyField(playOnEnableProp));
            
            var loopProp = serializedObject.FindProperty("loop");
            if (loopProp != null) Root.Add(new PropertyField(loopProp));
        
            var eventsProp = serializedObject.FindProperty("events");
            if (eventsProp != null) Root.Add(new PropertyField(eventsProp));
            
        }

        /// <summary>
        /// Draw the Timeline
        /// </summary>
        private void DrawTimeline()
        {
            Timeline = new ACTimeline(Animator,this);
            _updateables.Add(Timeline);
            Root.Add(Timeline);
        }

        /// <summary>
        /// Draw the Clip Inspector
        /// </summary>
        private void DrawInspector()
        {
            var container = new VisualElement();
            container.AddToClassList("inspector");
            Root.Add(container);
            
            var header = new Label("Inspector");
            container.Add(header);
            
            container.Add(new IMGUIContainer(() =>
            {
                if (GetSelectedClip() == null)
                {
                    return;
                }

                serializedObject.Update();
                var indexOf = Animator.GetClips().IndexOf(GetSelectedClip());
                if (indexOf < 0)
                {
                    return;
                }
                var clipProp = serializedObject.FindProperty("clips").GetArrayElementAtIndex(indexOf);
                var monoScript = FindClass(GetSelectedClip().GetType().Name);
                if (monoScript != null)
                {
                    GUI.enabled = false;
                    EditorGUILayout.ObjectField("Script", monoScript, typeof(MonoScript), false);
                    GUI.enabled = true;
                }

                DrawAllProperties(clipProp);

                var methodInfos = GetSelectedClip().GetType().GetMethods(BindingFlags.Default | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                foreach (var methodInfo in methodInfos)
                {
                    var customButtonAttribute = methodInfo.GetCustomAttribute<OMCustomButtonAttribute>();
                    if (customButtonAttribute != null)
                    {
                        if (GUILayout.Button(customButtonAttribute.ButtonName))
                        {
                            methodInfo.Invoke(GetSelectedClip(), null);
                        }
                    }
                }

                serializedObject.ApplyModifiedProperties();
            }));
        }

        /// <summary>
        /// Draw the Buttons Section
        /// </summary>
        private void DrawButtonsSection()
        {
            var gui = new IMGUIContainer();
            gui.style.marginTop = 10;
            gui.style.paddingRight = 5;
            gui.style.paddingLeft = 5;
            Root.Add(gui);

            gui.onGUIHandler = () =>
            {
                var animatorPlayer = target as ACAnimatorPlayer;
                if (animatorPlayer == null) return;
                GUI.enabled = Application.isPlaying;

                if (animatorPlayer.PlayState == ACAnimatorPlayState.Paused)
                {
                    if (GUILayout.Button("Resume"))
                    {
                        animatorPlayer.Resume();
                    }
                }
                else
                {
                    if (animatorPlayer.PlayState == ACAnimatorPlayState.Playing)
                    {
                        if (GUILayout.Button("Pause"))
                        {
                            animatorPlayer.Pause();
                        }       
                    }
                    else
                    {
                        if (GUILayout.Button("Play"))
                        {
                            animatorPlayer.Play();
                        }      
                    } 
                }
                
                if (GUILayout.Button("Restart"))
                {
                    animatorPlayer.Restart();
                }
                

                GUI.enabled = true;
            };
        }

        public override bool RequiresConstantRepaint()
        {
            return true;
        }

        /// <summary>
        /// Set the selected clip
        /// </summary>
        /// <param name="clip"></param>
        public void SetSelectedClip(ACClip clip)
        {
            SelectedClip = clip;
        }
        
        /// <summary>
        /// Get the selected clip
        /// </summary>
        /// <returns></returns>
        public ACClip GetSelectedClip()
        {
            return SelectedClip;
        }
        
        /// <summary>
        /// Draw all the properties of the SerializedProperty
        /// </summary>
        /// <param name="property"></param>
        /// <param name="enterChildren"></param>
        private void DrawAllProperties(SerializedProperty property,bool enterChildren = true)
        {
            if (property == null)
            {
                EditorGUILayout.HelpBox("SerializedProperty is null", MessageType.Error);
                return;
            }

            var currentProperty = property.Copy();
            var startDepth = currentProperty.depth;
            EditorGUIUtility.labelWidth = 100;
            var currentClipTypeName = GetSelectedClip().GetType().Name;
            while (currentProperty.NextVisible(enterChildren) && currentProperty.depth > startDepth)
            {
                enterChildren = false;
                if (currentProperty.name == "m_Script")
                {
                    continue;
                }
                
                if (currentProperty.name == "fromType")
                {
                    continue;
                }
                
                if (currentProperty.name == "from")
                {
                    var fromTypeProp = property.FindPropertyRelative("fromType");
                    EditorGUILayout.PropertyField(fromTypeProp);
                    
                    if (fromTypeProp.enumValueIndex == 0)
                    {
                        EditorGUILayout.PropertyField(currentProperty, true);
                    }
                    else
                    {
                        GUI.enabled = false;
                        EditorGUILayout.PropertyField(currentProperty, true);
                        GUI.enabled = true;
                    }
                    
                    continue;
                }
                
                EditorGUILayout.PropertyField(currentProperty, true);
            }
        }
        
        /// <summary>
        /// Find the class MonoScript by name
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        static MonoScript FindClass(string className)
        {
            string[] assetsPaths = AssetDatabase.FindAssets(className);

            foreach (string assetPath in assetsPaths)
            {
                string assetFilePath = AssetDatabase.GUIDToAssetPath(assetPath);

                if (assetFilePath.EndsWith(className + ".cs")) // Assuming it's a C# file
                {
                    var loadAssetAtPath = AssetDatabase.LoadAssetAtPath<MonoScript>(assetFilePath);
                    return loadAssetAtPath;
                }
            }
            return null;
        }
        
        /// <summary>
        /// Get All clips in the project
        /// </summary>
        /// <returns></returns>
        private static List<ACClip> GetAllClips()
        {
            var types = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                from assemblyType in domainAssembly.GetTypes()
                where (assemblyType.IsSubclassOf(typeof(ACClip)) && !assemblyType.IsAbstract)
                select assemblyType);

            var uicClips = new List<ACClip>();
            foreach (var type in types)
            {
                uicClips.Add((ACClip)Activator.CreateInstance(type));
            }
            return uicClips;
        }
        
        /// <summary>
        /// Copy the Clip
        /// </summary>
        /// <param name="clip"></param>
        public void CopyClip(ACClip clip)
        {
            if(clip == null) return;
            CopiedClipJson = JsonUtility.ToJson(clip);
            CopiedClipType = clip.GetType();
        }
        
        /// <summary>
        /// Get the Copied Clip and reset the stored clip
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool TryGetClipFromCopy(out ACClip result)
        {
            try
            {
                var clip = JsonUtility.FromJson(CopiedClipJson, CopiedClipType);
                result = clip as ACClip;
                CopiedClipJson = "";
                CopiedClipType = null;
                return result != null;
            }
            catch (Exception)
            {
                result = null;
                return false;
            }
        }
        
        /// <summary>
        /// Set if this instance is the preview instance
        /// </summary>
        /// <param name="instance"></param>
        public void SetPreviewInstance(ACAnimatorEditor instance)
        {
            if (PreviewInstance != null)
            {
                foreach (var clip in PreviewInstance.Animator.GetClips())
                {
                    clip.OnPreviewModeChanged(false);
                }
            }
            PreviewInstance = instance;
            if (PreviewInstance != null)
            {
                foreach (var clip in PreviewInstance.Animator.GetClips())
                {
                    clip.OnPreviewModeChanged(true);
                }
            }
        }
        
        /// <summary>
        /// returns if this instance is the preview instance
        /// </summary>
        /// <returns></returns>
        public bool IsPreviewInstance()
        {
            return PreviewInstance == this;
        }
    }
}