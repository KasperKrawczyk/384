
using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[
    RequireComponent(typeof(ObjectInteractionManager)),
    RequireComponent(typeof(Image)),
    RequireComponent(typeof(ItemDragManager))
]
public class BaseItem : MonoBehaviour
{
    [SerializeField] public BaseInfo baseInfo;
    [SerializeField] public GameObject useEffectPrefab;
    [SerializeField] protected SlotPanelManager spm;
    [SerializeField] private int stackSize;
    [SerializeField] public Image image;
    [HideInInspector] public ItemDragManager idm;


    public virtual void Awake()
    {
        image = GetComponent<Image>();
        if (baseInfo != null)
        {
            image.sprite = baseInfo.sprite;
        }
        idm = GetComponent<ItemDragManager>();
    }
}