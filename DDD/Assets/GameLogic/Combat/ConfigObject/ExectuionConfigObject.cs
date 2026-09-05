using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NFramework.Module.Combat
{
    [CreateAssetMenu(fileName = "ExecutionConfigObject", menuName = "技能|状态/Execution")]
    public class ExecutionConfigObject : ScriptableObject
    {
        public int id;
        public ExecutionTargetInputType TargetInputType;

        [Space(10)]
        [ListDrawerSettings(DraggableItems = false, ShowItemCount = false, CustomAddFunction = "AddExecutionClipData")]
        public List<ExecuteClipData> ExecuteClipDataList = new List<ExecuteClipData>();

        public float TotalTime
        {
            get
            {
                float temp = 0;
                foreach (var item in ExecuteClipDataList)
                {
                    temp += item.Duration;
                }
                return temp;
            }
        }
        private void AddExecutionClipData()
        {
            var obj = CreateInstance<ExecuteClipData>();
            obj.name = "ExecuteClipData";
            obj.ExecuteClipType = ExecuteClipType.CollisionExecute;
            obj.CollisionExecuteData = new CollisionExecuteData();
            obj.GetClipTime().EndTime = 0.1f;
            ExecuteClipDataList.Add(obj);
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.AddObjectToAsset(obj, this);
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssetIfDirty(this);
#endif
        }

#if UNITY_EDITOR
        #region  自动命名
        [OnInspectorGUI]
        private void OnInspectorGUI()
        {
            if (UnityEditor.Selection.assetGUIDs.Length == 1)
            {
                string guid = UnityEditor.Selection.assetGUIDs[0];
                string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                var config = UnityEditor.AssetDatabase.LoadAssetAtPath<ExecutionConfigObject>(assetPath);
                if (config != this)
                {
                    return;
                }

                var oldName = Path.GetFileNameWithoutExtension(assetPath);
                var newName = $"Execution_{this.id}";
                if (oldName != newName)
                {
                    UnityEditor.AssetDatabase.RenameAsset(assetPath, newName);
                }
            }
        }
        #endregion
#endif



    }
}