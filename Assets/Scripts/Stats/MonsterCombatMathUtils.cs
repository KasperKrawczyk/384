using System;

public static class MonsterCombatMathUtils
{
    private static Random _rng = new Random();

    public static int GetDamageSuffered(int damageSent, int armour)
    {
        double minArmourReduction = 1d;
        double maxArmourReduction = 1d;
        if (armour > 1)
        {
            minArmourReduction = (armour * 0.475);
            maxArmourReduction = (minArmourReduction - 1) + minArmourReduction;    
        }
        
        return Math.Max(0, damageSent - (int) (_rng.NextDouble() * (maxArmourReduction - minArmourReduction) + minArmourReduction));
    }
}
