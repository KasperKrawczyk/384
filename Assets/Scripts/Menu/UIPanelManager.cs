using UnityEngine;

public class UIPanelManager : MonoBehaviour
{
    
    public GameObject ThisPanel;
    public void ClosePanel()
    {
        ThisPanel.SetActive(false);
        GameManager.Instance.ShowMainMenu();
    }

    public void OpenPanel()
    {
        ThisPanel.SetActive(true);
    }
}