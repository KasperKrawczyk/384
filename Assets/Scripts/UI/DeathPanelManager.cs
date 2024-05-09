using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathPanelManager : MonoBehaviour
{
    public bool IsActive = false;
    private void Start()
    {
        gameObject.SetActive(IsActive);
        PlayerCombatManager.OnDieAction += ToggleDeathPanel;
        MenuPanelManager.OnLeaveGameSceneAction += ToggleDeathPanel;
    }
    
    private void OnDestroy()
    {
        PlayerCombatManager.OnDieAction -= ToggleDeathPanel;
        MenuPanelManager.OnLeaveGameSceneAction -= ToggleDeathPanel;
    }

    private void ToggleDeathPanel()
    {
        IsActive = !IsActive;
        gameObject.SetActive(IsActive);
    }
    
    
    
}
