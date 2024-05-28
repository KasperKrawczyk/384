using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[
    RequireComponent(typeof(ObjectInteractionManager))
]
public class CorpseManager : MonoBehaviour
{
    public static float MinDistanceToPlayer = 1.5f;

    
    [SerializeField] private GameObject corpseContainerPrefab;
    [SerializeField] private GameObject thisContainerPrefab;
    [SerializeField] private DropTable dropTable;
    [SerializeField] private string monsterName;
    [SerializeField] private List<(string, int)> lootIdAndQtyPairs;

    private Transform playerTransform;
    private BoxCollider2D _bc;
    public ObjectInteractionManager _oim;
    public Sprite sprite;
    private const float CorpseDestroyTime = 60f * 5;
    private bool isOpen = false;
    
    public void Awake()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        _bc = GetComponent<BoxCollider2D>();
        _oim = GetComponent<ObjectInteractionManager>();
        _oim.OnInteractableClick += OnClick;
        Destroy(gameObject, CorpseDestroyTime);
        dropTable = AssetsDB.Instance.dropTableDictionary[monsterName];
        thisContainerPrefab = Instantiate(corpseContainerPrefab, transform.position, Quaternion.identity, transform);
        thisContainerPrefab.GetComponent<ContainerItem>().image.sprite = GetComponentInParent<SpriteRenderer>().sprite;
        thisContainerPrefab.GetComponent<ContainerItem>().Initialise();
        InitialiseLoot();
        thisContainerPrefab.GetComponent<ContainerItem>().TransferLootPairs(lootIdAndQtyPairs);
        isOpen = false;
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
    
    private void InitialiseLoot()
    {
        lootIdAndQtyPairs = new List<(string, int)>();

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
                if (count > 0) lootIdAndQtyPairs.Add((dsi.itemId, count));
            }

            i++;
        }
        CollectionUtils.Shuffle(lootIdAndQtyPairs);
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
            thisContainerPrefab.GetComponent<ContainerItem>().ToggleOpenClose();
            isOpen = true;
            StartCoroutine(CloseOnDistanceLoop());
        }
        else
        {
            thisContainerPrefab.GetComponent<ContainerItem>().ToggleOpenClose();
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