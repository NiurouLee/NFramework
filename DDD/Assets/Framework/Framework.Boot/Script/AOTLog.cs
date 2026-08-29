using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NFramework.Boot
{
    public static class AOTLog
    {
        public static void Info(string message)
        {
            Debug.Log(message);
        }

        public static void Error(string message)
        {
            Debug.LogError(message);
        }

    }
}
