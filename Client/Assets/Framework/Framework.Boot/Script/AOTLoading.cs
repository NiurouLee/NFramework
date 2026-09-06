using System;
using UnityEngine;

namespace NFramework.Boot
{
    public class AOTLoading : MonoBehaviour
    {
        public static AOTLoading Instace { get; private set; }

        private void Awake()
        {
            Instace = this;
        }
    }
}