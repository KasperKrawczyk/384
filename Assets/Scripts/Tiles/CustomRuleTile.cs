using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "NewCustomRuleTile", menuName = "Tiles/CustomRuleTile")]
public class CustomRuleTile : RuleTile
{
    public Vector3Int position;
    public List<GameObject> objectsOnTile = new List<GameObject>();

    public void Initialize(Vector3Int pos)
    {
        position = pos;
    }

    public void CopyFrom(RuleTile originalTile)
    {
        // Copy the properties from the original RuleTile
        this.m_DefaultSprite = originalTile.m_DefaultSprite;
        this.m_DefaultGameObject = originalTile.m_DefaultGameObject;
        this.m_DefaultColliderType = originalTile.m_DefaultColliderType;
        this.m_TilingRules = new List<TilingRule>(originalTile.m_TilingRules);
    }
}