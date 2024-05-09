using System;
using DefaultNamespace;
using UnityEngine;

public class PlayerCombatManager : ActorCombatManager
{
    public static event Action OnDieAction;

    
    [SerializeField] private SlotPanelManager weaponSlot;
    [SerializeField] private SlotPanelManager armourSlot;
    [SerializeField] private SlotPanelManager bootsSlot;
    [SerializeField] private SlotPanelManager shieldSlot;
    
    
    protected override void PerformAttack()
    {
        BaseItem baseItem = weaponSlot.GetCurrentItem();
        if (baseItem)
        {
            if (IsTargetInMeleeRange() && ((Weapon) baseItem).CombatType == CombatType.Melee)
            {
            
                SpawnMeleeEffect();
                currentTarget.TakeDamage(DamageType.PhysicalDamage, CalculateDamage());
            }
            else if (IsTargetInRangedAttackRange() && ((Weapon) baseItem).CombatType == CombatType.Ranged)
            {
            
                SpawnRangedEffect();
                currentTarget.TakeDamage(DamageType.PhysicalDamage, CalculateDamage());
            }   
        } else
        {
            currentTarget.TakeDamage(DamageType.PhysicalDamage, CalculateDamage());

        }
    }
    
    public override void TakeDamage(DamageType damageType, int damageSent)
    {
        damageSent -= CalculateDefence();
        if (damageSent < 0)
        {
            damageSent = 0;
        }
        SpawnPrefabHere(hitTakenPrefab);
        curHealth -= damageSent;
        UpdateHealthDisplayManager(curHealth);
        if (curHealth < 0)
        {
            Die();
            IsAlive = false;
        }
        
    }

    private int CalculateDamage()
    {
        BaseItem baseItem = weaponSlot.GetCurrentItem();
        if (baseItem != null)
        {
            return (int) ((Weapon) weaponSlot.GetCurrentItem()).FloatStats.GetStat(FloatStatInfoType.Attack);
        }

        return rng.Next(minMeleeDamage, maxMeleeDamage);
    }
    
    private int CalculateDefence()
    {
        int defence = 0;
        BaseItem baseItem = armourSlot.GetCurrentItem();
        if (baseItem != null)
        {
            defence += (int) ((Armour) baseItem).FloatStats.GetStat(FloatStatInfoType.Armour);
        }
        baseItem = bootsSlot.GetCurrentItem();
        if (baseItem != null)
        {
            defence += (int) ((Armour) baseItem).FloatStats.GetStat(FloatStatInfoType.Armour);
        }
        baseItem = shieldSlot.GetCurrentItem();
        if (baseItem != null)
        {
            defence += (int) ((Armour) baseItem).FloatStats.GetStat(FloatStatInfoType.Armour);
        }

        return defence;
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
    
    protected override void Die()
    {
        OnDieAction?.Invoke();
        GameObject thisGameObject = gameObject;
        Vector3 transformPosition = transform.position;
        Vector2 corpsePosition = new Vector2(Mathf.Floor(transformPosition.x) + .5f, Mathf.Floor(transformPosition.y) + .5f);
        GameObject corpse = Instantiate(corpsePrefab, corpsePosition, Quaternion.identity);
        PlayerController.Instance.GetChildComponentByName<Camera>("Main Camera").transform.SetParent(corpse.transform);
        Destroy(thisGameObject);
    }
}