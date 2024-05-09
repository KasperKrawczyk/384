using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public GameObject tutorialPanel;
    public PlayerController playerController;
    private TextMeshProUGUI tutorialText;
    private bool hasMovedUp, hasMovedDown, hasMovedLeft, hasMovedRight;
    private bool movementComplete = false;
    private bool lootingComplete = false;
    private bool attackDone = false;

    private void Start()
    {
        tutorialText = tutorialPanel.GetComponentInChildren<TextMeshProUGUI>();
        tutorialText.text = "Use WSAD to move.";

        PlayerController.OnMoveAction += InvokeMovementAction;
        CorpseManager.OnOpenedAction += EnableAttackTutorial;
        Monster.OnAttackedAction += EndTutorial;

    }


    void CheckMovementTutorial()
    {
        if (hasMovedUp && hasMovedDown && hasMovedLeft && hasMovedRight)
        {
            movementComplete = true;
            EnableLootingTutorial();
        }
        
    }

    void EndTutorial()
    {
        if (attackDone)
        {
            HideTutorial();
        }
    }
    
    void EnableLootingTutorial()
    {
        if (movementComplete)
        {
            CorpseManager.CanOpen = true;
            tutorialText.text = "Left click on a corpse to loot it.\nDrag items to your inventory slots.";
        }
    }
    
    void EnableAttackTutorial()
    {
        tutorialText.text = "Press CTRL + Left Click on an enemy to attack.";
        playerController.SetCanAttack(true);
        attackDone = true;
    }
    
    private void InvokeMovementAction(Vector2 direction)
    {
        direction.Normalize();

        if (direction.y > 0.5)
        {
            hasMovedUp = true;
        }
        else if (direction.y < -0.5)
        {
            hasMovedDown = true;
        }

        if (direction.x > 0.5)
        {
            hasMovedLeft = true;
        }
        else if (direction.x < -0.5)
        {
            hasMovedRight = true;
        } 
        
        CheckMovementTutorial();
    }
    public void HideTutorial()
    {
        tutorialPanel.SetActive(false);
    }
}