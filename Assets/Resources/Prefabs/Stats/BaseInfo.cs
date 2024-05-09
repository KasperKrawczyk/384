using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ItemBaseInfo", menuName = "ItemBaseInfo")]
public class BaseInfo : ScriptableObject
{
    public ItemType itemType;
    public string itemName;
    public string description;
    public bool isStackable;
    public float weight;
    public Sprite sprite;

}

public enum ItemType
{
    Body,
    Boots,
    Shield,
    Weapon
}