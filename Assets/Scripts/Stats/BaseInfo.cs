using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ItemBaseInfo", menuName = "ItemBaseInfo")]
public class BaseInfo : ScriptableObject
{
    [FormerlySerializedAs("statInfo")] public SerializedDictionary<IntStatInfoType, int> intStatInfo = new SerializedDictionary<IntStatInfoType, int>();
    public SerializedDictionary<BoolStatInfoType, bool> boolStatInfo = new SerializedDictionary<BoolStatInfoType, bool>();

    
    public ItemCategory ItemCategory;
    public SlotPosition SlotPosition;
    public WeaponType WeaponType;
    public string itemName;
    public string description;
    public string spriteId;
    public string useEffectId;
    public string monsterName;
    
    public int GetIntStat(IntStatInfoType intStatInfoType)
    {
        if (intStatInfo.ContainsKey(intStatInfoType)) {
            return intStatInfo[intStatInfoType];
        }
        Debug.Log($"No stat value found for {intStatInfoType} on {this.name}");
        return 0;
    }
    
    public bool GetBoolStat(BoolStatInfoType boolStatInfoTypeType)
    {
        if (boolStatInfo.ContainsKey(boolStatInfoTypeType)) {
            return boolStatInfo[boolStatInfoTypeType];
        }
        Debug.Log($"No stat value found for {boolStatInfoTypeType} on {this.name}");
        return false;
    }
}



