using System.Runtime.Serialization;
[System.Serializable]
public class StatInfo
{
        public IntStatInfoType intStatType;
        public int intStatValue;
        
}
    
public enum IntStatInfoType
{
        [EnumMember(Value = "Level")]
        Level,
        [EnumMember(Value = "Health")]
        Health,
        [EnumMember(Value = "Mana")]
        Mana,
        [EnumMember(Value = "Capacity")]
        Capacity,
        [EnumMember(Value = "Armour")]
        Armour,
        [EnumMember(Value = "Shielding")]
        Shielding,
        [EnumMember(Value = "Distance")]
        Distance,
        [EnumMember(Value = "Experience")]
        Experience,
        [EnumMember(Value = "Melee")]
        Melee,
        [EnumMember(Value = "MoveSpeed")]
        MoveSpeed,
        [EnumMember(Value = "RunsAt")]
        RunsAt,
        [EnumMember(Value = "DetectionRadius")]
        DetectionRadius,
        [EnumMember(Value = "ReturnRadius")]
        ReturnRadius,
        [EnumMember(Value = "MinPursuitDistance")]
        MinPursuitDistance,
        [EnumMember(Value = "ShootingDistanceMax")]
        ShootingDistanceMax,
        [EnumMember(Value = "ShootingDistanceMin")]
        ShootingDistanceMin,
        [EnumMember(Value = "MinRangedDamage")]
        MinRangedDamage,
        [EnumMember(Value = "MaxRangedDamage")]
        MaxRangedDamage,
        [EnumMember(Value = "MinMeleeDamage")]
        MinMeleeDamage,
        [EnumMember(Value = "MaxMeleeDamage")]
        MaxMeleeDamage,
        [EnumMember(Value = "Attack")]
        Attack,                 // Weapons
        [EnumMember(Value = "Defence")]
        Defence, // Weapons
        MinReqLevel,
        MinReqMagicLevel,
        RuneMagLevel,
        RuneLevel,
        AlwaysOnTopOrder,
        RotateTo,
        Speed,
        MaxItems,
        ExtraAttack,
        ExtraDefence,
        AttackSpeed,
        DecayTo,
        DecayTime,
        StopTime,
        LightLevel,
        LightColour,
        HitChance,
        MaxHitChance,
        BreakChance,
        Charges,
        Weight,
        Hands
        
}

public enum BoolStatInfoType
{
        Stackable,
        Usable,
        AlwaysOnTop,
        LookThrough,
        Pickupable,
        Rotable,
        HasHeight,
        ForceSerialize,
        BlockSolid,
        BlockProjectile,
        BlockPathFind,
        AllowPickupable,
        Moveable,
        AllowDistRead,
        CanReadText,
        CanWriteText,
        ShowDuration,
        ShowCharges,
        ShowAttributes,
        Container,
        Corpse
}

public enum ItemCategory
{
        Armour,
        Amulet,
        Boots,
        Containers,
        Decoration,
        Food,
        Helmet,
        Legs,
        Others,
        Potion,
        Ring,
        Rune,
        Shield,
        Tool,
        Valuable,
        Ammunition,
        Axe,
        Club,
        Distance,
        Sword,
        Wand,
        Scroll,
        Coin,
        CreatureProduct,
        Quiver
        
}


public enum CombatType
{
        None,
    
}

public enum SlotPosition
{
        None,
        Helmet,
        Amulet,
        Backpack,
        Armour,
        Shield,
        Weapon,
        Legs,
        Boots,
        Ring,
        Arrow
    
}

public enum ItemType
{
        Body,
        Boots,
        Sword,
        Axe,
        Mace,
        Throwing,
        Shooting
}

public enum WeaponType
{
        None,
        Sword,
        Club,
        Axe,
        Shield,
        Dist,
        Wand,
        Ammo,
        Fist
}

public enum AmmoType
{
        None,
        Bolt,
        Arrow,
        Spear,
        ThrowingStar,
        ThrowingKnife,
        Stone,
        Snowball
}