    using UnityEngine;

    public class MeleeMonster : Monster
    {
        public const double MinPursuitDistance = 1.4143;
        
        protected override void Pursue()
        {
            if (!isMoving)
            {
                // Check if the monster is already close enough to the player.
                if (Vector2.Distance(transform.position, currentDestination) <= MinPursuitDistance)
                {
                    // Monster is adjacent to the player, no need to move.
                    return;
                }
        
                FollowPathTowards(currentDestination);
            }
        }

    }
