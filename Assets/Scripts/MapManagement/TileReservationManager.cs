using System.Collections.Generic;
using UnityEngine;

public class TileReservationManager : MonoBehaviour {
    
    public static TileReservationManager Instance { get; private set; }

    private Dictionary<Vector2Int, Queue<GameObject>> _reservationQueues = new Dictionary<Vector2Int, Queue<GameObject>>();

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
}
