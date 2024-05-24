// using System.Collections;
// using System.Collections.Generic;
// using DefaultNamespace;
// using UnityEngine;
// using UnityEngine.AI;
// using UnityEngine.InputSystem;
// using UnityEngine.Serialization;
// using Random = UnityEngine.Random;
//
// [
//     RequireComponent(typeof(Rigidbody2D)),
//     RequireComponent(typeof(ActorCombatManager)),
//     RequireComponent(typeof(ObjectInteractionManager)),
//     RequireComponent(typeof(Animator))
// ]
// public class Monster : MonoBehaviour
// {
//     protected Rigidbody2D rb { get; private set; }
//     [SerializeField] protected MonsterBaseInfo mbi;
//     public IntStats monsterStats;
//     const float OverlapCircleRadius = 0.45f;
//     [SerializeField] protected Animator animator;
//     [SerializeField] protected ObjectInteractionManager oim;
//     [SerializeField] protected ActorCombatManager pcm;
//     [SerializeField] protected ActorCombatManager playerAcm;
//     protected NavMeshPath nmp;
//     [SerializeField] protected GridManager gm;
//     [SerializeField] protected TileReservationManager trm;
//
//     public Transform playerTransform;
//     public LayerMask obstaclesLayerMask;
//     public LayerMask monsterLayerMask;
//     public ContactFilter2D contactFilter;
//     public List<Collider2D> raycastHits;
//     protected Vector3 startPosition;
//     protected Vector3 currentDestination;
//     protected int currentDirectionIndex;
//     protected int attemptedDirectionIndex;
//
//     protected float moveSpeed;
//     protected bool isMoving = false;
//
//     [SerializeField] protected string currentAnimationState;
//
//     private void SetUpContactFilter()
//     {
//         contactFilter = new ContactFilter2D();
//         contactFilter.layerMask = LayerMask.GetMask("Monsters", "Obstacles", "Player");
//         contactFilter.useLayerMask = true;
//
//     }   
//     
//     void Start()
//     {
//         playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
//         playerAcm = playerTransform.gameObject.GetComponent<ActorCombatManager>();
//         // obstaclesLayerMask |= (1 << LayerMask.NameToLayer("Containers"));
//         obstaclesLayerMask |= (1 << LayerMask.NameToLayer("Monsters"));
//         obstaclesLayerMask |= (1 << LayerMask.NameToLayer("Obstacles"));
//         obstaclesLayerMask |= (1 << LayerMask.NameToLayer("Player"));
//
//         monsterLayerMask |= (1 << LayerMask.NameToLayer("Monsters"));
//         SetUpContactFilter();
//         rb = GetComponent<Rigidbody2D>();
//         animator = GetComponent<Animator>();
//         oim = GetComponent<ObjectInteractionManager>();
//         if (oim != null)
//         {
//             oim.OnInteractableClick += OnClick;
//         }
//
//         pcm = GetComponent<ActorCombatManager>();
//         pcm.corpsePrefab = mbi.corpsePrefab;
//         startPosition = rb.position;
//         currentDirectionIndex = 3;
//         moveSpeed = monsterStats.GetIntStat(IntStatInfoType.MoveSpeed);
//         nmp = new NavMeshPath();
//         gm = GridManager.instance;
//         trm = TileReservationManager.Instance;
//         StartCoroutine(BehaviourLoop());
//     }
//
//     private void Update()
//     {
//         UpdateAnimator();
//     }
//
//     protected virtual IEnumerator BehaviourLoop()
//     {
//         while (true)
//         {
//             switch (DetermineState())
//             {
//                 case MonsterState.Idle:
//                     Idle();
//                     break;
//                 case MonsterState.Pursue:
//                     Pursue();
//                     break;
//                 case MonsterState.Return:
//                     ReturnToStart();
//                     break;
//             }
//
//             yield return new WaitForSeconds(.5f); // Check every half second
//         }
//     }
//
//     protected virtual void Idle()
//     {
//         if (!isMoving)
//         {
//             MoveInRandomDirection();
//         }
//     }
//
//     protected virtual void Pursue()
//     {
//         if (!isMoving)
//         {
//             FollowPathTowards(currentDestination);
//         }
//     }
//
//     protected void FollowPathTowards(Vector3 target)
//     {
//         NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, path: nmp);
//
//         if (nmp != null && nmp.corners.Length > 1)
//         {
//             Vector2 nextStep = nmp.corners[1];
//
//             Vector2 directionToNextStep = (nextStep - (Vector2)transform.position).normalized;
//
//             MoveInDirection(directionToNextStep);
//         }
//     }
//
//     protected void MoveInDirection(Vector2 direction)
//     {
//         Vector2 closestDirection = GetClosestDirection(direction);
//         TryMove(closestDirection);
//     }
//
//     protected void ReturnToStart()
//     {
//         if (!isMoving)
//         {
//             FollowPathTowards(currentDestination);
//         }
//     }
//
//     private void MoveInRandomDirection()
//     {
//         attemptedDirectionIndex = Random.Range(0, MovementConstants.DIRECTIONS.Length);
//         Vector2 chosenDirection = MovementConstants.DIRECTIONS[attemptedDirectionIndex];
//         TryMove(chosenDirection);
//     }
//
//     private void MoveTowardsPlayer()
//     {
//         Vector2 directionToPlayer = ((Vector2)playerTransform.position - rb.position).normalized;
//         Vector2 chosenDirection = GetClosestDirection(directionToPlayer);
//         TryMove(chosenDirection);
//     }
//
//     private Vector2 GetClosestDirection(Vector2 direction)
//     {
//         float maxDot = -Mathf.Infinity;
//         Vector2 closestDir = Vector2.zero;
//
//         for (int i = 0; i < MovementConstants.DIRECTIONS.Length; i++)
//         {
//             Vector2 dir = MovementConstants.DIRECTIONS[i];
//             float dot = Vector2.Dot(dir.normalized, direction.normalized);
//             if (dot > maxDot)
//             {
//                 attemptedDirectionIndex = i;
//                 maxDot = dot;
//                 closestDir = dir;
//             }
//         }
//
//         return closestDir;
//     }
//
//     private void TryMove(Vector2 direction)
//     {
//         if (!isMoving)
//         {
//             Vector2 start = transform.position;
//             Vector2 targetTile = start + direction;
//             Vector2 end = new Vector2(Mathf.Floor(targetTile.x) + .5f, Mathf.Floor(targetTile.y) + .5f);
//             Vector2Int endInt = Vector2Int.FloorToInt(end);
//             // Check for obstacles using the LayerMask
//             if (!IsCollision(end))
//             {
//                 DrawDebugCircle(end, OverlapCircleRadius, Color.blue);
//
//                 // Additional check for other monsters
//                 if (IsCollision(end))
//                 {
//                     Debug.Log("Monster collision in " + name);
//                     // Attempt to find a sidestep direction
//                     Vector2 sidestepDirection = FindNextBestDirection(end);
//                     DrawDebugCircle(end, OverlapCircleRadius, Color.red);
//
//                     if (sidestepDirection != Vector2.zero)
//                     {
//                         end = start + sidestepDirection;
//                         endInt = Vector2Int.FloorToInt(end);
//                         if (trm.ReserveTile(endInt, gameObject)) // Reserve the target tile
//                         {
//                             // Found a valid sidestep direction, move there instead
//                             StartCoroutine(Move(end));
//                             trm.ReleaseTile(Vector2Int.FloorToInt(start), gameObject); // Release the reservation upon arrival
//                         }
//                     }
//                     // No valid sidestep found, might choose to wait or handle differently
//                 }
//                 else
//                 {
//                     // No collision with other monsters, proceed with the move
//                     currentDirectionIndex = attemptedDirectionIndex;
//                     endInt = Vector2Int.FloorToInt(end);
//                     if (trm.ReserveTile(endInt, gameObject)) // Reserve the target tile
//                     {
//                         // Found a valid sidestep direction, move there instead
//                         StartCoroutine(Move(end));
//                         trm.ReleaseTile(Vector2Int.FloorToInt(start), gameObject); // Release the reservation upon arrival
//                     }
//                 }
//             }
//         }
//     }
//
//     private bool IsCollision(Vector2 moveTarget)
//     {
//         Vector2Int moveTargetInt = Vector2Int.FloorToInt(moveTarget);
//         return Physics2D.OverlapCircle(moveTarget, OverlapCircleRadius, contactFilter, raycastHits) > 0 || trm.IsTileReserved(moveTargetInt);
//     }
//
//     // Attempts to find a valid direction to sidestep around another monster
//     protected Vector2 FindNextBestDirection(Vector2 currentTarget)
//     {
//         float bestScore = float.MaxValue;
//         Vector2 bestDir = Vector2.zero;
//         foreach (Vector2 dir in MovementConstants.DIRECTIONS)
//         {
//             Vector2 potentialTarget = currentTarget + dir;
//             Vector2Int potentialTargetInt = Vector2Int.FloorToInt(potentialTarget);
//             if (!Physics2D.OverlapCircle(potentialTarget, OverlapCircleRadius, obstaclesLayerMask | monsterLayerMask) &&
//                 !trm.IsTileReserved(potentialTargetInt))
//             {
//                 // Calculate a score for this direction (e.g., based on distance to target)
//                 float score = (potentialTarget - (Vector2)playerTransform.position).sqrMagnitude;
//                 if (score < bestScore)
//                 {
//                     bestScore = score;
//                     bestDir = dir;
//                 }
//             }
//         }
//         // int count = rb.Cast(step, contactFilter2D, hits, speed);
//
//         return bestDir; // Might be Vector2.zero if no good direction is found
//     }
//     private IEnumerator Move(Vector2 destination)
//     {
//         isMoving = true;
//         Vector2 step;
//         
//         while ((Vector2)rb.position != destination)
//         {
//             step = Vector2.MoveTowards(rb.position, destination, moveSpeed * Time.deltaTime);
//             rb.MovePosition(step);
//             yield return null;
//         }
//
//         isMoving = false;
//     }
//
//     protected MonsterState DetermineState()
//     {
//         float distanceToPlayer = Vector3.Distance(playerTransform.position, rb.position);
//         float distanceFromStart = Vector3.Distance(rb.position, startPosition);
//
//         if (distanceToPlayer <= monsterStats.GetIntStat(IntStatInfoType.DetectionRadius))
//         {
//             pcm.currentTarget = playerAcm;
//             currentDestination = playerTransform.position;
//             return MonsterState.Pursue;
//         }
//         if (distanceFromStart > monsterStats.GetIntStat(IntStatInfoType.ReturnRadius))
//         {
//             pcm.currentTarget = null;
//             currentDestination = startPosition;
//             return MonsterState.Return;
//         }
//
//         pcm.currentTarget = null;
//         return MonsterState.Idle;
//     }
//
//     protected enum MonsterState
//     {
//         Idle,
//         Pursue,
//         Return
//     }
//
//
//     public void OnClick(InputAction.CallbackContext context)
//     {
//         ActorCombatManager playerAcm = playerTransform.gameObject.GetComponent<ActorCombatManager>();
//         Debug.Log("OnClick in Monster.");
//         Debug.Log("Players pcm = " + playerAcm);
//         if (playerAcm.currentTarget == pcm)
//         {
//             playerAcm.currentTarget = null;
//             Debug.Log("Player target cleared.");
//         }
//         else
//         {
//             playerAcm.currentTarget = pcm;
//             Debug.Log($"Player target set to {gameObject.name}.");
//         }
//     }
//
//
//     public void UpdateAnimator()
//     {
//         string newAnimationState;
//
//         if (isMoving)
//         {
//             newAnimationState =
//                 DefaultNamespace.MovementConstants.MOVE_ANIMATION_STATES
//                     [currentDirectionIndex]; // Use the index to select the correct moving animation
//         }
//         else
//         {
//             newAnimationState =
//                 DefaultNamespace.MovementConstants.IDLE_ANIMATION_STATES
//                     [currentDirectionIndex]; // Use the index to select the correct idle animation
//         }
//
//         if (currentAnimationState == newAnimationState)
//             return; // If the animation state hasn't changed, no need to update
//
//         animator.Play(newAnimationState);
//         currentAnimationState = newAnimationState;
//     }
//
//     void DrawDebugCircle(Vector2 center, float radius, Color color, float duration = 0.5f, int segments = 36)
//     {
//         float angleStep = 360.0f / segments;
//         Quaternion rotation = Quaternion.Euler(0, 0, angleStep);
//
//         Vector3 direction = radius * Vector3.right;
//         Vector3 lastPoint = center + (Vector2)direction;
//         Vector3 startPoint = lastPoint;
//
//         for (int i = 0; i < segments; i++)
//         {
//             direction = rotation * direction;
//             Vector3 nextPoint = center + (Vector2)direction;
//             Debug.DrawLine(lastPoint, nextPoint, color, duration);
//             lastPoint = nextPoint;
//         }
//
//         // Connect the last point with the start point for a complete circle
//         Debug.DrawLine(lastPoint, startPoint, color, duration);
//     }
// }