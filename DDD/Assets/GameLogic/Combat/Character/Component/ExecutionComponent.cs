
using System;
using System.Collections.Generic;
using NFramework.Module.EntityModule;
using NFramework.Module.ResModule;

namespace NFramework.Module.Combat
{
    public class ExecutionComponent : Entity
    {
        public CombatEntity Combat => GetParent<CombatEntity>();
        public Dictionary<int, ExecutionConfigObject> executionDict = new Dictionary<int, ExecutionConfigObject>();

        public ExecutionConfigObject AttachExecution(int executionID)
        {
            if (GetExecution(executionID) != null)
            {
                return GetExecution(executionID);
            }
            ExecutionConfigObject executionConfigObject = null;//= NFROOT.I.G<ResM>().Load<ExecutionConfigObject>(string.Empty);
            if (executionConfigObject == null)
            {
                return null;
            }

            if (!executionDict.ContainsKey(executionConfigObject.id))
            {
                executionDict.Add(executionConfigObject.id, executionConfigObject);
            }
            return executionConfigObject;
        }

        public ExecutionConfigObject GetExecution(int executionID)
        {
            if (executionDict.ContainsKey(executionID))
            {
                return executionDict[executionID];
            }
            return null;
        }
    }
}