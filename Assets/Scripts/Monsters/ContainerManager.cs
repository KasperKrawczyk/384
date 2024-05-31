using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

[
    RequireComponent(typeof(ObjectInteractionManager))
]
public class ContainerManager : MonoBehaviour
{
    public static float MinDistanceToPlayer = 1.5f;

    [SerializeField] protected GameObject containerParentPanelPrefab;
    [SerializeField] protected ContainerPanelManager cpm;
    [SerializeField] public GameObject[] Items;
    [SerializeField] public GameObject[] Slots;
    // private ObjectInteractionManager _oim;
    public GameObject ThisContainer { private set; get; }
    // [SerializeField] private GameObject thisContainerObject;
    [SerializeField] private DropTable dropTable;

    private Transform playerTransform;
    private const float CorpseDestroyTime = 60f * 5;
    private bool isOpen = false;
    
    public void Awake()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        GetComponentInParent<ObjectInteractionManager>().OnInteractableClick += OnClick;
        // Destroy(gameObject, CorpseDestroyTime);

        // Initialise(monsterName);
    }

    public void Initialise(BaseInfo corpseItemBaseInfo) {
        GameObject containerPrefab = Resources.Load<GameObject>("Prefabs/UI/ContainerParentPanel");
        
        // thisContainerObject = Instantiate(containerPrefab, transform.position, Quaternion.identity, transform);
        // ThisContainer = Instantiate(containerPrefab, InventoryManager.Instance.transform);
        // ThisContainer.SetActive(false);
        isOpen = false;
        byte numSlots = (byte)corpseItemBaseInfo.GetIntStat(IntStatInfoType.NumSlots);
        // cpm = ThisContainer.transform.Find("ContainerPanel").GetComponent<ContainerPanelManager>();
        // cpm.Initialise(numSlots);
        this.Items = new GameObject[numSlots];
        this.Slots = new GameObject[numSlots];
        if (corpseItemBaseInfo.GetBoolStat(BoolStatInfoType.Corpse)) {
            dropTable = AssetsDB.Instance.dropTableDictionary[corpseItemBaseInfo.itemName];
            // cpm.AddLootToInstantiate(InitialiseFromDropTable(dropTable));
            InitialiseFromDropTable(dropTable, Items);
        }
        
    }
    
    protected virtual IEnumerator CloseOnDistanceLoop()
    {
        while (isOpen)
        {
            if (GetDistanceToPlayer() > MinDistanceToPlayer)
            {
                ToggleOpenClose();
                yield break;
            }
            yield return new WaitForSeconds(.5f); // Check every half second
        }
    }
    
    private void InitialiseFromDropTable(DropTable dropTable, GameObject[] containerItemsArray)
    {
        List<GameObject> items = new List<GameObject>();

        foreach (DropStatInfo dsi in dropTable.dropStatInfoList)
        {
            if (Random.Range(0f, 1f) <= dsi.prob)
            {
                int count = dsi.qty;
                if (dsi.qty > 1)
                {
                    count = Random.Range(0, dsi.qty + 1);
                    
                }

                GameObject item = InstantiateItem(dsi.itemId, count);
                if (count > 0) items.Add(item);
            }

        }
        CollectionUtils.Shuffle(items);
        for (int i = 0; i < items.Count; i++) {
            containerItemsArray[i] = items[i];
        }
    }
    
    public void OnClick(InputAction.CallbackContext context)
    {
        Debug.Log("OnPointerClick in " + name);
        if (GetDistanceToPlayer() > MinDistanceToPlayer)
        {
            return;
        }
        ToggleOpenClose();

    }
    
    private void ToggleOpenClose()
    {

        Debug.Log("in ToggleOpenClose");
        if (!isOpen)
        {
            ThisContainer = InventoryManager.Instance.containerParentPanelPool.Get();
            cpm = ThisContainer.GetComponent<ContainerPanelManager>();
            cpm.ConnectWithContainer(this);
            isOpen = true;
            StartCoroutine(CloseOnDistanceLoop());
        }
        else
        {
            cpm.DisconnectFromContainer();
            InventoryManager.Instance.containerParentPanelPool.Return(ThisContainer);
            ThisContainer = null;
            cpm = null;
            isOpen = false;
        }
        // _cv.alpha = _cv.alpha > 0 ? 0 : 1;
        // _cv.blocksRaycasts = !_cv.blocksRaycasts;
    }

    private float GetDistanceToPlayer()
    {
        return Vector3.Distance(playerTransform.position, transform.position);
    }
    
    // public void Initialise(int numSlots)
    // {
    //     Slots = new GameObject[numSlots];
    //     Items = new GameObject[numSlots];
    //     for (int i = 0; i < numSlots; i++)
    //     {
    //         // Slots[i] = Instantiate(slotPanelPrefab, transform);
    //         Slots[i].GetComponent<SlotPanelManager>().SetSlotIndex(i);
    //         Slots[i].GetComponent<SlotPanelManager>().SetContainerPanelManager(this);
    //
    //     }
    // } 
    
    public GameObject InstantiateItem(string itemId, int count)
    {
        GameObject itemPrefab = Resources.Load<GameObject>("Prefabs/Items/BaseItem");
        GameObject item = Instantiate(itemPrefab, transform);
        BaseInfo baseInfo = AssetsDB.Instance.baseInfoDictionary[itemId];
        item.GetComponent<BaseItem>().InitialiseFromBaseInfo(baseInfo, count);
        item.SetActive(false);

        return item;
    }
}