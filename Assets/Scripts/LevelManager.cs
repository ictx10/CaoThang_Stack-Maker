using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    private int currentLevel = 1;
    private int currentScore = 0;
    private const string LEVEL_KEY = "CurrentLevel";
    private const string SCORE_KEY = "CurrentScore";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        LoadData();
    }

    public void LoadData()
    {
        currentLevel = PlayerPrefs.GetInt(LEVEL_KEY, 1);
        currentScore = PlayerPrefs.GetInt(SCORE_KEY, 0);
    }

    public void SaveData()
    {
        PlayerPrefs.SetInt(LEVEL_KEY, currentLevel);
        PlayerPrefs.SetInt(SCORE_KEY, currentScore);
        PlayerPrefs.Save();
    }

    public void LoadLevel(int levelIndex)
    {
        if (levelIndex >= 1 && levelIndex <= SceneManager.sceneCountInBuildSettings)
        {
            currentLevel = levelIndex;
            SaveData();
            SceneManager.LoadScene(levelIndex - 1); 
        }
        else
        {
            Debug.LogError("Invalid level index: " + levelIndex);
        }
    }

    public void ReloadLevel()
    {
        LoadLevel(currentLevel);
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    public void SetCurrentLevel(int level)
    {
        currentLevel = level;
        SaveData();
    }

    public int GetCurrentScore()
    {
        return currentScore;
    }

    public void AddScore(int points)
    {
        currentScore += points;
        SaveData();
    }

    public void ResetScore()
    {
        currentScore = 0;
        SaveData();
    }
}