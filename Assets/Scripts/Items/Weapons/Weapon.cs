public class Weapon : BaseItem
{
    public FloatStats FloatStats;
    public ActorCombatManager.CombatType CombatType;
    public WeaponType WeaponType;
}

public enum WeaponType
{
    Sword,
    Axe,
    Mace,
    Ranged
}