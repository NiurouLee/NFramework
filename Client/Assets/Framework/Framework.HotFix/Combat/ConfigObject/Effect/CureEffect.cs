using Sirenix.OdinInspector;

namespace NFramework.ModuleSystem.Combat
{
    [Effect("治疗英雄", 20)]
    public class CureEffect : Effect
    {
        public override string Label => "治疗英雄";

        [ToggleGroup("Enabled"), LabelText("取值")]
        public string CureValueFormula;

    }
}