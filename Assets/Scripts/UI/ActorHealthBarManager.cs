using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ActorHealthBarManager : MonoBehaviour
{
    public new Camera camera;
    public ActorCombatManager acm;
    public Image healthFillImage;

    

    public void Update()
    {
        // Position the health bar
        if(acm.transform != null)
        {
            Vector3 worldPosition = acm.transform.position + Vector3.up * 2;
            Vector3 screenPosition = camera.WorldToScreenPoint(worldPosition);

            if (screenPosition.z > 0)
            {
                transform.position = screenPosition;
                transform.gameObject.SetActive(true);
            }
            else
            {
                transform.gameObject.SetActive(false);
            }
        }

        // Update health bar fill based on the enemy's health
        healthFillImage.fillAmount =  (float) acm.curHealth / acm.maxHealth;
    }
}