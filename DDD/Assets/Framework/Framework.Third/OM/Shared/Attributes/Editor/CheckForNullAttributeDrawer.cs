using UnityEditor;
using UnityEngine;

namespace OM.Editor
{
    [CustomPropertyDrawer(typeof(CheckForNullAttribute))]
    public class CheckForNullAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var propertyRect = new Rect(position)
            {
                height = EditorGUIUtility.singleLineHeight
            };

            EditorGUI.PropertyField(propertyRect, property, label);
            
            if (property.objectReferenceValue == null)
            {
                var rect = new Rect(position);
                rect.y += EditorGUIUtility.singleLineHeight;
                rect.height = EditorGUIUtility.singleLineHeight * 2;
                
                EditorGUI.HelpBox(rect, "Reference is null", MessageType.Error);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = EditorGUIUtility.singleLineHeight;
            if(property.objectReferenceValue == null)
                height += EditorGUIUtility.singleLineHeight * 2;
            return height;
        }
    }
}