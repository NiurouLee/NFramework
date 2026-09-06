using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace NFramework.ModuleSystem
{
    [SerializeField]
    public class NText : Text
    {
        public string languageKey;

        public static event Func<string, string> GetLanguageStr;

    }
}