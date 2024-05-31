using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDragManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    private BaseItem baseItem;

    private void Awake() {
        baseItem = GetComponent<BaseItem>();
    }
    
    public void OnBeginDrag(PointerEventData eventData) {
        Vector2 offset = new Vector2(-32, -32); // Adjust as necessary to position the icon relative to the cursor
        DragManager.Instance.OnBeginDrag(eventData, baseItem, offset);
    }

    public void OnEndDrag(PointerEventData eventData) {
        DragManager.Instance.EndDrag(eventData);
    }
    
    public void OnDrag(PointerEventData eventData) {
        DragManager.Instance.OnDrag(eventData);
    }

}