using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "NewCustomRandomTile", menuName = "Tiles/CustomRandomTile")]
public class CustomRandomTile : RandomTile
{
    public Vector3Int position;
    public List<GameObject> objectsOnTile = new List<GameObject>();

    public void Initialize(Vector3Int pos)
    {
        position = pos;
    }

    public void CopyFrom(RandomTile originalTile)
    {
        // Copy the properties from the original RandomTile
        this.m_Sprites = originalTile.m_Sprites;
        this.color = originalTile.color;
        this.colliderType = originalTile.colliderType;
        this.flags = originalTile.flags;
        this.gameObject = originalTile.gameObject;
        this.sprite = originalTile.sprite;
        this.transform = originalTile.transform;
    }
}