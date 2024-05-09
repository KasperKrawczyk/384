using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerPanelManager : MonoBehaviour
{
    [SerializeField] protected GameObject slotPanelPrefab;
    private GameObject[] items;
    private GameObject[] slots;

    private void Awake()
    {
        this.items = new GameObject[1];
        this.slots = new GameObject[1];
    }

    public void Initialise(int numSlots)
    {
        slots = new GameObject[numSlots];
        items = new GameObject[numSlots];
        for (int i = 0; i < numSlots; i++)
        {
            slots[i] = Instantiate(slotPanelPrefab, transform);
            slots[i].GetComponent<SlotPanelManager>().SetSlotIndex(i);
            slots[i].GetComponent<SlotPanelManager>().SetContainerPanelManager(this);

        }
        // for (int i = 0; i < items.Length; i++)
        // {
            // slots[i].GetComponent<SlotPanelManager>().SetCurrentItem(items[i]);
        // }

    } 
    public void AddPrefabsToInstantiate(List<GameObject> prefabsToAdd)
    {
        int itemsSlotCount = 0;
        for (int i = 0; i < prefabsToAdd.Count; i++)
        {
            while (items[itemsSlotCount] != null && itemsSlotCount < items.Length)
            {
                itemsSlotCount++;
            }
            
            SlotPanelManager spm = slots[itemsSlotCount].GetComponent<SlotPanelManager>();
            GameObject item = spm.InstantiateAndSetCurrentItem(prefabsToAdd[i]);
            items[itemsSlotCount] = item;
        }

    }

    public GameObject[] getItems()
    {
        return this.items;
    }


}