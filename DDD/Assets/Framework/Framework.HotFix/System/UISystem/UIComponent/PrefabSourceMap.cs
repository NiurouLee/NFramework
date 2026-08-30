using System.Collections.Generic;
using UnityEngine;

namespace NFramework.ModuleSystem
{
    public class PrefabSourceMap : MonoBehaviour
    {
        public List<GameObject> prefabGameObjectList;

        public virtual GameObject GetObject(int listIndex)
        {
            if (listIndex >= 0 && prefabGameObjectList != null && prefabGameObjectList.Count > 0)
            {
                if (listIndex < prefabGameObjectList.Count)
                {
                    return prefabGameObjectList[listIndex];
                }
            }

            return null;
        }
    }
}