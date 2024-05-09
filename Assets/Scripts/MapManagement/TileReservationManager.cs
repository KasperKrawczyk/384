using System.Collections.Generic;
using UnityEngine;

public class TileReservationManager : MonoBehaviour {
    
    public static TileReservationManager Instance { get; private set; }

    private Dictionary<Vector2Int, Queue<GameObject>> reservationQueues = new Dictionary<Vector2Int, Queue<GameObject>>();

    void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public bool ReserveTile(Vector2Int tile, GameObject monster) {
        if (!reservationQueues.ContainsKey(tile)) {
            reservationQueues[tile] = new Queue<GameObject>();
        }

        var queue = reservationQueues[tile];

        // Check if the monster is already in the queue or if it's at the front of the queue
        if (queue.Contains(monster) || (queue.Count > 0 && queue.Peek() == monster)) {
            // Monster is already in the queue or has the reservation
            return true;
        }

        // Add to the queue
        queue.Enqueue(monster);

        // Only grant reservation if it's the first in the queue
        return queue.Peek() == monster;
    }

    public void ReleaseTile(Vector2Int tile, GameObject monster) {
        if (reservationQueues.ContainsKey(tile)) {
            var queue = reservationQueues[tile];
            if (queue.Count > 0 && queue.Peek() == monster) {
                // Remove the monster from the queue
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
        return reservationQueues.ContainsKey(tile) && reservationQueues[tile].Count > 0;
    }

    public GameObject GetReservedBy(Vector2Int tile) {
        if (reservationQueues.ContainsKey(tile) && reservationQueues[tile].Count > 0) {
            return reservationQueues[tile].Peek();
        }
        return null;
    }
}
