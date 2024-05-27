using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class ClickManager : MonoBehaviour
{
    private int _backgroundLayer;
    private int _itemsLayer;
    private int _monstersLayer;
    private int _combinedLayerMask;
    public enum ClickType
    {
        MouseLeftClick,
        MouseRightClick,
        CtrlMouseLeftClick,
    }
    
    [SerializeField]
    private InputActionAsset inputActions;

    private InputActionMap actionMap;
    private InputAction attackAction;
    private InputAction showContextMenuAction;

    private Camera mainCamera;

    void Awake()
    {
        mainCamera = Camera.main;

        actionMap = inputActions.FindActionMap("InGame"); 
        attackAction = actionMap.FindAction("Attack");
        showContextMenuAction = actionMap.FindAction("ShowContextMenu");
        
        _backgroundLayer = LayerMask.NameToLayer("Background");
        _itemsLayer = LayerMask.NameToLayer("Items");
        _monstersLayer = LayerMask.NameToLayer("Monsters");
        _combinedLayerMask = (1 << _backgroundLayer) | (1 << _itemsLayer) | (1 << _monstersLayer);
        
        attackAction.performed += context => HandleClick(context);
        showContextMenuAction.performed += context => HandleClick(context);
    }

    void OnEnable()
    {
        attackAction.Enable();
        showContextMenuAction.Enable();
    }

    void OnDisable()
    {
        attackAction.Disable();
        showContextMenuAction.Disable();
    }

    private void HandleClick(InputAction.CallbackContext context)
    {
        Vector2 screenPosition = Mouse.current.position.ReadValue();
        Vector2 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(worldPosition, Vector2.zero, Mathf.Infinity, _combinedLayerMask);

        RaycastHit2D hit = new RaycastHit2D();
        bool objectHit = false;

        foreach (var h in hits)
        {
            int layerHit = h.collider.gameObject.layer;
            if (layerHit == _itemsLayer || layerHit == _monstersLayer)
            {
                hit = h;
                objectHit = true;
                break;
            }
            if (layerHit == _backgroundLayer && !objectHit)
            {
                hit = h;
            }
        }
        
        if (objectHit)
        {
            IClickable clickable = hit.collider.GetComponent<IClickable>();
            if (clickable != null)
            {
                Debug.Log("Clickable hit!");

                clickable.OnClick(context);
            }
        }
        else if (hit.collider != null && hit.collider.GetComponent<TilemapCollider2D>() != null)
        {
            Debug.Log("Tile hit!: " + hit.collider);
            HandleTileClick(hit);
        }
        
        // Debug.Log("Hit on point: " + hit.point);
        // if (hit.collider != null)
        // {
        //     Debug.Log("hit.collider: " + hit.collider);
        //
        //     IClickable clickable = hit.collider.GetComponent<IClickable>();
        //     if (clickable != null)
        //     {
        //         Debug.Log("Clickable hit!");
        //
        //         clickable.OnClick(context);
        //     }
        //     else if (hit.collider.gameObject.GetComponent<TilemapCollider2D>() != null)
        //     {
        //         Debug.Log("Tile hit!: " + hit.collider);
        //         HandleTileClick(hit);
        //     }
        // }
    }

    private void HandleTileClick(RaycastHit2D hit)
    {
        Tilemap tilemap = hit.collider.GetComponent<Tilemap>();
        Vector3 worldPosition = hit.point;
        Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);
        TileBase clickedTile = tilemap.GetTile(cellPosition);

        if (clickedTile != null)
        {
            Debug.Log($"Clicked on tile at {cellPosition}");
            // Trigger any specific action or event for the clicked tile
        }
    }
}
