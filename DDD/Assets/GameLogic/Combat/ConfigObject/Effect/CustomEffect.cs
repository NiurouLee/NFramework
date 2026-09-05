using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace NFramework.Module.Combat
{
    [Effect("自定义效果", 1000)]
    public class CustomEffect : Effect
    {
        public override string Label => "自定义效果";

        [ToggleGroup("Enable"), LabelText("自定义效果")]
        public string CustomEffectType;

        [ToggleGroup("Enable"), LabelText("参数列表")]
        public Dictionary<string, string> ParamsDict = new Dictionary<string, string>();
    }
}