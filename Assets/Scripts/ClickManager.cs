using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class ClickManager : MonoBehaviour
{
    
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
        RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);
        Debug.Log("Hit on point: " + hit.point);
        if (hit.collider != null)
        {
            Debug.Log("hit.collider: " + hit.collider);

            IClickable clickable = hit.collider.GetComponent<IClickable>();
            if (clickable != null)
            {
                Debug.Log("Clickable hit!");

                clickable.OnClick(context);
            }
            else if (hit.collider.gameObject.GetComponent<TilemapCollider2D>() != null)
            {
                Debug.Log("Tile hit!: " + hit.collider);
                HandleTileClick(hit);
            }
        }
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
