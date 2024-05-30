using UnityEngine;

[CreateAssetMenu(fileName = "MonsterBaseInfo", menuName = "MonsterBaseInfo")]
public class MonsterBaseInfo : ScriptableObject
{
    public new string name;
    public string corpseBaseInfoName;
    public string description;
    public bool pushable;
}
