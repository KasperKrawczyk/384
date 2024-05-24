using System;
using System.Collections;
using DefaultNamespace;
using UnityEngine;

[
    RequireComponent(typeof(StatsManager))
]
public class PlayerCombatManager : ActorCombatManager
{
    public static event Action OnDieAction;
    private const float CombatTimeout = 60.0f;

    [SerializeReference] public StatsManager statsManager;
    
    [SerializeField] private SlotPanelManager weaponSlot;
    [SerializeField] private SlotPanelManager armourSlot;
    [SerializeField] private SlotPanelManager bootsSlot;
    [SerializeField] private SlotPanelManager shieldSlot;
    [SerializeField] private bool canAttack = true;
    
    private bool _isEngagedInCombat = false;
    private int _blockCount = 2;
    private Coroutine _resetBlockCountCoroutine;
    private float _lastDamageTime;
    
    public override void Awake()
    {
        base.Awake();
        healthDisplayManager = GetComponent<HealthBarManager>();
        statsManager = GetComponent<StatsManager>();

    }

    protected override void Start()
    {
        base.Start();
        UpdateHealthDisplay(curHealth);
    }

    private IEnumerator ResetBlockCount()
    {
        while (_isEngagedInCombat)
        {
            yield return new WaitForSeconds(2f);
            _blockCount = 2;
            if (Time.time - _lastDamageTime >= CombatTimeout)
            {
                _isEngagedInCombat = false;
                StopCoroutine(_resetBlockCountCoroutine);
                _resetBlockCountCoroutine = null;
                break;
            }
        }
    }

    protected override void PerformAttack()
    {
        BaseItem baseItem = weaponSlot.GetCurrentItem();
        if (baseItem)
        {
            if (IsTargetInMeleeRange() &&
                (baseItem.baseInfo.WeaponType == WeaponType.Sword || 
                 baseItem.baseInfo.WeaponType == WeaponType.Axe || 
                 baseItem.baseInfo.WeaponType == WeaponType.Club))
            {
            
                // SpawnMeleeEffect();
                currentTarget.TakeDamage(DamageType.PhysicalDamage, CalculateDamage(IntStatInfoType.Melee));
            }
            else if (IsTargetInRangedAttackRange() && (baseItem.baseInfo.WeaponType == WeaponType.Dist))
            {
            
                SpawnRangedEffect();
                currentTarget.TakeDamage(DamageType.PhysicalDamage, CalculateDamage(IntStatInfoType.Distance));
            }   
        } else
        {
            currentTarget.TakeDamage(DamageType.PhysicalDamage, CalculateDamage(IntStatInfoType.Melee));

        }
    }
    
    public override void TakeDamage(DamageType damageType, int damageSent)
    {
        if (_blockCount > 0) _blockCount--;
        _isEngagedInCombat = true;
        _lastDamageTime = Time.time;
        if (_resetBlockCountCoroutine == null)
        {
            _resetBlockCountCoroutine = StartCoroutine(ResetBlockCount());
        }
        
        int damageSuffered = CalculateDamageSuffered(damageSent, (_blockCount > 0));
        Debug.Log($"{this.name} got sent {damageSent}. It suffered {damageSuffered} damage.");
        curHealth -= damageSuffered;
        if (curHealth < 0)
        {
            curHealth = 0;
        }
        if (damageSuffered > 0) SpawnPrefabHere(hitTakenPrefab);
        else SpawnPrefabHere(hitDeflectedPrefab); 
        
        UpdateHealthDisplay(curHealth);

        if (curHealth == 0)
        {
            Die();
            IsAlive = false;
        }
        
    }

    private int CalculateDamage(IntStatInfoType combatSkillType)
    {
        BaseItem baseItem = weaponSlot.GetCurrentItem();
        int level = statsManager.GetSkillLevel(IntStatInfoType.Level);
        int skillLevel = statsManager.GetSkillLevel(combatSkillType);
        int weaponAttack = 1;
        if (baseItem != null)
        {
           weaponAttack = weaponSlot.GetCurrentItem().baseInfo.GetIntStat(IntStatInfoType.Attack);
        }
        
        if (combatSkillType == IntStatInfoType.Distance)
        {
            return PlayerCombatMathUtils.GetDistanceDamage(level, skillLevel, weaponAttack);

        }
        else
        {
            return PlayerCombatMathUtils.GetMeleeDamage(level, skillLevel, weaponAttack);
        }
    }
    
    private int CalculateDamageSuffered(int damageSent, bool checkShield)
    {
        int totalArmour = 1;
        int totalDefence = 0;
        int shieldingSkillLevel = statsManager.GetSkillLevel(IntStatInfoType.Shielding);
        BaseItem baseItem = armourSlot.GetCurrentItem();
        if (baseItem != null)
        {
            totalArmour += (int)  baseItem.baseInfo.GetIntStat(IntStatInfoType.Armour);
        }
        baseItem = bootsSlot.GetCurrentItem();
        if (baseItem != null)
        {
            totalArmour += (int) baseItem.baseInfo.GetIntStat(IntStatInfoType.Armour);
        }

        if (checkShield)
        {
            baseItem = shieldSlot.GetCurrentItem();
            if (baseItem != null)
            {
                totalDefence += baseItem.baseInfo.GetIntStat(IntStatInfoType.Defence);
            }
            baseItem = weaponSlot.GetCurrentItem();
            if (baseItem != null)
            {
                totalDefence += baseItem.baseInfo.GetIntStat(IntStatInfoType.Defence);
            }
        }


        return PlayerCombatMathUtils.GetDamageSuffered(damageSent, totalDefence, totalArmour, shieldingSkillLevel);
    }
    
    
    protected override void SpawnMeleeEffect()
    {
        BaseItem baseItem = weaponSlot.GetCurrentItem();

        if (baseItem)
        {
            GameObject useEffectPrefab = baseItem.useEffectPrefab;
            Instantiate(useEffectPrefab, currentTarget.transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(meleeEffectPrefab, currentTarget.transform.position, Quaternion.identity);
            
        }
    }
    
    
    protected override void SpawnRangedEffect()
    {
        BaseItem baseItem = weaponSlot.GetCurrentItem();

        if (baseItem)
        {
            GameObject useEffectPrefab = baseItem.useEffectPrefab;

            GameObject projectile = Instantiate(useEffectPrefab, transform.position, Quaternion.identity, transform.parent);
            Vector2 direction = (currentTarget.transform.position - transform.position).normalized;

            // calculate the angle to rotate the projectile
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            projectile.transform.rotation = Quaternion.Euler(0f, 0f, angle - 45); 
            StartCoroutine(MoveProjectileTowardsTarget(projectile, currentTarget.transform.position));
        }
    }
    
    public void SetCanAttack(bool canAttack)
    {
        this.canAttack = canAttack;
    }
    
    protected override void Die()
    {
        OnDieAction?.Invoke();
        GameObject thisGameObject = gameObject;
        Vector3 transformPosition = transform.position;
        Vector2 corpsePosition = new Vector2(Mathf.Floor(transformPosition.x) + .5f, Mathf.Floor(transformPosition.y) + .5f);
        GameObject corpse = Instantiate(corpsePrefab, corpsePosition, Quaternion.identity);
        PlayerController.Instance.GetChildComponentByName<Camera>("Main Camera").transform.SetParent(corpse.transform);
        Destroy(overheadDisplayInstance);
        Destroy(thisGameObject);
    }
    
    protected void UpdateHealthDisplay(int curHealth)
    {
        overheadDisplayManager.UpdateHealth(actorStats.GetStat(IntStatInfoType.Health), curHealth);
        healthDisplayManager.UpdateHealth(actorStats.GetStat(IntStatInfoType.Health), curHealth);
    }
    
}