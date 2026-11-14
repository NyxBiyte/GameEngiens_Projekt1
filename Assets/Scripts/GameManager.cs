using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Transform playerSpawnPoint;
    public GameObject playerPrefab;
    public PlayerController player;

    public float playerLives = 5.0f;
    public float maxLives = 5.0f;
    public int playerScore = 0;
    public int playerLevel = 1;

    public float maxLevelTime = 180f;
    public bool levelRunning = false;

    // Score tracking
    public int coinCount = 0;
    public float levelTime = 0f;
    public int lastScore = 0;
    public bool lastWasNewHighscore = false;



    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (levelRunning)
        {
            levelTime += Time.deltaTime;
            if (levelTime >= maxLevelTime)
            {
                GameOver();
            }
        }
    }

    public void RespawnPlayer()
    {
        var player = FindFirstObjectByType<PlayerController>();
        if (player == null) return;
        if (player.TryGetComponent<Rigidbody2D>(out var rb))
                {
                    rb.linearVelocity = Vector2.zero;

                }
        player.transform.position = playerSpawnPoint.position;

        player.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
    }

    public void NewGame()
    {
        playerLives = 5f;
        playerLevel = 1;
        coinCount = 0;
        SceneManager.LoadScene("Level1", LoadSceneMode.Single);
    }

    public void LoadLevel(int level)
    {
        playerLevel = level;
        SceneManager.LoadScene($"Level{level}", LoadSceneMode.Single);
        levelRunning = true;
        levelTime = 0f;
    }

    public void GameOver()
    {
        SceneManager.LoadScene("GameOver", LoadSceneMode.Single);
    }

    public void GameWon()
    {
        lastScore = CalculateScore();
        
        string key = $"Highscore_Level{playerLevel}";
        if (lastScore > PlayerPrefs.GetInt(key, 0))
        {
            lastWasNewHighscore = true;
            PlayerPrefs.SetInt(key, lastScore);
            PlayerPrefs.Save();
        }
        SceneManager.LoadScene("GameWon", LoadSceneMode.Single);
    }

    public void LevelComplete()
    {
        levelRunning = false;
        playerLevel++;
        if (playerLevel > 2)
        {
            GameWon();
            return;
        }
        LoadLevel(playerLevel);
    }

    public int CalculateScore()
    {
        int coinScore = coinCount * 100;
        int lifeScore = Mathf.RoundToInt(playerLives * 500f);
        int timeScore = Mathf.Max(0, Mathf.RoundToInt((maxLevelTime - levelTime) * 10f));
        return coinScore + lifeScore + timeScore;
    }


}