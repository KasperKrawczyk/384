using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using System.Collections.Generic;

public class TilemapConverter : MonoBehaviour
{
    public Tilemap tilemap;
    public CustomRuleTile customRuleTilePrefab;
    public CustomRandomTile customRandomTilePrefab;
    public CustomTileBase customTilePrefab;

    [ContextMenu("Convert Tilemap to Custom Tiles")]
    public void ConvertTilemap()
    {
        if (tilemap == null || customRuleTilePrefab == null || customRandomTilePrefab == null || customTilePrefab == null)
        {
            Debug.LogError("Tilemap or Custom Tile Prefabs not assigned.");
            return;
        }

        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);
        Dictionary<TileBase, TileBase> tileConversionMap = new Dictionary<TileBase, TileBase>();

        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x + bounds.xMin, y + bounds.yMin, 0);
                TileBase originalTile = tilemap.GetTile(tilePosition);

                if (originalTile != null && !tileConversionMap.ContainsKey(originalTile))
                {
                    TileBase customTile = null;

                    if (originalTile is RuleTile)
                    {
                        CustomRuleTile customRuleTile = ScriptableObject.CreateInstance<CustomRuleTile>();
                        customRuleTile.Initialize(tilePosition);
                        customRuleTile.CopyFrom((RuleTile)originalTile);
                        customTile = customRuleTile;
                    }
                    else if (originalTile is RandomTile)
                    {
                        CustomRandomTile customRandomTile = ScriptableObject.CreateInstance<CustomRandomTile>();
                        customRandomTile.Initialize(tilePosition);
                        customRandomTile.CopyFrom((RandomTile)originalTile);
                        customTile = customRandomTile;
                    }
                    else if (originalTile is Tile)
                    {
                        CustomTileBase customTileBase = ScriptableObject.CreateInstance<CustomTileBase>();
                        customTileBase.Initialize(tilePosition);
                        customTileBase.CopyFrom((Tile)originalTile);
                        customTile = customTileBase;
                    }

                    if (customTile != null)
                    {
                        tileConversionMap[originalTile] = customTile;
                    }
                }

                if (tileConversionMap.ContainsKey(originalTile))
                {
                    tilemap.SetTile(tilePosition, tileConversionMap[originalTile]);
                }
            }
        }

        Debug.Log("Tilemap conversion completed.");
    }
}
