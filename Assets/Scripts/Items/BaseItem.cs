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
public class BaseItem : MonoBehaviour, ICloneable {
    [SerializeField] public BaseInfo baseInfo;
    [SerializeField] public GameObject useEffectPrefab;
    [SerializeField] public SpriteRenderer sr;
    [SerializeField] public BoxCollider2D bc;
    [SerializeField] public SlotPanelManager Spm;
    [SerializeField] public Image image;
    [SerializeField] public int count;
    [HideInInspector] public ItemDragManager idm;
    private TextMeshProUGUI _countText;
    private GameObject _countTextObject;
    private ObjectInteractionManager _oim;

    public virtual void Awake() {
        image = GetComponent<Image>();
        if (baseInfo != null) {
            if (baseInfo.GetBoolStat(BoolStatInfoType.Usable)) {
                _oim = this.GetComponent<ObjectInteractionManager>();
                _oim.OnInteractableClick += Use;
            }
        }

        idm = GetComponent<ItemDragManager>();
    }

    private void Start() {
        if (baseInfo == null) {
            return;
        }

        _countTextObject = transform.Find("CountTextObject")?.gameObject;
        if (baseInfo.GetBoolStat(BoolStatInfoType.Stackable)) {
            if (_countTextObject != null) {
                _countText = _countTextObject.GetComponent<TextMeshProUGUI>();
                SetCount(count);
            }
            else {
                Debug.LogError("Stackable item is missing CountTextObject.");
            }
        }
        else {
            if (_countTextObject != null) {
                Destroy(_countTextObject);
                _countTextObject = null;
            }
        }

        if (!baseInfo.GetBoolStat(BoolStatInfoType.Pickupable)) {
            Destroy(idm);
        }
    }

    public void InitialiseFromBaseInfo(BaseInfo bi, int count) {
        baseInfo = bi;
        image = GetComponent<Image>();
        if (baseInfo != null) {
            image.sprite = AssetsDB.Instance.spriteDictionary[baseInfo.spriteId];
            if (baseInfo.GetBoolStat(BoolStatInfoType.Usable) && _oim == null) {
                _oim = GetComponent<ObjectInteractionManager>();
                _oim.OnInteractableClick += Use;
            }

            if (!string.IsNullOrEmpty(baseInfo.useEffectId) && baseInfo.WeaponType == WeaponType.Dist) {
                Sprite useEffectSprite = AssetsDB.Instance.spriteDictionary[baseInfo.useEffectId];
                useEffectPrefab = Resources.Load<GameObject>("Prefabs/CombatEffects/Ranged/RangedCombatEffectPrefab");
                useEffectPrefab.GetComponent<SpriteRenderer>().sprite = useEffectSprite;
            }

            if (baseInfo.GetBoolStat(BoolStatInfoType.Stackable)) {
                this.count = count;
                if (_countTextObject != null) {
                    _countText = _countTextObject.GetComponent<TextMeshProUGUI>();
                    SetCount(count);
                }
            }
            else {
                if (_countTextObject != null) {
                    Destroy(_countTextObject);
                    _countTextObject = null;
                }
            }
            
        }
    }

    public void SetCount(int count) {
        this.count = count;
        if (this.count == 0) {
            this.Spm.SetCurrentItem(null);
            Destroy(gameObject);
            return;
        }

        UpdateCountText();
    }

    private void UpdateCountText() {
        if (_countText != null) {
            _countText.text = count.ToString();
        }
    }

    public object Clone() {
        if (this.gameObject == null) {
            Debug.LogError("GameObject is null, cannot clone BaseItem.");
            return null;
        }

        GameObject clonedObject = Instantiate(gameObject);
        BaseItem cloneBaseItem = clonedObject.GetComponent<BaseItem>();
        if (cloneBaseItem == null) {
            Debug.LogError("Clone failed: No BaseItem component found on the cloned object.");
            Destroy(clonedObject); // Cleanup the cloned object if the expected component is missing
            return null;
        }

        Debug.Log("BaseItem cloned and count text object initialized for the clone.");
        return clonedObject;
    }

    public GameObject CloneWithCount(int newCount) {
        GameObject clone = (GameObject)Clone();
        BaseItem clonedBaseItem = clone.GetComponent<BaseItem>();
        clonedBaseItem.SetCount(newCount);
        return clone;
    }

    public void Use(InputAction.CallbackContext context) {
        switch (baseInfo.ItemCategory) {
            case ItemCategory.Food:
                break;
            case ItemCategory.Potion:
                break;
        }
    }

    public void ToggleMapPresence(bool makePresentOnMap) {
        if (makePresentOnMap && sr == null) {
            sr = gameObject.AddComponent<SpriteRenderer>();
            bc = gameObject.AddComponent<BoxCollider2D>();
            bc.size = new Vector2(1, 1);
            gameObject.layer = LayerMask.NameToLayer("Items");
            sr.sprite = image.sprite;
            transform.localScale = Vector3.one;
            sr.sortingLayerName = "Background";
            image.enabled = false;
            this.Spm.SetCurrentItem(null);
            this.Spm = null;
        }
        else if (!makePresentOnMap) {
            if (sr) Destroy(sr);
            if (bc) Destroy(bc);
            GetComponent<RectTransform>().localScale = Vector3.one;
            // gameObject.layer = LayerMask.NameToLayer("UI");
            image.enabled = true;
        }
    }
}