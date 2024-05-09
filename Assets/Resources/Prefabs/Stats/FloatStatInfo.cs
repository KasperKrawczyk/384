[System.Serializable]
public class FloatStatInfo
{
        public FloatStatInfoType floatStatType;
        public float floatStatValue;
        
}
    
public enum FloatStatInfoType
{
        Health,
        Armour,
        Experience,
        MoveSpeed,
        RunsAt,
        DetectionRadius,
        ReturnRadius,
        MinPursuitDistance,
        ShootingDistanceMax,
        ShootingDistanceMin,
        MinRangedDamage,
        MaxRangedDamage,
        MinMeleeDamage,
        MaxMeleeDamage,
        Attack,                 // Weapons
        Defence                 // Weapons
}

