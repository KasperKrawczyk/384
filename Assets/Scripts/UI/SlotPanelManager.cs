using System;
using DarkPixelRPGUI.Scripts.UI.Equipment;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotPanelManager : MonoBehaviour, IDropHandler, IEndDragHandler
{
    private ContainerPanelManager cpm;
    [SerializeField] private BaseItem CurrentItem;
    private int SlotIndex;
    public ItemType? itemType;

    private void Awake()
    {
        this.cpm = GetComponentInParent<ContainerPanelManager>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount != 0)
        {
            return;
        }

        if (itemType != null && eventData.pointerDrag.GetComponent<BaseItem>().baseInfo.itemType == itemType) 
        {
            GameObject droppedItem = eventData.pointerDrag;
            SetCurrentItem(droppedItem);
        }
        else if (itemType == null)
        {
            GameObject droppedItem = eventData.pointerDrag;
            SetCurrentItem(droppedItem);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (transform.childCount == 0 && this.cpm != null)
        {
            this.cpm.getItems()[SlotIndex] = null;
        }
    }

    public void SetCurrentItem(GameObject item)
    {
        item.SetActive(true);
        ItemDragManager idm = item.GetComponent<ItemDragManager>();
        CurrentItem = item.GetComponent<BaseItem>();
        item.transform.SetParent(transform);
        idm.originalParent = transform;
        if (this.cpm != null) this.cpm.getItems()[SlotIndex] = item;
        
    } 
    
    public GameObject InstantiateAndSetCurrentItem(GameObject prefabItem)
    {
        // GameObject item = Instantiate(prefabItem, Vector3.zero, Quaternion.identity, transform);
        GameObject item = Instantiate(prefabItem, transform);
        // item.SetActive(true);
        // ItemDragManager idm = item.GetComponent<ItemDragManager>();
        CurrentItem = item.GetComponent<BaseItem>();
        // idm.originalParent = transform;
        if (this.cpm != null) this.cpm.getItems()[SlotIndex] = item;
        return item;

    } 
    
    public void SetSlotIndex(int SlotIndex)
    {
        this.SlotIndex = SlotIndex;
    }

    public void SetContainerPanelManager(ContainerPanelManager containerPanelManager)
    {
        this.cpm = containerPanelManager;
    }

    public BaseItem GetCurrentItem()
    {
        return this.CurrentItem;
    }

}