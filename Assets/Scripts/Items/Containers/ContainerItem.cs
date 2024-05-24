using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

[
    RequireComponent(typeof(ContainerButtonManager)),
]
public class ContainerItem : BaseItem
{
    [SerializeField] protected GameObject containerParentPanelPrefab;
    [SerializeField] protected ContainerPanelManager cpm;
    [SerializeField] protected ContainerButtonManager cbm;
    [SerializeField] protected ContainerStats containerStats;
    private ContainerInitialiser _ci;
    
    // private ObjectInteractionManager _oim;
    private CanvasGroup _cv;
    private bool isOpen = false;
    public GameObject ThisContainer { private set; get; }
    
    public new void Awake()
    {
        image = GetComponent<Image>();
        idm = GetComponent<ItemDragManager>();
    }
    
    public void Start()
    {
        // _oim = GetComponent<ObjectInteractionManager>();
        _cv = GetComponent<CanvasGroup>();
        cbm = GetComponent<ContainerButtonManager>();
        cpm = GetComponent<ContainerPanelManager>();
        // _oim.OnInteractableClick += OnClick;
        
    }

    public void Initialise()
    {
        ThisContainer = Instantiate(containerParentPanelPrefab, InventoryManager.Instance.transform);
        cpm = ThisContainer.transform.Find("ContainerPanel").GetComponent<ContainerPanelManager>();
        ThisContainer.SetActive(false);
        cpm.Initialise(containerStats.numSlots);

    }
    
    public void ToggleOpenClose()
    {
        Debug.Log("in ToggleOpenClose");
        if (!isOpen)
        {
            ThisContainer.SetActive(true);
            isOpen = true;
        }
        else
        {
            ThisContainer.SetActive(false);
            isOpen = false;
        }
        // _cv.alpha = _cv.alpha > 0 ? 0 : 1;
        // _cv.blocksRaycasts = !_cv.blocksRaycasts;
    }


    public void TransferLootPairs(List<(string, int)> lootIdAndQtyPairs)
    {
        cpm.AddLootToInstantiate(lootIdAndQtyPairs);

    }
}