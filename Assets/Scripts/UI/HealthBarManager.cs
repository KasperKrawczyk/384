using System;
using DefaultNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[Serializable]
public class HealthBarManager : AHealthDisplayer
{
    public TextMeshProUGUI healthText;

    // private void Awake()
    // {
        // healthFillImage.fillAmount = 1f;
    // }

    public override void UpdateHealth(int maxHealth, int curHealth)
    {
        if (healthFillImage == null) return;
        curHealth = Math.Abs(curHealth);
        UpdateHealthBar(maxHealth, curHealth);
        UpdateHealthText(maxHealth, curHealth);
    }

    private void UpdateHealthBar(int maxHealth, int curHealth)
    {
        float fillAmount = (float) curHealth / maxHealth;
        healthFillImage.fillAmount = fillAmount;
    }

    private void UpdateHealthText(int maxHealth, int curHealth)
    {
        healthText.text = $"{maxHealth} / {curHealth}";
    }
}