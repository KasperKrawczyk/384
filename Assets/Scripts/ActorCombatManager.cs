using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

[
    RequireComponent(typeof(HealthBarManager))
]
public class ActorCombatManager : MonoBehaviour, IDamageable
{
    
    [Flags]
    public enum CombatType
    {
        None = 0,
        Melee = 1,
        Ranged = 2
    }

    [SerializeField] protected CombatType combatCapabilities = CombatType.Melee | CombatType.Ranged; // Default to both
    [SerializeField] public GameObject overheadDisplayPrefab;
    [SerializeField] public GameObject overheadDisplayInstance;
    [SerializeField] public Canvas worldCanvas;

    [SerializeReference] public AHealthDisplayer healthDisplayManager;
    [SerializeField] public OverheadDisplayManager overheadDisplayManager;

    [SerializeField] protected Random rng = new Random();
    [SerializeField] public int maxHealth;
    [SerializeField] public int CurHealth { get; set; }
    [SerializeField] protected int maxMeleeDamage = 1;
    [SerializeField] protected int minMeleeDamage = 1;
    [SerializeField] protected int maxRangedDamage = 1;
    [SerializeField] protected int minRangedDamage = 1;
    [SerializeField] protected float meleeRange = 2f;
    [SerializeField] protected float minRangeDistance = 2f; // Minimum for ranged attack
    [SerializeField] protected float maxRangeDistance = 5f; // Maximum for ranged attack
    [SerializeField] public MonsterBaseInfo mbi;
    [SerializeField] public IntStats actorStats;

    [SerializeField] private float attackCooldown = 1f; // Time between attacks

    [SerializeField] public bool IsAlive { get; protected set; } = true;

    [SerializeField] protected GameObject meleeEffectPrefab; 
    [SerializeField] protected GameObject hitTakenPrefab;
    [SerializeField] protected GameObject hitDeflectedPrefab;
    [SerializeField] protected GameObject rangeEffectPrefab;
    [SerializeField] protected ActorCombatManager _currentTarget;
    public ActorCombatManager currentTarget
    {
        get => _currentTarget;
        set
        {
            if (_currentTarget != value)
            {
                _currentTarget = value;
                
                if (_currentTarget != null)
                {
                    if (attackRoutine != null)
                    {
                        StopCoroutine(attackRoutine);
                    }
                    attackRoutine = StartCoroutine(AttackRoutine()); // Start the new attack routine and store the reference
                    
                }
                else
                {
                    if (attackRoutine != null)
                    {
                        StopCoroutine(attackRoutine);
                        attackRoutine = null; // Clear the reference
                        
                    }
                }
            }
        }
    }



    private Coroutine attackRoutine = null;

    public Rigidbody2D rb { get; private set; }

    public virtual void Awake()
    {
        worldCanvas = GameObject.Find("GameCanvas").GetComponent<Canvas>();
        maxHealth = (int) actorStats.GetStat(IntStatInfoType.Health);
        maxMeleeDamage = (int) actorStats.GetStat(IntStatInfoType.MaxMeleeDamage);
        minMeleeDamage = (int) actorStats.GetStat(IntStatInfoType.MinMeleeDamage);
        maxRangedDamage = (int) actorStats.GetStat(IntStatInfoType.MaxRangedDamage);
        minRangedDamage = (int) actorStats.GetStat(IntStatInfoType.MinRangedDamage);
        CurHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Start()
    {
        InitialiseOverheadDisplay();
    }

    void Update()
    {
        // if (currentTarget != null)
        // {
        //     // Only start the coroutine if it's not already running
        //     if (!isCoroutineRunning)
        //     {
        //         StartCoroutine(AttackRoutine());
        //     }
        // }
        // else
        // {
        //     if (isCoroutineRunning)
        //     {
        //         StopAllCoroutines(); // Stop the coroutine if there's no target
        //         isCoroutineRunning = false;
        //     }
        // }
    }

    public bool IsTargetInMeleeRange()
    {
        return Vector2.Distance(rb.position, currentTarget.rb.position) <= meleeRange;
    }

    public bool IsTargetInRangedAttackRange()
    {
        float distance = Vector2.Distance(rb.position, currentTarget.rb.position);
        return distance >= minRangeDistance && distance <= maxRangeDistance;
    }

    public virtual void TakeDamage(DamageType damageType, int damageSent)
    {
        SpawnPrefabHere(hitTakenPrefab);
        int damageSuffered = MonsterCombatMathUtils.GetDamageSuffered(damageSent, actorStats.GetStat(IntStatInfoType.Armour));
        Debug.Log($"{this.name} got sent {damageSent}. It suffered {damageSuffered} damage.");
        CurHealth -= damageSuffered;
        CurHealth = Math.Max(CurHealth, 0);
        overheadDisplayManager.UpdateHealth(actorStats.GetStat(IntStatInfoType.Health), CurHealth);
        if (CurHealth == 0)
        {
            Die();
        }
        
    }
    
    IEnumerator AnimatePrefabHere(GameObject prefab)
    {
        while (prefab && Vector2.Distance(prefab.transform.position, transform.position) > 0.1f)
        {
            // Move the projectile towards the target
            prefab.transform.position = Vector2.MoveTowards(prefab.transform.position, transform.position, 10f * Time.deltaTime);
            yield return null;
        }
    
        if (prefab)
        {
            Destroy(prefab); // delay if needed
        }
    }

    // public void Attack()
    // {
    //     if (currentTarget == null) return;
    //
    //     
    //     if (IsTargetInMeleeRange())
    //     {
    //         // Perform melee attack
    //         SpawnMeleeEffect();
    //         // Apply damage, etc.
    //         currentTarget.TakeDamage(DamageType.PhysicalDamage, rng.Next(minMeleeDamage, maxMeleeDamage));
    //     }
    //     else if (IsTargetInRangedAttackRange())
    //     {
    //         // Perform ranged attack
    //         SpawnRangedEffect();
    //         // Apply damage, etc.
    //         currentTarget.TakeDamage(DamageType.PhysicalDamage, rng.Next(minRangedDamage, maxRangedDamage));
    //     }
    // }

    protected virtual void SpawnMeleeEffect()
    {
        if (meleeEffectPrefab)
        {
            Instantiate(meleeEffectPrefab, currentTarget.transform.position, Quaternion.identity);
        }
    }

    protected void SpawnPrefabHere(GameObject prefab)
    {
        if (prefab)
        {
            GameObject effectInstance = Instantiate(prefab, transform.position, Quaternion.identity, transform.parent);
            // StartCoroutine(AnimatePrefabHere(effectInstance));

        }
    }
    
    void SpawnPrefabOnTarget(GameObject prefab, Vector2 target)
    {
        if (prefab)
        {
            GameObject effectInstance = Instantiate(prefab, target, Quaternion.identity);
        }
    }

    protected virtual void SpawnRangedEffect()
    {
        if (rangeEffectPrefab)
        {
            GameObject projectile = Instantiate(rangeEffectPrefab, transform.position, Quaternion.identity, transform.parent);
            Vector2 direction = (currentTarget.transform.position - transform.position).normalized;

            // Calculate the angle to rotate the projectile
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            projectile.transform.rotation = Quaternion.Euler(0f, 0f, angle - 45); 
            StartCoroutine(MoveProjectileTowardsTarget(projectile, currentTarget.transform.position));
        }
    }
    
    protected IEnumerator MoveProjectileTowardsTarget(GameObject projectile, Vector2 targetPosition)
    {
        while (projectile && Vector2.Distance(projectile.transform.position, targetPosition) > 0.1f)
        {
            // Move the projectile towards the target
            projectile.transform.position = Vector2.MoveTowards(projectile.transform.position, targetPosition, 10f * Time.deltaTime);
            yield return null;
        }
    
        // Destroy the projectile upon reaching the target or completing the animation
        if (projectile)
        {
            Destroy(projectile); 
            // Destroy(projectile, 0.2f); // delay
        }
    }
    



    private IEnumerator AttackRoutine()
    {
        
        while (currentTarget != null)
        {
            PerformAttack();
            yield return new WaitForSeconds(attackCooldown);
        }
    }

    protected virtual void PerformAttack()
    {
        
        if (IsTargetInMeleeRange() && combatCapabilities.HasFlag(CombatType.Melee))
        {
            
            // SpawnMeleeEffect();
            currentTarget.TakeDamage(DamageType.PhysicalDamage, rng.Next(minMeleeDamage, maxMeleeDamage));
        }
        else if (IsTargetInRangedAttackRange() && combatCapabilities.HasFlag(CombatType.Ranged))
        {
            
            SpawnRangedEffect();
            currentTarget.TakeDamage(DamageType.PhysicalDamage, rng.Next(minRangedDamage, maxRangedDamage));
        }
        else
        {
            
        }
    }

    
    protected virtual void Die()
    {
        Vector3 transformPosition = transform.position;
        Vector2 corpsePosition = new Vector2(Mathf.Floor(transformPosition.x) + .5f, Mathf.Floor(transformPosition.y) + .5f);
        // PlayerController.Instance.GetStatsManager().curExperiencePoints += (int) actorStats.GetIntStat(IntStatInfoType.Experience);
        
        
        GameObject itemPrefab = Resources.Load<GameObject>("Prefabs/Items/BaseItem");
        GameObject item = Instantiate(itemPrefab, corpsePosition, Quaternion.identity);

        BaseInfo baseInfo = AssetsDB.Instance.corpseBaseInfoDictionary[mbi.corpseBaseInfoName];
        item.GetComponent<BaseItem>().InitialiseFromBaseInfo(baseInfo, 0);
        item.GetComponent<RectTransform>().localScale = Vector3.one;
        
        Destroy(overheadDisplayInstance);
        Destroy(gameObject);
    }
    


    private void InitialiseOverheadDisplay()
    {
        overheadDisplayInstance = Instantiate(overheadDisplayPrefab);
        
        overheadDisplayInstance.transform.SetParent(worldCanvas.transform, false);

        overheadDisplayInstance.GetComponent<RectTransform>().localPosition = new Vector3(0, 2, 0); 

        overheadDisplayManager = overheadDisplayInstance.GetComponent<OverheadDisplayManager>();
        overheadDisplayManager.nameText.text = mbi.name;
        overheadDisplayManager.targetTransform = transform;
        overheadDisplayManager.UpdateHealth(maxHealth, CurHealth);
    }
    

}
