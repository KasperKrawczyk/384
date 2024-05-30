using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotPanelManager : MonoBehaviour, IDropHandler, IEndDragHandler
{
    private ContainerPanelManager cpm;
    [SerializeField] private BaseItem CurrentItem;
    private int SlotIndex;

    private void Awake()
    {
        cpm = GetComponentInParent<ContainerPanelManager>();
    }

    public virtual void OnDrop(PointerEventData eventData)
    {
        GameObject droppedItem = eventData.pointerDrag;
        if (IsThisContainer(droppedItem)) {
            return;
        }
        BaseItem droppedBaseItem = droppedItem.GetComponent<BaseItem>();
        
        if (droppedBaseItem.baseInfo.GetBoolStat(BoolStatInfoType.Stackable))
        {
            if (transform.childCount == 0 && Input.GetKey(KeyCode.LeftControl))
            {
                SetCurrentItem(droppedItem);
            }
            else if (IsAcceptOnStackable(droppedItem) && Input.GetKey(KeyCode.LeftControl)) {
                InventoryManager.Instance.TransferWithoutSlider(cpm, SlotIndex, droppedBaseItem, CurrentItem);
            }
            else if (IsAcceptOnStackable(droppedItem))
            {
                InventoryManager.Instance.ShowTransferSlider(cpm, SlotIndex, droppedBaseItem, CurrentItem);
            }
            // else if (IsAcceptOnStackable(droppedItem) && !Input.GetKeyDown(KeyCode.LeftControl))
            // {
            //     InventoryManager.Instance.ShowTransferSlider(cpm, SlotIndex, droppedBaseItem, CurrentItem);
            // }
            // else if (IsAcceptOnStackable(droppedItem) && Input.GetKeyDown(KeyCode.LeftControl))
            // {
            //     InventoryManager.Instance.TransferWithoutSlider(cpm, SlotIndex, droppedBaseItem, CurrentItem);
            // }
        }
        else
        {
            if (transform.childCount == 0)
            {
                SetCurrentItem(droppedItem);
            }
            // else
            // {
            //     InventoryManager.Instance.TransferWithoutSlider(cpm, SlotIndex, droppedBaseItem, CurrentItem);
            // }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (transform.childCount == 0 && cpm != null)
        {
            cpm.getItems()[SlotIndex] = null;
        }
    }
    

    public void SetCurrentItem(GameObject item) {
        if (item == null) {
            if (cpm != null) cpm.getItems()[SlotIndex] = null;
            CurrentItem = null;
            return;
        }
        item.SetActive(true);
        ItemDragManager idm = item.GetComponent<ItemDragManager>();
        CurrentItem = item.GetComponent<BaseItem>();
        item.transform.SetParent(transform, false);
        if (CurrentItem.Spm != null && CurrentItem.Spm.transform.childCount == 0) {
            CurrentItem.Spm.CurrentItem = null;
        }
        CurrentItem.Spm = this;
        CurrentItem.ToggleMapPresence(false);
        // idm.originalParent = transform;
        if (cpm != null) cpm.getItems()[SlotIndex] = item;
    }

    public GameObject InstantiateAndSetCurrentItem(string itemId, int count)
    {
        GameObject itemPrefab = Resources.Load<GameObject>("Prefabs/Items/BaseItem");
        GameObject item = Instantiate(itemPrefab, transform);

        BaseInfo baseInfo = AssetsDB.Instance.baseInfoDictionary[itemId];
        item.GetComponent<BaseItem>().InitialiseFromBaseInfo(baseInfo, count);

        SetCurrentItem(item);

        return item;
    }

    public void SetSlotIndex(int slotIndex)
    {
        SlotIndex = slotIndex;
    }

    public void SetContainerPanelManager(ContainerPanelManager containerPanelManager)
    {
        cpm = containerPanelManager;
    }

    public BaseItem GetCurrentItem()
    {
        return CurrentItem;
    }

    private bool IsAcceptOnStackable(GameObject droppedItem)
    {
        BaseInfo droppedBi = droppedItem.GetComponent<BaseItem>().baseInfo;

        if (CurrentItem != null)
        {
            BaseInfo curBi = CurrentItem.baseInfo;
            return curBi.itemName == droppedBi.itemName && curBi.GetBoolStat(BoolStatInfoType.Stackable) && droppedBi.GetBoolStat(BoolStatInfoType.Stackable) &&
                   CurrentItem.count < 100;
        }
        else
        {
            return droppedBi.GetBoolStat(BoolStatInfoType.Stackable);
        }
    }

    private bool IsThisContainer(GameObject droppedItemObject) {
        return droppedItemObject == this.gameObject.transform.parent.gameObject;
    }
}
