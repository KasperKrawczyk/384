using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[
    RequireComponent(typeof(ObjectInteractionManager))
]
public class CorpseManager : MonoBehaviour
{
    public static event Action OnOpenedAction;
    public static float MinDistanceToPlayer = 1.5f;

    
    [SerializeField] private GameObject corpseContainerPrefab;
    [SerializeField] private GameObject thisContainerPrefab;
    [SerializeField] private DropTable dropTable;
    [FormerlySerializedAs("loot")] [SerializeField] private List<GameObject> lootPrefabs;

    private Transform playerTransform;
    private BoxCollider2D _bc;
    private ObjectInteractionManager _oim;
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
        
        thisContainerPrefab = Instantiate(corpseContainerPrefab, transform.position, Quaternion.identity, transform);
        thisContainerPrefab.GetComponent<ContainerItem>().image.sprite = GetComponentInParent<SpriteRenderer>().sprite;
        thisContainerPrefab.GetComponent<ContainerItem>().Initialise();
        InitialiseLoot();
        thisContainerPrefab.GetComponent<ContainerItem>().TransferItems(lootPrefabs);
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
            yield return new WaitForSeconds(.25f); // Check every half second
        }
    }
    
    private void InitialiseLoot()
    {
        lootPrefabs = new List<GameObject>();

        int i = 0;
        foreach (DropStatInfo dsi in dropTable.dropStatInfoList)
        {
            if (UnityEngine.Random.Range(0f, 1f) <= dsi.prob)
            {
                GameObject itemPrefab = Resources.Load<GameObject>($"{dsi.address}");
                // GameObject itemInstance = Instantiate(itemPrefab, transform.position, Quaternion.identity);
                lootPrefabs.Add(itemPrefab);
            }

            i++;
        }
        CollectionUtils.Shuffle(lootPrefabs);
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
            OnOpenedAction?.Invoke();
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