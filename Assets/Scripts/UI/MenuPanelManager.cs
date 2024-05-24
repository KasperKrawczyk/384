using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPanelManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pauseButtonText;
    public static event Action OnLeaveGameSceneAction;

    public void PauseGame()
    {
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
            pauseButtonText.text = "Pause";
        }
        else
        {
            Time.timeScale = 0;
            pauseButtonText.text = "Resume";
        }
    }

    public void LoadMenuScene()
    {
        Time.timeScale = 1;
        string userName = PlayerPrefs.GetString("CurrentUser");
        GameManager.Instance.SaveScore(userName, PlayerController.Instance.GetStatsManager().GetSkillLevel(IntStatInfoType.Experience));
        GameManager.Instance.LoadMenuScene(GameManager.SceneOrigin.GameScene);
        
    }
}