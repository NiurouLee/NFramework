using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace OM.AC.Editor
{
    /// <summary>
    /// The Search Window for the ACAnimator
    /// </summary>
    public class ACAnimatorSearchWindow : ScriptableObject,ISearchWindowProvider
    {
        private ACTimeline _timeline;
        private Texture2D _indentationIcon;

        public void Initialize(ACTimeline timeline)
        {
            _timeline = timeline;
            
            _indentationIcon = new Texture2D(1, 1);
            _indentationIcon.SetPixel(0,0,Color.clear);
            _indentationIcon.Apply();
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var searchTreeEntries = new List<SearchTreeEntry>()
            {
                new SearchTreeGroupEntry(new GUIContent("Create"),0),
            };

            
            var objectsByPath = new Dictionary<string, List<ACClip>>();

            foreach (var obj in ACAnimatorEditor.AllClipsInProject)
            {
                var attribute = obj.GetType().GetCustomAttribute<ACClipCreateAttribute>();
                if(attribute == null) continue;
                string[] pathSegments = attribute.ClipMenuName.Split('/');
                pathSegments = pathSegments.Take(pathSegments.Length - 1).ToArray();
                if (pathSegments.Length == 0)
                {
                    if (objectsByPath.ContainsKey("Custom"))
                    {
                        objectsByPath["Custom"].Add(obj);
                    }
                    else
                    {
                        objectsByPath["Custom"] = new List<ACClip>() {obj};
                    }
                    continue;
                }
                string currentPath = "";
                foreach (string segment in pathSegments)
                {
                    currentPath += segment;
                
                    if (!objectsByPath.ContainsKey(currentPath))
                    {
                        objectsByPath[currentPath] = new List<ACClip>();
                    }

                    objectsByPath[currentPath].Add(obj);
                    currentPath += "/";
                }
            }
            
            
            foreach (var pair in objectsByPath)
            {
                searchTreeEntries.Add(new SearchTreeGroupEntry(new GUIContent(pair.Key),1));
                foreach (var acClip in pair.Value)
                {
                    var attribute = acClip.GetType().GetCustomAttribute<ACClipCreateAttribute>();
                    var clipName = string.IsNullOrEmpty(attribute.ClipName) ? acClip.GetType().Name : attribute.ClipName;
                    searchTreeEntries.Add(new SearchTreeEntry(new GUIContent(clipName,_indentationIcon))
                    {
                        level = 2,
                        userData = acClip,
                    });
                }
            }
            
            return searchTreeEntries;
        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            var type = (ACClip) searchTreeEntry.userData;
            if (type != null)
            {
                var attribute = type.GetType().GetCustomAttribute<ACClipCreateAttribute>();
                _timeline.CreateNewClip(type.GetType(),attribute.ClipName);
            }
            return true;
        }
        
        
    }
}