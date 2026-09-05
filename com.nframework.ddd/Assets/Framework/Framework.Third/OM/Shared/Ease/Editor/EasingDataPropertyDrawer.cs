using UnityEditor;
using UnityEngine;

namespace OM.Ease.Editor
{
    [CustomPropertyDrawer(typeof(EaseData),true)]
    public class EasingDataPropertyDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var easingDataTypeProperty = property.FindPropertyRelative("easeDataType");
            var easingTypeProperty = property.FindPropertyRelative("easeType");
            var animationCurveProperty = property.FindPropertyRelative("animationCurve");
            
            var propertyRect = new Rect(position);
            propertyRect.width -= 20;
            var buttonRect = new Rect(propertyRect)
            {
                x = propertyRect.xMax,
                width = 20,
                height = EditorGUIUtility.singleLineHeight,
            };
            propertyRect.height = EditorGUIUtility.singleLineHeight;

            if (GUI.Button(buttonRect,"*"))
            {
                Undo.RecordObject(property.serializedObject.targetObject,"Switch Ease Type");
                easingDataTypeProperty.intValue = easingDataTypeProperty.intValue == 1 ? 0 : 1;
            }

            EditorGUI.PropertyField(propertyRect,
                easingDataTypeProperty.intValue == 0 ? easingTypeProperty : animationCurveProperty,
                new GUIContent(label));
        }
    }
}