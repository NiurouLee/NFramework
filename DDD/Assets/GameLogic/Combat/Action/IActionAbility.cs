namespace NFramework.Module.Combat
{
    public interface IActionAbility
    {
        public bool Enable { get; set; }
        public CombatEntity Owner { get;  }
    }
}