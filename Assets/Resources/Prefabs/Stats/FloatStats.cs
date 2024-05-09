using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FloatStats", menuName = "FloatStats")]
public class FloatStats : ScriptableObject
{
    public List<FloatStatInfo> statInfo = new List<FloatStatInfo>();
    public Dictionary<FloatStatInfoType, float> stats = new Dictionary<FloatStatInfoType, float>();

    public float GetStat(FloatStatInfoType floatStatInfoType)
    {
        foreach (var fsit in this.statInfo) 
        {
            if (fsit.floatStatType == floatStatInfoType)
            {
                return fsit.floatStatValue;
            }
        }
        Debug.Log($"No stat value found for {floatStatInfoType} on {this.name}");
        return 0;
    }
}




