using System.Collections.Generic;
using UnityEngine;

public class TileReservationManager : MonoBehaviour {
    
    public static TileReservationManager Instance { get; private set; }

    private Dictionary<Vector2Int, Queue<GameObject>> _reservationQueues = new Dictionary<Vector2Int, Queue<GameObject>>();
    private Dictionary<Vector2Int, List<GameObject>> _tileObjects = new Dictionary<Vector2Int, List<GameObject>>();
    [SerializeField] private Grid _grid;
    
    void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public bool ReserveTile(Vector2Int tile, GameObject reservingObject) {
        if (!_reservationQueues.ContainsKey(tile)) {
            _reservationQueues[tile] = new Queue<GameObject>();
        }

        var queue = _reservationQueues[tile];

        // Check if the reservingObject is already in the queue or if it's at the front of the queue
        if (queue.Contains(reservingObject) || (queue.Count > 0 && queue.Peek() == reservingObject)) {
            // Monster is already in the queue or has the reservation
            return true;
        }

        // Add to the queue
        queue.Enqueue(reservingObject);

        // Only grant reservation if it's the first in the queue
        return queue.Peek() == reservingObject;
    }

    public void ReleaseTile(Vector2Int tile, GameObject reservingObject) {
        if (_reservationQueues.ContainsKey(tile)) {
            var queue = _reservationQueues[tile];
            if (queue.Count > 0 && queue.Peek() == reservingObject) {
                // Remove the reservingObject from the queue
                queue.Dequeue();

                // Optionally, notify the next in line that the tile is now available
                if (queue.Count > 0) {
                    GameObject nextMonster = queue.Peek();
                    // Implement notification logic here, e.g., a callback or event
                    // nextMonster.GetComponent<YourMonsterScript>().NotifyTileAvailable(tile);
                }
            }
        }
    }

    public bool IsTileReserved(Vector2Int tile) {
        return _reservationQueues.ContainsKey(tile) && _reservationQueues[tile].Count > 0;
    }

    public GameObject GetReservedBy(Vector2Int tile) {
        if (_reservationQueues.ContainsKey(tile) && _reservationQueues[tile].Count > 0) {
            return _reservationQueues[tile].Peek();
        }
        return null;
    }
    public void AddObjectToTile(GameObject obj, Vector2Int tileCoordinates)
    {
        if (!_tileObjects.ContainsKey(tileCoordinates))
        {
            _tileObjects[tileCoordinates] = new List<GameObject>();
        }

        // Add the new item to the tile
        _tileObjects[tileCoordinates].Add(obj);
        obj.transform.position = CellToWorld(new Vector3Int(tileCoordinates.x, tileCoordinates.y, 0)) + new Vector3(0.5f, 0.5f, 0);
        obj.transform.SetParent(null); // Detach from any parent to avoid hierarchy issues

        // Ensure the object is visible and not affected by UI layers
        SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            // Set the sorting order to be above all other items on the same tile
            int maxSortingOrder = 0;
            foreach (GameObject existingObj in _tileObjects[tileCoordinates])
            {
                if (existingObj != obj) // Skip the newly added object itself
                {
                    SpriteRenderer existingSpriteRenderer = existingObj.GetComponent<SpriteRenderer>();
                    if (existingSpriteRenderer != null && existingSpriteRenderer.sortingOrder > maxSortingOrder)
                    {
                        maxSortingOrder = existingSpriteRenderer.sortingOrder;
                    }
                }
            }
            spriteRenderer.sortingLayerName = "Items"; // Set to the appropriate sorting layer
            spriteRenderer.sortingOrder = maxSortingOrder + 1; // Place on top
        }
    }

    public void RemoveObjectFromTile(GameObject obj, Vector2Int tileCoordinates)
    {
        if (_tileObjects.ContainsKey(tileCoordinates))
        {
            _tileObjects[tileCoordinates].Remove(obj);
            if (_tileObjects[tileCoordinates].Count == 0) _tileObjects[tileCoordinates] = null;
        }
    }

    public void MoveObject(GameObject obj, Vector2Int oldTileCoordinates, Vector2Int newTileCoordinates)
    {
        RemoveObjectFromTile(obj, oldTileCoordinates);
        AddObjectToTile(obj, newTileCoordinates);
    }

    private void SortObjectsOnTile(Vector2Int tileCoordinates)
    {
        // if (_tileObjects.ContainsKey(tileCoordinates))
        // {
        //     _tileObjects[tileCoordinates].Sort((a, b) =>
        //     {
        //         var aBaseInfo = a.GetComponent<BaseInfo>();
        //         var bBaseInfo = b.GetComponent<BaseInfo>();
        //
        //         bool aAlwaysOnTop = aBaseInfo.GetBoolStat(BoolStatInfoType.AlwaysOnTop);
        //         bool bAlwaysOnTop = bBaseInfo.GetBoolStat(BoolStatInfoType.AlwaysOnTop);
        //         bool aAlwaysOnBottom = aBaseInfo.GetBoolStat(BoolStatInfoType.AlwaysOnBottom);
        //         bool bAlwaysOnBottom = bBaseInfo.GetBoolStat(BoolStatInfoType.AlwaysOnBottom);
        //
        //         if (aAlwaysOnTop && !bAlwaysOnTop) return -1;
        //         if (!aAlwaysOnTop && bAlwaysOnTop) return 1;
        //         if (aAlwaysOnBottom && !bAlwaysOnBottom) return 1;
        //         if (!aAlwaysOnBottom && bAlwaysOnBottom) return -1;
        //
        //         return 0;
        //     });
        // }
    }

    private void UpdateObjectPositions(Vector2Int tileCoordinates)
    {
        if (_tileObjects.ContainsKey(tileCoordinates))
        {
            foreach (var obj in _tileObjects[tileCoordinates])
            {
                obj.transform.position = new Vector3(tileCoordinates.x + 0.5f, tileCoordinates.y + 0.5f, obj.transform.position.z);
            }
        }
    }
    public Vector3Int WorldToCell(Vector3 worldPosition)
    {
        return _grid.WorldToCell(worldPosition);
    }

    public Vector3 CellToWorld(Vector3Int cellPosition)
    {
        return _grid.CellToWorld(cellPosition);
    }
    // public bool IsTileBlockingPath(Vector2Int tileCoordinates)
    // {
    //     if (_tileObjects.ContainsKey(tileCoordinates))
    //     {
    //         foreach (var obj in _tileObjects[tileCoordinates])
    //         {
    //             if (obj.GetComponent<BaseInfo>().GetStat("BlocksPath"))
    //             {
    //                 return true;
    //             }
    //         }
    //     }
    //     return false;
    // }
}
