using System;
using System.Collections.Generic;

public static class SkillMathUtils
{
    public static Dictionary<IntStatInfoType, int> SkillConstants = new Dictionary<IntStatInfoType, int>()
    {
        { IntStatInfoType.Melee, 50},
        { IntStatInfoType.Shielding, 100},
        { IntStatInfoType.Distance, 25},
    };
    public const float KnightConst = 1.1f;

    public static int GetNextLevelExp(int curLevel)
    {
        return (int) (50 * Math.Pow(curLevel, 2) - 150 * curLevel + 200);
    }
    
    public static int GetSExpPointsBetweenCurLevelAndNextLevel(int curLevel)
    {
        return GetNextLevelExp(curLevel) - GetNextLevelExp(curLevel - 1);
    }

    public static int GetNextLevelSkill(IntStatInfoType skillType, float vocConstant, int curSkillLevel)
    {
        return (int) (SkillConstants[skillType] * Math.Pow(vocConstant, curSkillLevel));
    }

    public static int GetSkillPointsBetweenCurLevelAndNextLevel(IntStatInfoType skillType, float vocConstant, int curSkillLevel)
    {
        return GetNextLevelSkill(skillType, vocConstant, curSkillLevel) -
               GetNextLevelSkill(skillType, vocConstant, curSkillLevel - 1);
    }
    
    
}