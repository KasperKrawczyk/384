using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileAssetConverter
{
    [MenuItem("Tools/Convert Tile Assets")]
    public static void ConvertTileAssets()
    {
        // Find all assets of type TileBase in the Assets/Tiles directory
        string[] guids = AssetDatabase.FindAssets("t:TileBase", new[] { "Assets/Tiles" });

        foreach (string guid in guids)
        {
            // Get the asset path from the GUID
            string path = AssetDatabase.GUIDToAssetPath(guid);
            // Load the original tile asset
            TileBase originalTile = AssetDatabase.LoadAssetAtPath<TileBase>(path);
            TileBase customTile = null;

            // Check the type of the original tile and create a corresponding custom tile
            if (originalTile is RuleTile)
            {
                CustomRuleTile customRuleTile = ScriptableObject.CreateInstance<CustomRuleTile>();
                customRuleTile.CopyFrom((RuleTile)originalTile);
                customTile = customRuleTile;
            }
            else if (originalTile is RandomTile)
            {
                CustomRandomTile customRandomTile = ScriptableObject.CreateInstance<CustomRandomTile>();
                customRandomTile.CopyFrom((RandomTile)originalTile);
                customTile = customRandomTile;
            }
            else if (originalTile is Tile)
            {
                CustomTileBase customTileBase = ScriptableObject.CreateInstance<CustomTileBase>();
                customTileBase.CopyFrom((Tile)originalTile);
                customTile = customTileBase;
            }

            // If a custom tile was created, save it as a new asset
            if (customTile != null)
            {
                string newPath = path.Replace(".asset", "_Custom.asset");
                AssetDatabase.CreateAsset(customTile, newPath);
                Debug.Log($"Converted {path} to {newPath}");
            }
        }

        // Save and refresh the asset database
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Tile asset conversion completed.");
    }
}
