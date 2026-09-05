using UnityEditor;
using UnityEngine;

namespace OM.AC.Editor
{
    [CustomPropertyDrawer(typeof(ACClipInitializer<>),true)]
    public class ACClipInitializerPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var useInitializer = property.FindPropertyRelative("enabled");
            var initialValue = property.FindPropertyRelative("value");
            
            position.width -= 20;
            position.x += 20;
            
            var wideMode = EditorGUIUtility.wideMode;
            EditorGUIUtility.wideMode = true;

            Rect labelRect = new Rect(position);
            labelRect.width = 20;
            labelRect.x -= 20;
            
            
            EditorGUI.BeginChangeCheck();
            var use = EditorGUI.ToggleLeft(labelRect, new GUIContent(""), useInitializer.boolValue);
            
            if (EditorGUI.EndChangeCheck())
            {
                useInitializer.boolValue = use;
            }
            
            GUI.enabled = use;
            EditorGUI.PropertyField(position, initialValue,label);
            GUI.enabled = true;
            
            EditorGUIUtility.wideMode = wideMode;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 1;
        }
    }
}