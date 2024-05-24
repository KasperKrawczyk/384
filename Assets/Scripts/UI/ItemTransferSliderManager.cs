using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ItemTransferSliderManager : MonoBehaviour
{
    // Start is called before the first frame update

    public Slider slider;
    public TextMeshProUGUI countText;
    public TextMeshProUGUI maxText;
    private int maxCount;
    private int slotIndex;
    private BaseItem sourceItem;
    private BaseItem targetItem;
    private ContainerPanelManager targetCpm;

    public void Initialize(ContainerPanelManager cpm, int slotIndex, BaseItem sourceItem, BaseItem targetItem, int maxCount)
    {
        this.sourceItem = sourceItem;
        this.targetItem = targetItem;
        this.maxCount = maxCount;
        this.targetCpm = cpm;
        this.slotIndex = slotIndex;

        slider.maxValue = maxCount;
        maxText.text = maxCount.ToString();
        slider.value = 0;
        slider.onValueChanged.AddListener(OnSliderValueChanged);
        UpdateValueText(0);
    }

    private void OnSliderValueChanged(float value)
    {
        UpdateValueText((int)value);
    }

    private void UpdateValueText(int value)
    {
        countText.text = value.ToString();
    }

    public void ConfirmTransfer()
    {
        int transferCount = (int) slider.value;

        if (transferCount > 0)
        {
            if (targetItem && targetItem.count + transferCount > 100)
            {
                int spaceAvailable = 100 - targetItem.count;
                targetItem.SetCount(100);
                transferCount -= spaceAvailable;
                sourceItem.SetCount(sourceItem.count - transferCount);
                transferCount = targetCpm.DistributeStackable(sourceItem.gameObject, transferCount);
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
                this.targetCpm.Slots[slotIndex].GetComponent<SlotPanelManager>().SetCurrentItem(cloned);
                
            }
            

        }

        Destroy(gameObject); // Close the slider UI after confirming the transfer
    }

    public void CancelTransfer()
    {
        Destroy(gameObject); // Close the slider UI without making any changes
    }
    
}
