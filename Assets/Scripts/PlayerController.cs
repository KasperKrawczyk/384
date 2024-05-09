using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[
    RequireComponent(typeof(Rigidbody2D)),
    RequireComponent(typeof(PlayerCombatManager)),
    RequireComponent(typeof(ObjectInteractionManager)),
    RequireComponent(typeof(Animator)),
]
public class PlayerController : MonoBehaviour
{
    public static event Action<Vector2> OnMoveAction;
    public static PlayerController Instance;

    [Header("Movement")]
    public LayerMask obstaclesMask;
    protected Rigidbody2D rb { get; private set; }
    private Vector2 movementInputVec = Vector2.zero;
    
    public float moveSpeed = 5f;
    private Vector2 currentTarget;
    private string currentAnimationState;
    private int currentDirectionIndex;
    private bool isMoving = false;
    
    [SerializeField] public Animator Animator { get; private set; }
    [SerializeField] public SpriteRenderer SpriteRenderer { get; private set; }
    [SerializeField] protected ObjectInteractionManager oim;
    [SerializeField] protected MonsterBaseInfo mbi; 
    [SerializeField] protected PlayerCombatManager pcm { get; private set;  } 
    

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        Animator = GetComponent<Animator>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        oim = GetComponent<ObjectInteractionManager>();
        pcm = GetComponent<PlayerCombatManager>();
        SetCanAttack(false);
        currentTarget = rb.position;
        currentDirectionIndex = 3;
        
        oim.OnInteractableClick += OnClick;

    }
    
    private void OnMove(InputValue value)
    {
        movementInputVec = value.Get<Vector2>();
        if (movementInputVec != Vector2.zero && !isMoving)
        {
            OnMoveAction?.Invoke(movementInputVec);
            SetNewTarget(movementInputVec);
        }
    }
    
    public void OnClick(InputAction.CallbackContext context)
    {
        Debug.Log("I got clicked!");
    }
    
    private void OnClickToMove(InputValue value)
    {
        Debug.Log("value.Get<Vector2>(): " + value.Get<Vector2>());
    }
    
    void Update()
    {
        // Animator update for movement visualization
        UpdateAnimator();
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            MoveTowardsTarget();
            
        }
        else if (movementInputVec != Vector2.zero) 
        {
            SetNewTarget(movementInputVec);
        }
    }

    private void SetNewTarget(Vector2 direction)
    {
        // calculate the potential target considering one tile move
        Vector2 potentialTarget = rb.position + direction;
    
        // align the target with the grid by rounding to the nearest whole number
        // this is critical for diagonal movements to ensure landing in the center of a tile
        potentialTarget.x = Mathf.Floor(potentialTarget.x) + .5f;
        potentialTarget.y = Mathf.Floor(potentialTarget.y) + .5f;

        if (IsWalkable(potentialTarget))
        {
            currentTarget = potentialTarget;
            isMoving = true;
            SetDirectionIndex(direction); // Update direction index here

        }
        else
        {
            isMoving = false;
        }
    }

    private void MoveTowardsTarget()
    {
        if (isMoving)
        {
            rb.position = Vector2.MoveTowards(rb.position, currentTarget, moveSpeed * Time.fixedDeltaTime);
            if (Vector2.Distance(rb.position, currentTarget) < 0.01f)
            {            
                rb.position = currentTarget; // Ensure precise alignment with the target tile
                isMoving = false; // Allow for the next movement initiation
            }
        }
    }

    bool IsWalkable(Vector2 targetPosition)
    {
        Collider2D hit = Physics2D.OverlapCircle(targetPosition, 0.25f, obstaclesMask);

        return hit == null; // Target is walkable if no obstacle is hit
    }


    
    private void SetDirectionIndex(Vector2 direction)
    {
        float maxDot = -Mathf.Infinity;
        int closestDirectionIndex = 0;

        for (int i = 0; i < DefaultNamespace.MovementConstants.DIRECTIONS.Length; i++)
        {
            Vector2 normalisedDir = DefaultNamespace.MovementConstants.DIRECTIONS[i].normalized;
            float dot = Vector2.Dot(direction, normalisedDir);

            if (dot > maxDot)
            {
                maxDot = dot;
                closestDirectionIndex = i;
            }
        }

        currentDirectionIndex = closestDirectionIndex;
    }

    public void UpdateAnimator()
    {
        string newAnimationState;
        
        if (isMoving)
        {
            
            newAnimationState = DefaultNamespace.MovementConstants.MOVE_ANIMATION_STATES[currentDirectionIndex]; // Use the index to select the correct moving animation
        }
        else
        {
            newAnimationState = DefaultNamespace.MovementConstants.IDLE_ANIMATION_STATES[currentDirectionIndex]; // Use the index to select the correct idle animation
        }
    
        if (currentAnimationState == newAnimationState) return; // If the animation state hasn't changed, no need to update
    
        Animator.Play(newAnimationState);
        currentAnimationState = newAnimationState;
    }
    
    public T GetChildComponentByName<T>(string childName) where T : Component
    {
        Transform child = transform.Find(childName);
        if (child != null)
        {
            T component = child.GetComponent<T>();
            if (component != null)
            {
                return component;
            }
        }
        return null;
    }

    public void SetCanAttack(bool canAttack)
    {
        pcm.SetCanAttack(canAttack);
    }

    public StatsManager GetStatsManager()
    {
        return pcm.statsManager;
    }
    
}
