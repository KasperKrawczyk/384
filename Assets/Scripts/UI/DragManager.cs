using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragManager : MonoBehaviour {
    public static DragManager Instance;

    private GameObject draggedItemIcon;
    private RectTransform draggedItemIconRectTransform;
    private Canvas canvas;
    private BaseItem draggedItem;
    private Vector2 dragOffset;
    private Vector2 canvasScaleFactor;
    private Camera mainCamera;
    private int uiLayerMask;
    private int itemsLayerMask;
    private int backgroundLayerMask;
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }

        canvas = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<Canvas>();
        mainCamera = canvas.worldCamera;
        canvasScaleFactor = new Vector2(canvas.scaleFactor, canvas.scaleFactor);
    }

    private void Start() {
        uiLayerMask = LayerMask.NameToLayer("UI");
        itemsLayerMask = LayerMask.NameToLayer("Items");
        backgroundLayerMask = LayerMask.NameToLayer("Background");
    }

    public void OnBeginDrag(PointerEventData eventData, BaseItem item, Vector2 offset) {
        draggedItem = item;
        dragOffset = offset;
        item.image.raycastTarget = false;
        
        if (draggedItem == null) return;

        // Create the dragged item icon
        draggedItemIcon = new GameObject("DraggedItemIcon");
        draggedItemIcon.transform.SetParent(canvas.transform);
        draggedItemIconRectTransform = draggedItemIcon.AddComponent<RectTransform>();
        Image image = draggedItemIcon.AddComponent<Image>();
        image.sprite = AssetsDB.Instance.spriteDictionary[item.baseInfo.spriteId];
        // image.SetNativeSize();
        image.raycastTarget = false;
        // draggedItemIconRectTransform.transform.localScale = new Vector2(32, 32);
        draggedItemIconRectTransform.sizeDelta = new Vector2(1, 1);

        UpdateDraggedIconPosition(eventData);
    }

    public void OnDrag(PointerEventData eventData) {
        UpdateDraggedIconPosition(eventData);
    }

    public void EndDrag(PointerEventData eventData) {
        if (draggedItem == null) return;

        Destroy(draggedItemIcon);
        Transform dropTarget = GetDropTarget(eventData);

        if (dropTarget != null) {
            HandleItemDrop(dropTarget, eventData);
        }

        draggedItem.image.raycastTarget = true;
        draggedItem = null;
    }

    private void UpdateDraggedIconPosition(PointerEventData eventData) {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, eventData.position, mainCamera, out localPoint);
        draggedItemIconRectTransform.localPosition = localPoint + dragOffset;
        
    }

    private Transform GetDropTarget(PointerEventData eventData) {
        PointerEventData pointerData = new PointerEventData(EventSystem.current) {
            position = eventData.position
        };

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, raycastResults);

        foreach (RaycastResult result in raycastResults) {
            int curLayer = result.gameObject.layer;
            if (curLayer == uiLayerMask || curLayer == itemsLayerMask || curLayer == backgroundLayerMask) {
                return result.gameObject.transform;
            }
        }

        return null;
    }

    
    
    private void HandleItemDrop(Transform dropTarget, PointerEventData eventData) {
        if (dropTarget.CompareTag("Slot")) {
            dropTarget.GetComponent<SlotPanelManager>().OnDrop(eventData);
        } else if (dropTarget.CompareTag("Tilemap")) {
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(eventData.position);
            DropOnTilemap(worldPosition);
        }
    }

    private void DropOnTilemap(Vector3 worldPosition) {
        Vector3Int cellPosition = TileReservationManager.Instance.WorldToCell(worldPosition);
        TileReservationManager.Instance.AddObjectToTile(draggedItem.gameObject, (Vector2Int)cellPosition);
        draggedItem.transform.SetParent(null);
        draggedItem.transform.position = TileReservationManager.Instance.CellToWorld(cellPosition) + new Vector3(0.5f, 0.5f, 0);
        draggedItem.ToggleMapPresence(true);
    }

    public void StartDrag(BaseItem item, Vector2 offset) {

    }
}
