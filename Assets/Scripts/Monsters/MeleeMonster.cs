using UnityEngine;

public class MeleeMonster : Monster
{
    public const double MinPursuitDistance = 1.5;
    
    protected override void Start() {
        base.Start();
        pursueState = new MeleeMonsterPursueState(); // Use the specialized pursue state
    }
    
    // protected override void Pursue()
    // {
    //     if (!isMoving)
    //     {
    //         // Check if the monster is already close enough to the player.
    //         if (Vector2.Distance(transform.position, playerTransform.position) <= MinPursuitDistance)
    //         {
    //             // Monster is adjacent to the player, no need to move.
    //             return;
    //         }
    //
    //         FollowPathTowards(playerTransform.position);
    //     }
    // }

}

public class MeleeMonsterPursueState : PursueState {
    public override IMonsterState Execute(Monster monster) {
        if (!(monster is MeleeMonster meleeMonster)) {
            base.Execute(monster);
            return monster.DetermineState();
        }

        if (!meleeMonster.isMoving) {
            float distanceToPlayer = Vector2.Distance(meleeMonster.transform.position, meleeMonster.playerTransform.position);

            // Check if the monster is already close enough to the player.
            if (distanceToPlayer <= MeleeMonster.MinPursuitDistance) {
                // Monster is adjacent to the player, no need to move.
                meleeMonster.isMoving = false;
                return monster.DetermineState();
            }

            meleeMonster.currentDestination = meleeMonster.playerTransform.position;
            meleeMonster.FollowPathTowards(meleeMonster.currentDestination);
        }

        return monster.DetermineState();
    }
}
