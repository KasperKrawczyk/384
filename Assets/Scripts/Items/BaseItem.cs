using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

[
    RequireComponent(typeof(ObjectInteractionManager)),
    RequireComponent(typeof(Image)),
    RequireComponent(typeof(ItemDragManager))
]
public class BaseItem : MonoBehaviour, ICloneable
{
    [SerializeField] public BaseInfo baseInfo;
    [SerializeField] public GameObject useEffectPrefab;
    [SerializeField] public SpriteRenderer sr;
    [SerializeField] public BoxCollider2D bc;
    [SerializeField] protected SlotPanelManager spm;
    [SerializeField] public Image image;
    [SerializeField] public int count;
    [HideInInspector] public ItemDragManager idm;
    private TextMeshProUGUI _countText;
    private ObjectInteractionManager _oim;

    
    public virtual void Awake()
    {
        image = GetComponent<Image>();
        if (baseInfo != null)
        {
            if (baseInfo.GetBoolStat(BoolStatInfoType.Usable))
            {
                _oim = this.GetComponent<ObjectInteractionManager>();
                _oim.OnInteractableClick += Use;
            }
        }
        idm = GetComponent<ItemDragManager>();
    }

    public void InitialiseFromBaseInfo(BaseInfo bi, int count)
    {
        baseInfo = bi;
        image = GetComponent<Image>();
        if (baseInfo != null)
        {
            image.sprite = AssetsDB.Instance.spriteDictionary[baseInfo.spriteId];
            if (baseInfo.GetBoolStat(BoolStatInfoType.Usable) && !_oim)
            {
                _oim = this.GetComponent<ObjectInteractionManager>();
                _oim.OnInteractableClick += Use;
            }

            if (!String.IsNullOrEmpty(baseInfo.useEffectId) && baseInfo.WeaponType == WeaponType.Dist)
            {
                Sprite useEffectSprite = AssetsDB.Instance.spriteDictionary[baseInfo.useEffectId];
                useEffectPrefab = Resources.Load<GameObject>("Prefabs/CombatEffects/Ranged/RangedCombatEffectPrefab");
                useEffectPrefab.GetComponent<SpriteRenderer>().sprite = useEffectSprite;
            }
            if (baseInfo.GetBoolStat(BoolStatInfoType.Stackable))
            {
                this.count = count;
                SetCount(count);
            }
        }
    }
    

    public void SetCount(int count)
    {
        this.count = count;
        if (this.count == 0)
        {
            Destroy(gameObject);
            return;
        }
        UpdateCountText();
    }

    public void EnableCountText()
    {
        if (_countText == null)
        {
            GameObject countTextObject = new GameObject("CountText");
            countTextObject.transform.SetParent(transform, false);

            // // Configure the RectTransform
            RectTransform rectTransform = countTextObject.AddComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(1, 0); // Lower right corner
            rectTransform.anchorMax = new Vector2(1, 0); // Lower right corner
            rectTransform.pivot = new Vector2(1, 0); // Pivot in the lower right corner
            rectTransform.sizeDelta = new Vector2(32, 32); // Match the size of the image
            // rectTransform.anchoredPosition = new Vector2(-4, 4); // Padding to the right and bottom

            // Add and configure TextMeshProUGUI component
            _countText = countTextObject.AddComponent<TextMeshProUGUI>();
            _countText.fontSize = 18; // Set the font size
            _countText.alignment = TextAlignmentOptions.BottomRight; // Align text to the lower right
            _countText.color = Color.white; // Set text color to white, adjust as needed
            _countText.raycastTarget = false;

        }

    }

    private void UpdateCountText()
    {
        EnableCountText();
        _countText.text = count.ToString();
    }

    public object Clone()
    {
        if (this.gameObject == null)
        {
            Debug.LogError("GameObject is null, cannot clone BaseItem.");
            return null;
        }

        GameObject clonedObject = Instantiate(gameObject);
    

        BaseItem cloneBaseItem = clonedObject.GetComponent<BaseItem>();
        if (cloneBaseItem == null)
        {
            Debug.LogError("Clone failed: No BaseItem component found on the cloned object.");
            Destroy(clonedObject); // Cleanup the cloned object if the expected component is missing
            return null;
        }

        cloneBaseItem.count = this.count;
        return clonedObject;
    }

    public GameObject CloneWithCount(int newCount)
    {
        GameObject clone = (GameObject) Clone();
        BaseItem clonedBaseItem = clone.GetComponent<BaseItem>();
        clonedBaseItem.SetCount(newCount);
        return clone;
    }

    public void Use(InputAction.CallbackContext context)
    {
        switch (baseInfo.ItemCategory)
        {
            case ItemCategory.Food:
                break;
            case ItemCategory.Potion:
                break;
        }    
    }

    public void ToggleMapPresence(bool makePresentOnMap)
    {
        if (makePresentOnMap && sr == null)
        {
            sr = gameObject.AddComponent<SpriteRenderer>();
            bc = gameObject.AddComponent<BoxCollider2D>();
            bc.size = new Vector2(1, 1);
            gameObject.layer = LayerMask.NameToLayer("Items");
            sr.sprite = image.sprite;
            transform.localScale = Vector3.one;
            sr.sortingLayerName = "Background";
            image.enabled = false;
        }
        else if (!makePresentOnMap)
        {
            if(sr) Destroy(sr);
            if(bc) Destroy(bc);
            GetComponent<RectTransform>().localScale = Vector3.one;
            image.enabled = true;
        }
        

    }
    
    
}