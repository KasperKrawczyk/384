using System;
using DefaultNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[Serializable]
public class OverheadDisplayManager : MonoBehaviour
{
    public Transform targetTransform; // The transform of the monster
    public Vector3 offset = new (0, 0.7f, 0);
    public TextMeshProUGUI nameText;
    public Image overheadDisplayBackgroundImage;
    public Image healthFillImage;
    public string monsterName;

    private void Awake()
    {
        healthFillImage.fillAmount = 1f;
    }

    private void LateUpdate()
    {
        transform.position = targetTransform.position + offset;
    }

    public void UpdateHealth(int maxHealth, int curHealth)
    {
        if (healthFillImage == null) return;
        curHealth = Math.Abs(curHealth);
        UpdateHealthBar(maxHealth, curHealth);
    }

    private void UpdateHealthBar(int maxHealth, int curHealth)
    {
        float fillAmount = (float)curHealth / maxHealth;
        healthFillImage.fillAmount = fillAmount;
    }
}