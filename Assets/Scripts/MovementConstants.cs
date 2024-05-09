using UnityEngine;

namespace DefaultNamespace
{
    public class MovementConstants
    {
        public static Vector2[] DIRECTIONS = 
        {
            Vector2.up, Vector2.up + Vector2.right, Vector2.right, Vector2.down + Vector2.right, 
            Vector2.down, Vector2.down + Vector2.left, Vector2.left, Vector2.up + Vector2.left
        };

        public static string[] IDLE_ANIMATION_STATES =
        {
            "IdleUp", "IdleUp", "IdleRight", "IdleDown", "IdleDown", "IdleDown", "IdleLeft", "IdleUp"
        };

        public static string[] MOVE_ANIMATION_STATES =
        {
            "MoveUp", "MoveUp", "MoveRight", "MoveDown", "MoveDown", "MoveDown", "MoveLeft", "MoveUp"
        };
        

    }
}