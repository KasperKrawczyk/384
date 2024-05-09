using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

[
    RequireComponent(typeof(HealthBarManager)),
    RequireComponent(typeof(StatsManager))
]
public class ActorCombatManager : MonoBehaviour, IAttackable, IDamageable
{
    
    [Flags]
    public enum CombatType
    {
        None = 0,
        Melee = 1,
        Ranged = 2
    }

    [SerializeField] protected CombatType combatCapabilities = CombatType.Melee | CombatType.Ranged; // Default to both
    [SerializeField] public GameObject corpsePrefab;
    
    [SerializeReference] public AHealthDisplayer healthDisplayManager;
    [SerializeReference] public StatsManager statsManager;

    [SerializeField] protected Random rng = new Random();
    [SerializeField] public int maxHealth;
    [SerializeField] public int curHealth { get; protected set; }
    [SerializeField] protected int maxMeleeDamage = 1;
    [SerializeField] protected int minMeleeDamage = 1;
    [SerializeField] protected int maxRangedDamage = 1;
    [SerializeField] protected int minRangedDamage = 1;
    [SerializeField] protected float meleeRange = 1f;
    [SerializeField] protected float minRangeDistance = 2f; // Minimum for ranged attack
    [SerializeField] protected float maxRangeDistance = 5f; // Maximum for ranged attack
    [SerializeField] public FloatStats actorStats;

    [SerializeField] private float attackCooldown = 1f; // Time between attacks

    [SerializeField] private bool canAttack = true;
    [SerializeField] public bool IsAlive { get; protected set; } = true;

    [SerializeField] protected GameObject meleeEffectPrefab; 
    [SerializeField] protected GameObject hitTakenPrefab;
    [SerializeField] protected GameObject hitParriedPrefab;
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

    public void Awake()
    {
        statsManager = GetComponent<StatsManager>();
        healthDisplayManager = GetComponent<HealthBarManager>();
        maxHealth = (int) actorStats.GetStat(FloatStatInfoType.Health);
        maxMeleeDamage = (int) actorStats.GetStat(FloatStatInfoType.MaxMeleeDamage);
        minMeleeDamage = (int) actorStats.GetStat(FloatStatInfoType.MinMeleeDamage);
        maxRangedDamage = (int) actorStats.GetStat(FloatStatInfoType.MaxRangedDamage);
        minRangedDamage = (int) actorStats.GetStat(FloatStatInfoType.MinRangedDamage);
        curHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        UpdateHealthDisplayManager(curHealth);
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
        // Here run the defence checks
        SpawnPrefabHere(hitTakenPrefab);
        curHealth -= damageSent;
        UpdateHealthDisplayManager(curHealth);
        if (curHealth < 0)
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

    public void Attack()
    {
        if (currentTarget == null) return;
    
        
        if (IsTargetInMeleeRange())
        {
            // Perform melee attack
            SpawnMeleeEffect();
            // Apply damage, etc.
            currentTarget.TakeDamage(DamageType.PhysicalDamage, rng.Next(minMeleeDamage, maxMeleeDamage));
        }
        else if (IsTargetInRangedAttackRange())
        {
            // Perform ranged attack
            SpawnRangedEffect();
            // Apply damage, etc.
            currentTarget.TakeDamage(DamageType.PhysicalDamage, rng.Next(minRangedDamage, maxRangedDamage));
        }
    }

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
            
            SpawnMeleeEffect();
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
        GameObject thisGameObject = gameObject;
        Vector3 transformPosition = transform.position;
        Vector2 corpsePosition = new Vector2(Mathf.Floor(transformPosition.x) + .5f, Mathf.Floor(transformPosition.y) + .5f);
        PlayerController.Instance.GetStatsManager().curExperiencePoints += (int) actorStats.GetStat(FloatStatInfoType.Experience);
        Instantiate(corpsePrefab, corpsePosition, Quaternion.identity);
        Destroy(thisGameObject);
    }
    
    public void SetCanAttack(bool canAttack)
    {
        this.canAttack = canAttack;
    }

    protected void UpdateHealthDisplayManager(int curHealth)
    {
        if (healthDisplayManager != null)
        {
            healthDisplayManager.UpdateHealth(maxHealth, curHealth);
        }
    }

    public void ReceiveExperience(int experienceSent)
    {
        this.statsManager.AddExperiencePoints(experienceSent);
    }
}
