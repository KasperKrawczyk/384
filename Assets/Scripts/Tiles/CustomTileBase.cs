using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CustomTileBase : Tile
{
    public Vector3Int position;
    public List<GameObject> objectsOnTile = new List<GameObject>();

    public void Initialize(Vector3Int pos)
    {
        position = pos;
    }

    public virtual void CopyFrom(Tile originalTile)
    {
        // Copy properties from the original Tile
        this.sprite = originalTile.sprite;
        this.color = originalTile.color;
        this.transform = originalTile.transform;
        this.gameObject = originalTile.gameObject;
        this.flags = originalTile.flags;
        this.colliderType = originalTile.colliderType;
    }
}
