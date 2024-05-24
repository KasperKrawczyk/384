using System;

public static class PlayerCombatMathUtils
{
    private static Random _rng = new Random();

    public static int GetMeleeDamage(int level, int skillLevel, int weaponAttack)
    {
        double basse = GetBaseDamage(level);
        double mid = basse + Math.Floor((float) weaponAttack * (skillLevel + 4) / 28);
        int low = (int) (mid * 0.75);
        int high = (int) (mid * 1.25);
        return _rng.Next(low, high);
    }
    
    public static int GetDistanceDamage(int level, int skillLevel, int weaponAttack)
    {
        double minimumDamage = (float) level / 5;
        double maximumDamage = 0.09 * skillLevel * weaponAttack + minimumDamage;
        int low = (int) minimumDamage;
        int high = (int) maximumDamage;
        return _rng.Next(low, high);
    }

    public static double GetBaseDamage(int level)
    {
        double step = Math.Floor((Math.Sqrt(2 * level + 2025) + 5) / 10);
        return Math.Floor((level + 1000) / step - 50 * step) + 100 * step - 450;
    }

    public static int GetDamageSuffered(int damageSent, int totalDefence, int totalArmour, int shieldingSkillLevel)
    {
        double damageReducedByDefence = 0;
        double damageReducedByArmour = 0;
        if (totalDefence > 0)
        {
            damageReducedByDefence = GetDefenceValue(totalDefence, shieldingSkillLevel);
            
        }
        
        double minArmourReduction = Math.Floor((double) totalArmour / 2);
        double maxArmourReduction = minArmourReduction * 2 - 1;
        damageReducedByArmour = _rng.NextDouble() * (maxArmourReduction - minArmourReduction) + minArmourReduction;
        
        return (int) Math.Max(0, damageSent - damageReducedByDefence - damageReducedByArmour);
    }

    private static int GetDefenceValue(int totalDefence, int shieldingSkillLevel)
    {
        return (int) Math.Floor(Math.Ceiling(0.7 * (float) totalDefence) * ((float) shieldingSkillLevel + 10) / 40);
    }
}