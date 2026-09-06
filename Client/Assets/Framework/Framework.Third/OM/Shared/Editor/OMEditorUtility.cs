using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace OM.Editor
{
    public static class OMEditorUtility
    {
        public static VisualTreeAsset GetBaseEditorTree()
        {
            return Resources.Load<VisualTreeAsset>("OM Base");
        }
        
        public static StyleSheet GetBaseEditorStyleSheet()
        {
            return Resources.Load<StyleSheet>("OM Base");
        }

        public static void DrawAllProperties(this SerializedProperty property,bool enterChildren = true)
        {
            if (property == null)
            {
                EditorGUILayout.HelpBox("SerializedProperty is null", MessageType.Error);
                return;
            }

            EditorGUI.indentLevel++;

            var currentProperty = property.Copy();
            var startDepth = currentProperty.depth;

            while (currentProperty.NextVisible(enterChildren) && currentProperty.depth > startDepth)
            {
                enterChildren = false;
                if (currentProperty.name == "m_Script")
                {
                    continue;
                }
                EditorGUILayout.PropertyField(currentProperty, true);
            }

            EditorGUI.indentLevel--;
        }
        
        public static void DrawAllProperties(this SerializedProperty property,Rect position,bool enterChildren = true)
        {
            if (property == null)
            {
                EditorGUI.HelpBox(position,"SerializedProperty is null", MessageType.Error);
                return;
            }

            EditorGUI.indentLevel++;

            var currentProperty = property.Copy();
            var startDepth = currentProperty.depth;
            
            while (currentProperty.NextVisible(enterChildren) && currentProperty.depth > startDepth)
            {
                enterChildren = false;
                if (currentProperty.name == "m_Script")
                {
                    continue;
                }
                EditorGUI.PropertyField(position,currentProperty, true);
                position.y += EditorGUI.GetPropertyHeight(currentProperty, true);
            }

            EditorGUI.indentLevel--;
        }

        public static IEnumerable<SerializedProperty> GetAllProperties(this SerializedProperty property,
            bool enterChildren = true)
        {
            if (property == null)
            {
                yield break;
            }

            var currentProperty = property.Copy();
            var startDepth = currentProperty.depth;

            while (currentProperty.NextVisible(enterChildren) && currentProperty.depth > startDepth)
            {
                enterChildren = false;
                yield return currentProperty;
            }
        }
        
        public static IEnumerable<SerializedProperty> GetAllPropertiesFrom(SerializedProperty property,bool enterChildren = true)
        {
            if (property == null)
            {
                yield break;
            }

            var currentProperty = property.Copy();
            var startDepth = currentProperty.depth;

            while (currentProperty.NextVisible(enterChildren) && currentProperty.depth > startDepth)
            {
                enterChildren = false;
                yield return currentProperty;
            }
        }
        
        public static IEnumerable<SerializedProperty> GetAllProperties(this SerializedObject serializedObject,bool enterChildren = true)
        {
            if (serializedObject == null)
            {
                yield break;
            }

            var currentProperty = serializedObject.GetIterator();
            var startDepth = currentProperty.depth;

            while (currentProperty.NextVisible(enterChildren) && currentProperty.depth > startDepth)
            {
                enterChildren = false;
                yield return currentProperty;
            }
        }
        
        public static float GetHeightOfAllProperties(this SerializedProperty property,bool enterChildren = true)
        {
            if (property == null)
            {
                return 20;
            }
            float height = 20;
            var currentProperty = property.Copy();
            var startDepth = currentProperty.depth;

            while (currentProperty.NextVisible(enterChildren) && currentProperty.depth > startDepth)
            {
                enterChildren = false;
                if (currentProperty.name == "m_Script")
                {
                    continue;
                }
                height += EditorGUI.GetPropertyHeight(currentProperty, true);
            }
            return height;
        }
        
        public static void DrawAllFieldsInSerializedObject(VisualElement root,SerializedObject serializedObject,Action onChanged = null)
        {
            var imguiContainer = new IMGUIContainer(() =>
            {
                if(serializedObject.targetObject == null) return;
                
                serializedObject.Update();
                serializedObject.GetIterator().DrawAllProperties();
                if (GUI.changed)
                {
                    onChanged?.Invoke();
                }
                serializedObject.ApplyModifiedProperties();
            });
            imguiContainer.style.marginTop = 5;
            imguiContainer.style.marginBottom = 5;
            root.Add(imguiContainer);
        }
        
        public static void DrawAllFields(this SerializedObject serializedObject,VisualElement root,Action onChanged = null)
        {
            var imguiContainer = new IMGUIContainer(() =>
            {
                if(serializedObject.targetObject == null) return;
                
                serializedObject.Update();
                serializedObject.GetIterator().DrawAllProperties();
                if (GUI.changed)
                {
                    onChanged?.Invoke();
                }
                serializedObject.ApplyModifiedProperties();
            });
            imguiContainer.style.marginTop = 5;
            imguiContainer.style.marginBottom = 5;
            imguiContainer.style.marginLeft = -5;
            root.Add(imguiContainer);
        }
        
        public static bool TryGetOMTitle(Object target,out string title)
        {
            var type = target.GetType();
            var titleAttribute = type.GetCustomAttribute<OMTitleAttribute>();
            if (titleAttribute != null)
            {
                title = titleAttribute.Title;
                return true;
            }
            title = target.GetType().Name;
            return false;
        }
        
        public static void DrawInGrid(VisualElement root,float windowWidth,int count,Action<int> itemCallback)
        {
            root.Add(new IMGUIContainer(() =>
            {
                const float buttonWidth = 60;
                var buttonsCountInRow = (int)windowWidth / (int)buttonWidth;

                GUILayout.BeginHorizontal();

                for (var i = 0; i < count; i++)
                {
                        
                    if (i % buttonsCountInRow == 0)
                    {
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                    }
                    
                    itemCallback?.Invoke(i);
                }
                
                GUILayout.EndHorizontal();
            }));
        }
     
        public static void DrawAllObjectsInSceneInGrid<T>(this EditorWindow editorWindow,ref IMGUIContainer container,float buttonWidth,Action<T> itemCreate) where T : MonoBehaviour
        {
            container.onGUIHandler += () =>
            {
                var buttonsCountInRow = (int)editorWindow.position.width / (int)buttonWidth;
                var objects = Object.FindObjectsOfType<T>();
                
                GUILayout.BeginHorizontal();

                for (var i = 0; i < objects.Length; i++)
                {
                    if (i % buttonsCountInRow == 0)
                    {
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                    }
                    
                    itemCreate?.Invoke(objects[i]);
                }
                
                GUILayout.EndHorizontal();
            };
        }
        
        public static void DrawObjectsInGrid<T>(this EditorWindow editorWindow,ref IMGUIContainer container,float buttonWidth,Func<T[]> getObjects,Action<T> itemCreate)
        {
            container.onGUIHandler += () =>
            {
                var buttonsCountInRow = (int)editorWindow.position.width / (int)buttonWidth;
                
                GUILayout.BeginHorizontal();

                var objects = getObjects.Invoke();
                if (objects == null) return;
                for (var i = 0; i < objects.Length; i++)
                {
                    if (i % buttonsCountInRow == 0)
                    {
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                    }
                    
                    itemCreate?.Invoke(objects[i]);
                }
                
                GUILayout.EndHorizontal();
            };
        }

        
    }
}