using System;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private static InventoryManager _instance;

    [SerializeField] public Canvas uiCanvas;
    [SerializeField] public GameObject transferSliderPrefab;

    // debug
    [SerializeField] private BaseItem[] items; 
    public static InventoryManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<InventoryManager>();
            }

            return _instance;
        }
    }

    public void ShowTransferSlider(ContainerPanelManager cpm, int slotIndex, BaseItem sourceItem, BaseItem targetItem)
    {
        GameObject sliderUI = Instantiate(transferSliderPrefab, uiCanvas.transform);
        ItemTransferSliderManager itemTransferSlider = sliderUI.GetComponent<ItemTransferSliderManager>();
        itemTransferSlider.Initialize(cpm, slotIndex, sourceItem, targetItem, sourceItem.count);
    }

    public void TransferWithoutSlider(ContainerPanelManager cpm, int slotIndex, BaseItem sourceItem, BaseItem targetItem)
    {
        TransferItemsWithCount(cpm, slotIndex, sourceItem, targetItem, sourceItem.count);
    }

    public void TransferItemsWithCount(ContainerPanelManager cpm, int slotIndex, BaseItem sourceItem, BaseItem targetItem, int transferCount)
    {
        if (transferCount > 0)
        {
            if (targetItem && targetItem.count + transferCount > 100)
            {
                int spaceAvailable = 100 - targetItem.count;
                targetItem.SetCount(100);
                transferCount -= spaceAvailable;
                sourceItem.SetCount(sourceItem.count - spaceAvailable);
                transferCount = cpm.DistributeStackable(sourceItem.gameObject, transferCount);
            }
            else if (targetItem && targetItem.count + transferCount <= 100)
            {
                targetItem.SetCount(targetItem.count + transferCount);
                sourceItem.SetCount(sourceItem.count - transferCount);
            }
            else if (!targetItem)
            {
                GameObject cloned = sourceItem.CloneWithCount(transferCount);
                sourceItem.SetCount(sourceItem.count - transferCount);
                cpm.Slots[slotIndex].GetComponent<SlotPanelManager>().SetCurrentItem(sourceItem.gameObject);
            }
        }
    }
}
