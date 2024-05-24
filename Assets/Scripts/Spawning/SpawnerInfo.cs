using UnityEngine;

[CreateAssetMenu(fileName = "SpawnerInfo", menuName = "SpawnerInfo", order = 0)]
public class SpawnerInfo : ScriptableObject
{
    public string monsterPrefabPath;
    public string spawnEffectPrefabPath;
    public int numMonsters;
    public int boxHalfSide;
    public int delay;
}