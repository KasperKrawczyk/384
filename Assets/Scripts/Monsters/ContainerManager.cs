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
        Destroy(gameObject, CorpseDestroyTime);
        // Initialise(monsterName);
    }

    public void Initialise(BaseInfo corpseItemBaseInfo) {
        GameObject containerPrefab = Resources.Load<GameObject>("Prefabs/UI/ContainerParentPanel");
        
        // thisContainerObject = Instantiate(containerPrefab, transform.position, Quaternion.identity, transform);
        ThisContainer = Instantiate(containerPrefab, InventoryManager.Instance.transform);
        ThisContainer.SetActive(false);
        isOpen = false;
        
        cpm = ThisContainer.transform.Find("ContainerPanel").GetComponent<ContainerPanelManager>();
        cpm.Initialise(corpseItemBaseInfo.GetIntStat(IntStatInfoType.NumSlots));

        if (corpseItemBaseInfo.GetBoolStat(BoolStatInfoType.Corpse)) {
            dropTable = AssetsDB.Instance.dropTableDictionary[corpseItemBaseInfo.itemName];
            cpm.AddLootToInstantiate(InitialiseLoot(dropTable));
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
    
    private List<(string, int)> InitialiseLoot(DropTable dropTable)
    {
        List<(string, int)> pairs = new List<(string, int)>();

        int i = 0;
        foreach (DropStatInfo dsi in dropTable.dropStatInfoList)
        {
            if (Random.Range(0f, 1f) <= dsi.prob)
            {
                // GameObject itemPrefab = Resources.Load<GameObject>("Prefabs/Items/BaseItem");
                // BaseInfo baseInfo = AssetsDB.Instance.baseInfoDictionary[dsi.itemId];
                // itemPrefab.GetComponent<BaseItem>().InitialiseFromBaseInfo(baseInfo);
                int count = dsi.qty;
                if (dsi.qty > 1)
                {
                    count = Random.Range(0, dsi.qty + 1);
                    
                }
                // GameObject itemInstance = Instantiate(itemPrefab, transform.position, Quaternion.identity);
                if (count > 0) pairs.Add((dsi.itemId, count));
            }

            i++;
        }
        CollectionUtils.Shuffle(pairs);
        return pairs;
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
    
    public void ToggleOpenClose()
    {

        Debug.Log("in ToggleOpenClose");
        if (!isOpen)
        {
            ThisContainer.SetActive(true);
            isOpen = true;
            StartCoroutine(CloseOnDistanceLoop());
        }
        else
        {
            ThisContainer.SetActive(false);
            isOpen = false;
        }
        // _cv.alpha = _cv.alpha > 0 ? 0 : 1;
        // _cv.blocksRaycasts = !_cv.blocksRaycasts;
    }

    private float GetDistanceToPlayer()
    {
        return Vector3.Distance(playerTransform.position, transform.position);
    }

}