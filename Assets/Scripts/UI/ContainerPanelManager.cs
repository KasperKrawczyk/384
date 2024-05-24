using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ContainerPanelManager : MonoBehaviour
{
    [SerializeField] protected GameObject slotPanelPrefab;
    public GameObject[] Items { get; private set; }
    public GameObject[] Slots { get; private set; }

    private void Awake()
    {
        this.Items = new GameObject[1];
        this.Slots = new GameObject[1];
    }

    public void Initialise(int numSlots)
    {
        Slots = new GameObject[numSlots];
        Items = new GameObject[numSlots];
        for (int i = 0; i < numSlots; i++)
        {
            Slots[i] = Instantiate(slotPanelPrefab, transform);
            Slots[i].GetComponent<SlotPanelManager>().SetSlotIndex(i);
            Slots[i].GetComponent<SlotPanelManager>().SetContainerPanelManager(this);

        }
        // for (int i = 0; i < Items.Length; i++)
        // {
            // Slots[i].GetComponent<SlotPanelManager>().SetCurrentItem(Items[i]);
        // }

    } 
    public void AddLootToInstantiate(List<(string, int)> lootIdAndQtyPairs)
    {
        int itemsSlotCount = 0;
        for (int i = 0; i < lootIdAndQtyPairs.Count; i++)
        {
            while (Items[itemsSlotCount] != null && itemsSlotCount < Items.Length)
            {
                itemsSlotCount++;
            }

            var (curItemId, curItemCount) = lootIdAndQtyPairs[i];
            SlotPanelManager spm = Slots[itemsSlotCount].GetComponent<SlotPanelManager>();
            GameObject item = spm.InstantiateAndSetCurrentItem(curItemId, curItemCount);
            Items[itemsSlotCount] = item;
        }

    }

    public GameObject[] getItems()
    {
        return this.Items;
    }

    public int DistributeStackable(GameObject itemGameObject, int toDistribute)
    {
        BaseItem transferredItem = itemGameObject.GetComponent<BaseItem>();
        for (int i = 0; i < Slots.Length; i++)
        {
            BaseItem curItem = Slots[i].GetComponent<SlotPanelManager>().GetCurrentItem();
            if (curItem && curItem.baseInfo.itemName == transferredItem.baseInfo.itemName && curItem.count < 100)
            {
                int curItemCount = curItem.count;
                int spaceAvailable = 100 - curItemCount;

                if (toDistribute <= spaceAvailable)
                {
                    curItem.SetCount(curItemCount + toDistribute);
                    return 0;
                }
                else
                {
                    curItem.SetCount(100);
                    toDistribute -= spaceAvailable;
                }
            } else if (curItem == null)
            {
                Slots[i].GetComponent<SlotPanelManager>().SetCurrentItem(itemGameObject);
                return 0;
            }

        }

        return toDistribute;
    }


}