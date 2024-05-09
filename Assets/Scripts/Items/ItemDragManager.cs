using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class ItemDragManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private BaseItem baseItem;

    public Transform originalParent;
    private RectTransform rectTransform;
    [SerializeField] private Canvas canvas;
    
    private void Awake()
    {
        baseItem = GetComponent<BaseItem>();
        rectTransform = baseItem.GetComponent<RectTransform>();
        canvas = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<Canvas>();

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        baseItem.image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // transform.position = Mouse.current.position.ReadValue();
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        FinalizeDrag();
    }
    
    public void FinalizeDrag()
    {
        transform.SetParent(originalParent);
        GetComponent<BaseItem>().image.raycastTarget = true; // Re-enable raycast blocking
    }
}