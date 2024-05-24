using UnityEngine;

public class Shooter : Monster
{

    [SerializeField] public float shootingDistanceMax { get; set; } = 5f;
    [SerializeField] public float shootingDistanceMin { get; set; } = 3f;
    
    
    protected override void Pursue()
    {
        if (!isMoving)
        {
            // Calculate the current distance to the player
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
    
            // Check if the monster is within the desired shooting range
            if (distanceToPlayer > monsterStats.GetStat(IntStatInfoType.ShootingDistanceMin) && distanceToPlayer < monsterStats.GetStat(IntStatInfoType.ShootingDistanceMax))
            {
                // Maintain current position if already within the ideal shooting range
                isMoving = false;
            }
            else if (distanceToPlayer <= shootingDistanceMin)
            {
                // If too close, find a direction to move away from the player
                currentDestination = transform.position - playerTransform.position.normalized;
                FollowPathTowards(currentDestination);
            }
            else
            {
                // If too far, proceed with normal pursuit towards the player
                FollowPathTowards(currentDestination);
            }
        }
    }

}



