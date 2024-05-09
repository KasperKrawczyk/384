using UnityEngine;

[CreateAssetMenu(fileName = "MonsterBaseInfo", menuName = "MonsterBaseInfo")]
public class MonsterBaseInfo : ScriptableObject
{
    public GameObject corpsePrefab;
    public new string name;
    public string description;
    public bool pushable;
}
