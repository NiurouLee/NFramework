using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
#endif

namespace OM.AC
{
    /// <summary>
    /// The Animation Creator Settings
    /// </summary>
    public class ACSettings : ScriptableObject
    {
        private const string SettingsPath = "Assets/Plugins/OM/AC/Editor/Resources/ACSettings.asset";

        [SerializeField] private bool snapping = true;
        [SerializeField] private Color[] colors;

        public bool Snapping => snapping;

        /// <summary>
        /// Get Random Color From the Settings Asset
        /// </summary>
        /// <returns></returns>
        public Color GetRandomColor()
        {
            if (colors == null || colors.Length == 0)
            {
                return Color.gray;
            }

            return colors[Random.Range(0, colors.Length)];
        }
        

#if UNITY_EDITOR
        
        public static ACSettings GetOrCreateSettings()
        {
            var settings = Resources.Load<ACSettings>("ACSettings");
            if (settings == null)
            {
                settings = CreateInstance<ACSettings>();
                AssetDatabase.CreateAsset(settings, SettingsPath);
                AssetDatabase.SaveAssets();
            }
            return settings;
        }
        
        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }
#endif
    }
    
    #if UNITY_EDITOR

    internal static class ACSettingsUIElementsRegister
    {
        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {
            var provider = new SettingsProvider("Project/AC Settings", SettingsScope.Project)
            {
                label = "Animation Creator Settings",
                activateHandler = (searchContext, rootElement) =>
                {
                    var settings = ACSettings.GetSerializedSettings();
                    var title = new Label()
                    {
                        text = "Animation Creator Timeline Settings"
                    };
                    title.style.unityFontStyleAndWeight = FontStyle.Bold;
                    title.style.unityTextAlign = TextAnchor.MiddleCenter;
                    title.style.paddingTop = 10;
                    
                    title.AddToClassList("title");
                    rootElement.Add(title);

                    var properties = new VisualElement()
                    {
                        style =
                        {
                            flexDirection = FlexDirection.Column
                        }
                    };
                    properties.AddToClassList("property-list");
                    properties.style.paddingTop = 10;
                    properties.style.paddingRight = 10;
                    properties.style.paddingLeft = 10;

                    var colorsProp = settings.FindProperty("colors");
                    var propertyField = new PropertyField(colorsProp, "Colors");
                    propertyField.Bind(settings);
                    properties.Add(propertyField);
                    
                    var snappingProp = settings.FindProperty("snapping");
                    propertyField = new PropertyField(snappingProp, "Snapping");
                    propertyField.Bind(settings);
                    properties.Add(propertyField);
                    
                    rootElement.Add(properties);
                },

                // Populate the search keywords to enable smart search filtering and label highlighting:
                keywords = new HashSet<string>(new[] { "Colors" })
            };

            return provider;
        }
    }
    
    #endif
}