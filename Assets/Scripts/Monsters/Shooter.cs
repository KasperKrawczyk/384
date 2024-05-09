using UnityEngine;

public class Shooter : Monster
{

    [SerializeField] public float shootingDistanceMax { get; set; } = 5f;
    [SerializeField] public float shootingDistanceMin { get; set; } = 3f;

    protected override void Start() {
        base.Start();
        pursueState = new ShooterPursueState();
    }
    
    // protected override void Pursue()
    // {
    //     if (!isMoving)
    //     {
    //         // Calculate the current distance to the player
    //         float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
    //
    //         // Check if the monster is within the desired shooting range
    //         if (distanceToPlayer > monsterStats.GetStat(FloatStatInfoType.ShootingDistanceMin) && distanceToPlayer < monsterStats.GetStat(FloatStatInfoType.ShootingDistanceMax))
    //         {
    //             // Maintain current position if already within the ideal shooting range
    //             isMoving = false;
    //         }
    //         else if (distanceToPlayer <= shootingDistanceMin)
    //         {
    //             // If too close, find a direction to move away from the player
    //             currentDestination = transform.position - playerTransform.position.normalized;
    //             FollowPathTowards(currentDestination);
    //         }
    //         else
    //         {
    //             // If too far, proceed with normal pursuit towards the player
    //             FollowPathTowards(currentDestination);
    //         }
    //     }
    // }

}


public class ShooterPursueState : PursueState {
    public override IMonsterState Execute(Monster monster) {
        if (!(monster is Shooter shooter)) {
            base.Execute(monster);
            return monster.DetermineState();
        }

        if (!shooter.isMoving) {
            float distanceToPlayer = Vector2.Distance(shooter.transform.position, shooter.playerTransform.position);

            if (distanceToPlayer > shooter.shootingDistanceMin && distanceToPlayer < shooter.shootingDistanceMax) {
                // within ideal shooting range, stop moving
                shooter.isMoving = false;
            } else if (distanceToPlayer <= shooter.shootingDistanceMin) {
                // too close, run away from the player
                shooter.currentDestination = shooter.transform.position - (shooter.playerTransform.position - shooter.transform.position).normalized * 1.2f;
                shooter.FollowPathTowards(shooter.currentDestination);
            } else {
                // too far, close close the distance to the player
                shooter.currentDestination = shooter.playerTransform.position;
                shooter.FollowPathTowards(shooter.currentDestination);
            }
        }
        return monster.DetermineState();

    }
}


