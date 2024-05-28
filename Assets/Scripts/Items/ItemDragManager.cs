using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDragManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler {
    private BaseItem baseItem;

    public Transform originalParent;
    private RectTransform rectTransform;
    [SerializeField] private Canvas canvas;

    private Vector3 originalPosition;
    private bool isDragStartedOverUI;

    private void Awake() {
        baseItem = GetComponent<BaseItem>();
        rectTransform = baseItem.GetComponent<RectTransform>();
        canvas = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<Canvas>();
    }

    public void OnPointerDown(PointerEventData eventData) {
        Debug.Log($"OnPointerDown in {name}");
        if (baseItem.sr != null) {
            originalPosition = transform.position;
        }
    }

    public void OnBeginDrag(PointerEventData eventData) {
        Debug.Log($"OnBeginDrag in {name}");
        
        isDragStartedOverUI = EventSystem.current.IsPointerOverGameObject(eventData.pointerId);
        
        if (baseItem.sr == null) {
            originalParent = transform.parent;
            transform.SetParent(transform.root);
            transform.SetAsLastSibling();
            baseItem.image.raycastTarget = false;
        }
    }

    public void OnDrag(PointerEventData eventData) {
        Debug.Log($"OnDrag in {name}");
        if (baseItem.sr == null) {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData eventData) {
        Debug.Log($"OnEndDrag in {name}");
        FinalizeDrag(eventData);
    }

    public void FinalizeDrag(PointerEventData eventData) {
        int itemsLayer = LayerMask.NameToLayer("Items");
        int backgroundLayer = LayerMask.NameToLayer("Background");
        int uiLayer = LayerMask.NameToLayer("UI");
        int combinedLayerMask = (1 << itemsLayer) | (1 << backgroundLayer) | (1 << uiLayer);

        Ray ray = Camera.main.ScreenPointToRay(eventData.position);
        RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction, Mathf.Infinity, combinedLayerMask);

        RaycastHit2D hit = new RaycastHit2D();
        bool objectHit = false;
        foreach (var h in hits) {
            int hitLayer = h.collider.gameObject.layer;
            if (hitLayer == itemsLayer) {
                hit = h;
                objectHit = true;
                break;
            }

            if (hitLayer == backgroundLayer || hitLayer == uiLayer) {
                hit = h;
            }
        }

        if (objectHit) {
            DropOnTilemap(hit.point);
        } else if (hit.collider != null && hit.collider.gameObject.layer == backgroundLayer) {
            DropOnTilemap(hit.point);
        } else {
            // if (EventSystem.current.IsPointerOverGameObject(eventData.pointerId) && !baseItem.baseInfo.GetBoolStat(BoolStatInfoType.Stackable)) {
            if (isDragStartedOverUI && !baseItem.baseInfo.GetBoolStat(BoolStatInfoType.Stackable)) {
                MoveToUI(eventData);
            } else {
                ReturnToOriginalPosition();
            }
        }

        baseItem.image.raycastTarget = true;
    }

    private void DropOnTilemap(Vector3 worldPosition) {
        Vector3Int cellPosition = TileReservationManager.Instance.WorldToCell(worldPosition);
        TileReservationManager.Instance.AddObjectToTile(gameObject, (Vector2Int)cellPosition);
        transform.SetParent(null);
        transform.position = TileReservationManager.Instance.CellToWorld(cellPosition) + new Vector3(0.5f, 0.5f, 0);
        baseItem.ToggleMapPresence(true);
    }


    private void MoveToUI(PointerEventData eventData) {
        Transform dropTarget = GetDropTarget(eventData);
        // if (dropTarget.gameObject.CompareTag("Slot")) {
        //     dropTarget.gameObject.GetComponent<SlotPanelManager>().OnDrop(eventData);
        // }
        // if (dropTarget != null) {
            // transform.SetParent(dropTarget);
            // rectTransform.anchoredPosition = Vector2.zero;
            // baseItem.ToggleMapPresence(false);
        // }
        // else {
            ReturnToOriginalPosition();
        // }
    }

    private Transform GetDropTarget(PointerEventData eventData) {
        PointerEventData pointerData = new PointerEventData(EventSystem.current) {
            position = eventData.position
        };

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, raycastResults);

        foreach (RaycastResult result in raycastResults) {
            if (result.gameObject.layer == LayerMask.NameToLayer("UI")) {
                return result.gameObject.transform;
            }
        }

        return null;
    }

    public void ReturnToOriginalPosition() {
        if (baseItem.sr != null) {
            transform.position = originalPosition;
        }
        else {
            transform.SetParent(originalParent);
            rectTransform.anchoredPosition = Vector2.zero;
        }
    }
}