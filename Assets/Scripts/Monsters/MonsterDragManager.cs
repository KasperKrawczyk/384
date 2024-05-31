using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.EventSystems;

public class MonsterDragManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    private Monster monster;
    private Vector3 startDragPosition;
    private Vector2 dragDirection;

    private void Awake() {
        monster = GetComponent<Monster>();
    }

    public void OnBeginDrag(PointerEventData eventData) {
        if (!monster.mbi.pushable) {
            return;
        }
        startDragPosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData) {
        if (!monster.mbi.pushable) {
            return;
        }
        // Determine the drag direction
        Vector2 currentDragPosition = eventData.position;
        dragDirection = (currentDragPosition - (Vector2)startDragPosition).normalized;
    }

    public void OnEndDrag(PointerEventData eventData) {
        if (!monster.mbi.pushable) {
            return;
        }
        // Move the monster to the adjacent tile in the direction of the drag
        MoveMonsterInDragDirection();
    }

    private void MoveMonsterInDragDirection() {
        if (dragDirection == Vector2.zero) {
            return;
        }

        Vector2 closestDirection = GetClosestDirection(dragDirection);
        Vector2 targetTile = (Vector2)monster.transform.position + closestDirection;
        if (IsViableTile(targetTile)) {
            monster.MoveToTile(targetTile);
        }
    }

    private Vector2 GetClosestDirection(Vector2 direction) {
        float maxDot = -Mathf.Infinity;
        Vector2 closestDir = Vector2.zero;

        foreach (Vector2 dir in MovementConstants.DIRECTIONS) {
            float dot = Vector2.Dot(dir.normalized, direction.normalized);
            if (dot > maxDot) {
                maxDot = dot;
                closestDir = dir;
            }
        }

        return closestDir;
    }

    private bool IsViableTile(Vector2 tile) {
        // Check for obstacles using the LayerMask and other conditions for a viable tile
        return !Physics2D.OverlapCircle(tile, Monster.OverlapCircleRadius, monster.obstaclesLayerMask) &&
               !TileReservationManager.Instance.IsTileReserved(Vector2Int.FloorToInt(tile));
    }
}
