using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    
    public enum SceneOrigin
    {
        StartUp,
        GameScene,
    }
    
    public static GameManager Instance;
    
    public SceneOrigin lastSceneOrigin = SceneOrigin.StartUp; // default to start-up
    public GameObject mainMenuCanvas;
    public GameObject userButtonPrefab;
    public GameObject loginPanel;
    public GameObject mainMenuPanel;
    public GameObject scoresPanel;
    public GameObject userSelectionPanel;

    public string currentUser;
    private const string UserDataFileName = "userData.txt";
    private string _userDataPath;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _userDataPath = Path.Combine(Application.persistentDataPath, UserDataFileName);
            DontDestroyOnLoad(gameObject);
            LoadUsersFromFile();
        }
        else
        {
            Destroy(gameObject);
        }

    }
    
    void Start()
    {
        switch (GameManager.Instance.lastSceneOrigin)
        {
            case SceneOrigin.StartUp:
                ShowLogin();
                break;
            case SceneOrigin.GameScene:
                ShowMainMenu();
                break;
            default:
                ShowLogin(); // Default action
                break;
        }
    }

    public void ShowMainMenuCanvas(bool show)
    {
        if (mainMenuCanvas != null)
            mainMenuCanvas.SetActive(show);
    }
    
    public void LoadMenuScene(SceneOrigin origin)
    {
        lastSceneOrigin = origin;
        ShowMainMenuCanvas(true);
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    public void SetUser(string username)
    {
        AddNewUser(username);
        currentUser = username;
        PlayerPrefs.SetString("CurrentUser", username); // Save the current user
        PlayerPrefs.Save();
        Debug.Log("Current user set to: " + currentUser);
        ShowMainMenu();
    }

    public void ShowLogin()
    {
        loginPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
        scoresPanel.SetActive(false);
        userSelectionPanel.SetActive(false);
    }

    public void ShowMainMenu()
    {
        loginPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        scoresPanel.SetActive(false);
        userSelectionPanel.SetActive(false);
    }

    public void ShowScores()
    {
        loginPanel.SetActive(false);
        mainMenuPanel.SetActive(false);
        scoresPanel.SetActive(false);
        userSelectionPanel.SetActive(false);
        UpdateScoresText(); 
        scoresPanel.SetActive(true);

    }

    public void ShowUserSelection()
    {
        loginPanel.SetActive(false);
        mainMenuPanel.SetActive(false);
        scoresPanel.SetActive(false);
        PopulateUserList();
        userSelectionPanel.SetActive(true);
    }

    public void StartGame()
    {     
        ShowMainMenuCanvas(false);

        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }

    public void ExitGame()
    {
        PlayerPrefs.Save();
        SaveUsersToFile();
        Application.Quit();
    }

    public void UpdateScoresText()
    {
        TextMeshProUGUI scoreText = scoresPanel.GetComponentsInChildren<TextMeshProUGUI>()[1];
        scoreText.text = "";
        List<string> userNames = GetAllUsers();
        foreach (var userName in userNames)
        {
            scoreText.text += userName + " : " + PlayerPrefs.GetInt(userName + "_Score", 0) + "\n";

        }

    }

    public void PopulateUserList()
    {
        GameObject scrollViewContent = userSelectionPanel.GetComponentInChildren<ScrollRect>().content.gameObject;

        foreach (Transform child in scrollViewContent.transform)
        {
            Destroy(child.gameObject);
        }
        
        foreach (string user in GetAllUsers())
        {
            GameObject userButton = Instantiate(userButtonPrefab, scrollViewContent.transform);
            TextMeshProUGUI textComponent = userButton.GetComponentInChildren<TextMeshProUGUI>();
            textComponent.text = user;
            userButton.GetComponent<Button>().onClick.AddListener(() => SetUser(user));
        }
    }

    private List<string> GetAllUsers()
    {
        List<string> users = new List<string>();
        int userCount = PlayerPrefs.GetInt("UserCount", 0);
        for (int i = 0; i < userCount; i++)
        {
            string user = PlayerPrefs.GetString("User_" + i);
            users.Add(user);
        }

        return users;
    }

    public void AddNewUser(string username)
    {
        int userCount = PlayerPrefs.GetInt("UserCount", 0);
        for (int i = 0; i < userCount; i++)
        {
            string user = PlayerPrefs.GetString("User_" + i);
            if (username.Equals(user)) return;
            Debug.Log("User already exists: " + username);

        }

        PlayerPrefs.SetString("User_" + userCount, username);
        PlayerPrefs.SetInt("UserCount", userCount + 1);
        PlayerPrefs.SetInt(username + "_Score", 0);
        Debug.Log("User added: " + username);
        PlayerPrefs.Save();
        SaveUsersToFile();

    }

    public void SaveScore(string username, int score)
    {
        PlayerPrefs.SetInt(username + "_Score", score);
        PlayerPrefs.Save();
    }

    public void ToggleUserSelectionPanel(bool fromLogin)
    {
        ShowUserSelection();
    }

    private void PopulateDummyPlayers()
    {
        int numDummies = 5;
        PlayerPrefs.SetInt("UserCount", numDummies);
        string[] names = { "alpha", "beta", "gamma", "delta", "zeta" };
        for (int i = 0; i < numDummies; i++)
        {
            string username = names[i];
            PlayerPrefs.SetString("User_" + i, username);
            PlayerPrefs.SetInt(username + "_Score", 0); // Initialize score


        }
    }

    private void LoadUsersFromFile()
    {
        if (File.Exists(_userDataPath))
        {
            string[] lines = File.ReadAllLines(_userDataPath);
            foreach (string line in lines)
            {
                string[] parts = line.Split(',');
                if (parts.Length >= 2)
                {
                    // Assuming the format is "username,score"
                    PlayerPrefs.SetString(parts[0], parts[0]); // Storing username in PlayerPrefs temporarily
                    PlayerPrefs.SetInt(parts[0] + "_Score", int.Parse(parts[1]));
                }
            }
        }
        else
        {
            Debug.Log("User data file does not exist. Creating a new one.");
            File.WriteAllText(_userDataPath, "");
            PopulateDummyPlayers(); // Populate with dummy data if no file exists
            SaveUsersToFile(); // Save the dummy data immediately
        }
    }
    
    private void SaveUsersToFile()
    {
        List<string> lines = new List<string>();
        int userCount = PlayerPrefs.GetInt("UserCount", 0);
        for (int i = 0; i < userCount; i++)
        {
            string username = PlayerPrefs.GetString("User_" + i);
            int score = PlayerPrefs.GetInt(username + "_Score", 0);
            lines.Add(username + "," + score);
        }
        File.WriteAllLines(_userDataPath, lines);
    }
    
}