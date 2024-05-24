using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Serialization;

public class StatsManager : MonoBehaviour
{

    public static HashSet<IntStatInfoType> NonSkills = new HashSet<IntStatInfoType>()
    {
        IntStatInfoType.Health,
        IntStatInfoType.Experience,
        IntStatInfoType.Capacity,
        IntStatInfoType.Mana,
        IntStatInfoType.MoveSpeed,
        IntStatInfoType.RunsAt,
        IntStatInfoType.DetectionRadius,
        IntStatInfoType.ReturnRadius,
        IntStatInfoType.MinPursuitDistance,
        IntStatInfoType.ShootingDistanceMax,
        IntStatInfoType.ShootingDistanceMin,
        IntStatInfoType.MinRangedDamage,
        IntStatInfoType.MaxRangedDamage,
        IntStatInfoType.MinMeleeDamage,
        IntStatInfoType.MaxMeleeDamage,
        IntStatInfoType.Armour,
        IntStatInfoType.Attack,
        IntStatInfoType.Defence,
    };
    

    public SerializedDictionary<IntStatInfoType, int> skillTypeToSkillLevel;
    public SerializedDictionary<IntStatInfoType, int> skillTypeToSkillProgression;
    public SerializedDictionary<IntStatInfoType, int> skillTypeToSkillNextLevel;

    private void Awake()
    {
        LoadSkillData();
        InitialiseSkillTypeToSkillNextLevel();
    }

    public void UpdateSkill(IntStatInfoType skillType, int skillPointsToAdd)
    {
        int curPoints = skillTypeToSkillProgression[skillType] += skillPointsToAdd;
        if (curPoints >= skillTypeToSkillNextLevel[skillType])
        {
            int curSkillLevel = skillTypeToSkillLevel[skillType];
            skillTypeToSkillLevel[skillType]++;
            int skillNextLevel = 0;
            if (skillType == IntStatInfoType.Experience)
            {
                skillNextLevel = SkillMathUtils.GetNextLevelExp(curSkillLevel);
            }
            else
            {
                skillNextLevel = SkillMathUtils.GetNextLevelSkill(skillType, SkillMathUtils.KnightConst, curSkillLevel);
            }
            skillTypeToSkillProgression[skillType] = curPoints - skillTypeToSkillNextLevel[skillType];
            skillTypeToSkillLevel[skillType] = skillNextLevel;
        }
        
    }

    public int GetSkillLevel(IntStatInfoType skillType)
    {
        return skillTypeToSkillLevel[skillType];
    }

    public float GetProgressionPercentage(IntStatInfoType skillType)
    {
        return (float) skillTypeToSkillProgression[skillType] / skillTypeToSkillNextLevel[skillType];
    }

    private void InitialiseSkillTypeToSkillNextLevel()
    {
        foreach (IntStatInfoType isit in skillTypeToSkillLevel.Keys)
        {
            if (NonSkills.Contains(isit))
            {
                continue;
            }
            if (isit == IntStatInfoType.Level)
            {
                skillTypeToSkillNextLevel[isit] = SkillMathUtils.GetNextLevelExp(skillTypeToSkillLevel[isit]);
            }
            else
            {
                skillTypeToSkillNextLevel[isit] =
                    SkillMathUtils.GetNextLevelSkill(isit, SkillMathUtils.KnightConst, skillTypeToSkillLevel[isit]);
            }
        }
    }
    
    private void LoadSkillData()
    {
        TextAsset jsonData = Resources.Load<TextAsset>("skillData");  // Assuming JSON is in Resources/skillData.json
        SkillData loadedData = JsonUtility.FromJson<SkillData>(jsonData.text);

        skillTypeToSkillLevel = ConvertToDictionary(loadedData.skillLevels);
        skillTypeToSkillProgression = ConvertToDictionary(loadedData.skillProgressions);
    }

    private SerializedDictionary<IntStatInfoType, int> ConvertToDictionary(SkillDataEntry[] entries)
    {
        SerializedDictionary<IntStatInfoType, int> dict = new SerializedDictionary<IntStatInfoType, int>();
        foreach (var entry in entries)
        {
            Enum.TryParse(entry.skillType, out IntStatInfoType skillTypeKey);
            dict.Add(skillTypeKey, entry.value);
        }
        return dict;
    }


}

[System.Serializable]
public class SkillDataEntry
{
    public string skillType;
    public int value;
}

[System.Serializable]
public class SkillData
{
    public SkillDataEntry[] skillLevels;
    public SkillDataEntry[] skillProgressions;
    public SkillDataEntry[] skillNextLevels;
}


// public void AddExperiencePoints(int expPointsToAdd)
// {
//     curExperiencePoints += expPointsToAdd;
// }