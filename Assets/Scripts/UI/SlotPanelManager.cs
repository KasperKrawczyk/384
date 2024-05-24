using System;
using DarkPixelRPGUI.Scripts.UI.Equipment;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotPanelManager : MonoBehaviour, IDropHandler, IEndDragHandler
{
    private ContainerPanelManager cpm;
    [SerializeField] private BaseItem CurrentItem;
    private int SlotIndex;

    private void Awake()
    {
        this.cpm = GetComponentInParent<ContainerPanelManager>();
    }

    public virtual void OnDrop(PointerEventData eventData)
    {
        GameObject droppedItem = eventData.pointerDrag;
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (transform.childCount == 0 && !droppedItem.GetComponent<BaseItem>().baseInfo.GetBoolStat(BoolStatInfoType.Stackable))
            {
                SetCurrentItem(droppedItem);
            }
            else if (transform.childCount == 0 && droppedItem.GetComponent<BaseItem>().baseInfo.GetBoolStat(BoolStatInfoType.Stackable))
            {
                BaseItem droppedBaseItem = droppedItem.GetComponent<BaseItem>();
                InventoryManager.Instance.TransferWithoutSlider(this.cpm, SlotIndex, droppedBaseItem, CurrentItem);
            }
            else if (IsAcceptOnStackable(droppedItem))
            {
                BaseItem droppedBaseItem = droppedItem.GetComponent<BaseItem>();
                InventoryManager.Instance.TransferWithoutSlider(this.cpm, SlotIndex, droppedBaseItem, CurrentItem);
            }
        }
        else
        {
            if (transform.childCount == 0 && !droppedItem.GetComponent<BaseItem>().baseInfo.GetBoolStat(BoolStatInfoType.Stackable))
            {
                SetCurrentItem(droppedItem);
            }
            else if (transform.childCount == 0 && droppedItem.GetComponent<BaseItem>().baseInfo.GetBoolStat(BoolStatInfoType.Stackable))
            {
                BaseItem droppedBaseItem = droppedItem.GetComponent<BaseItem>();
                InventoryManager.Instance.ShowTransferSlider(this.cpm, SlotIndex, droppedBaseItem, CurrentItem);
            }
            else if (IsAcceptOnStackable(droppedItem))
            {
                BaseItem droppedBaseItem = droppedItem.GetComponent<BaseItem>();
                InventoryManager.Instance.ShowTransferSlider(this.cpm, SlotIndex, droppedBaseItem, CurrentItem);
            }
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
        item.transform.SetParent(transform, false);
        idm.originalParent = transform;
        if (this.cpm != null) this.cpm.getItems()[SlotIndex] = item;
    }

    public GameObject InstantiateAndSetCurrentItem(string itemId, int count)
    {
        GameObject itemPrefab = Resources.Load<GameObject>("Prefabs/Items/BaseItem");
        GameObject item = Instantiate(itemPrefab, transform);

        BaseInfo baseInfo = AssetsDB.Instance.baseInfoDictionary[itemId];
        item.GetComponent<BaseItem>().InitialiseFromBaseInfo(baseInfo, count);
        
        SetCurrentItem(item);


        // idm.originalParent = transform;
        // if (this.cpm != null) this.cpm.getItems()[SlotIndex] = item;
        return item;
    }

    public void SetSlotIndex(int slotIndex)
    {
        this.SlotIndex = slotIndex;
    }

    public void SetContainerPanelManager(ContainerPanelManager containerPanelManager)
    {
        this.cpm = containerPanelManager;
    }

    public BaseItem GetCurrentItem()
    {
        return this.CurrentItem;
    }

    private bool IsAcceptOnStackable(GameObject droppedItem)
    {
        BaseInfo droppedBi = droppedItem.GetComponent<BaseItem>().baseInfo;

        if (this.CurrentItem != null)
        {
            BaseInfo curBi = this.CurrentItem.baseInfo;
            return curBi.itemName == droppedBi.itemName && curBi.GetBoolStat(BoolStatInfoType.Stackable) && droppedBi.GetBoolStat(BoolStatInfoType.Stackable) &&
                   CurrentItem.count < 100;
        }
        else
        {
            return droppedBi.GetBoolStat(BoolStatInfoType.Stackable);
        }
    }
}