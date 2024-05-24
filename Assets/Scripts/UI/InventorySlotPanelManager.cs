using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class InventorySlotPanelManager : SlotPanelManager
{
    [SerializeField] public SlotPosition slotPosition;
    [SerializeField] private InventorySlotPanelManager otherHandIspm;

    public override void OnDrop(PointerEventData eventData)
    {
        if (!otherHandIspm && transform.childCount == 0 && eventData.pointerDrag.GetComponent<BaseItem>().baseInfo.SlotPosition == slotPosition)
        {
            GameObject droppedItem = eventData.pointerDrag;
            SetCurrentItem(droppedItem);
        }
        else if (otherHandIspm && transform.childCount == 0 && eventData.pointerDrag.GetComponent<BaseItem>().baseInfo.SlotPosition == slotPosition)
        {
            GameObject dropped = eventData.pointerDrag;
            int otherHandItemNumHands = GetOtherHandNumHands();
            int droppedItemNumHands = GetDroppedNumHands(dropped);
            if (droppedItemNumHands == -1) return;
            if (otherHandItemNumHands + droppedItemNumHands <= 2)
            {
                SetCurrentItem(dropped);
            }
            
        }
    }

    private int GetDroppedNumHands(GameObject dropped)
    {
        BaseItem bItem = dropped.GetComponent<BaseItem>();
        if (!bItem) return -1;
        return bItem.baseInfo.GetIntStat(IntStatInfoType.Hands);
    }

    private int GetOtherHandNumHands()
    {
        BaseItem otherHandCurItem = otherHandIspm.GetCurrentItem();
        if (!otherHandCurItem) return 0;
        return otherHandCurItem.baseInfo.GetIntStat(IntStatInfoType.Hands);
    }
}