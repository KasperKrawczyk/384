using System.Collections;

public abstract class ACombatManager
{
    public abstract void Die();
    public abstract void PerformAttack();
    public abstract IEnumerator  AttackRoutine();
    public abstract bool IsTargetInRangedAttackRange();
    public abstract bool IsTargetInMeleeAttackRange();
}