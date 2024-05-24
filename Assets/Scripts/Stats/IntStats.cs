using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName = "IntStats", menuName = "IntStats")]
public class IntStats : ScriptableObject
{
    public SerializedDictionary<IntStatInfoType, int> statInfo = new SerializedDictionary<IntStatInfoType, int>();

    public int GetStat(IntStatInfoType intStatInfoType)
    {
        if (statInfo.ContainsKey(intStatInfoType)) {
            return statInfo[intStatInfoType];
        }
        Debug.Log($"No stat value found for {intStatInfoType} on {this.name}");
        return 0;
    }
}




