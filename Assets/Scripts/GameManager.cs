using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    //Spielzustände
    public enum GameState
    {
        HomeMenu,
        Playing,
        Paused,
        GameOver
    }

    private GameState currentState;

    //UI Panels
    public GameObject homeScreen;
    public GameObject pauseScreen;
    public GameObject gameOverScreen;

    //UI Texte
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI stageText;
    public TextMeshProUGUI finalScoreText; //für GameOver Screen
    public TextMeshProUGUI highScoreText;  //Highscore Home Screen
    public TextMeshProUGUI lastScoreText;  //letzter Score Home Screen

    //Score und Zeit
    public float score = 0f;
    private float gameTime = 0f;
    
    //Scores
    private int highScore = 0;
    private int lastScore = 0;

    //Stage System
    private int currentStage = 1;
    private int previousStage = 1;

    private float[] stageZone = { 0f, 300f, 800f, 1400f, 2000f };
    private float[] stageBaseSpeeds = { 3f, 3.2f, 3.3f, 5f, 6f };
    private float[] stageScoreMultipliers = { 1.0f, 1.2f, 1.3f, 1.4f, 1.5f };
    private int[] stageObstacleCounts = { 1, 1, 2, 3, 3 };

    void Start()
    {
        //lade gespeicherte Scores
        LoadScores();
        UpdateHomeScreenUI();
        
        //startet in den HomeScreen
        SetGameState(GameState.HomeMenu);
    }

    void Update()
    {
        if (currentState == GameState.Playing)
        {
            //Zeit läuft hoch
            gameTime += Time.deltaTime;

            //Score basierend auf Stage
            float currentScoreRate = 10 * stageScoreMultipliers[currentStage - 1];
            score += currentScoreRate * Time.deltaTime;

            UpdateStage();

            //Wenn Stage gewechselt wurde
            if (currentStage != previousStage)
            {
                stagePause();
                previousStage = currentStage;
            }

            //UI Update
            scoreText.text = "Score: " + Mathf.FloorToInt(score) + "m";
            stageText.text = "Stage: " + currentStage;

            if (Input.GetKey(KeyCode.Z)) score = 100;
            if (Input.GetKey(KeyCode.U)) score = 300;
            if (Input.GetKey(KeyCode.I)) score = 800;
            if (Input.GetKey(KeyCode.O)) score = 1400;
            if (Input.GetKey(KeyCode.P)) score = 2000;
        }

        //Pause mit Escape-Taste und nur wenn playing
        if (Input.GetKeyDown(KeyCode.Escape) && currentState == GameState.Playing)
        {
            PauseGame();
        }
    }

    void LoadScores()
    {
        //PlayerPrefs.GetInt holt saved Werte
        //0, wenn nix da ist
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        lastScore = PlayerPrefs.GetInt("LastScore", 0);
    }
    
    void UpdateHomeScreenUI()
    {
        highScoreText.text = "Highscore: " + highScore + "m";
        lastScoreText.text = "Last Score: " + lastScore + "m";
    }

    public void SetGameState(GameState newState)
    {
        currentState = newState;

        //standartmäßig alle Panels aus
        homeScreen.SetActive(false);
        pauseScreen.SetActive(false);
        gameOverScreen.SetActive(false);

        //nach State das richtige Panel
        switch (currentState)
        {
            case GameState.HomeMenu:
                homeScreen.SetActive(true);
                UpdateHomeScreenUI();
                Time.timeScale = 0f; //pausiert
                break;

            case GameState.Playing:
                Time.timeScale = 1f; //Spiel läuft
                break;

            case GameState.Paused:
                pauseScreen.SetActive(true);
                Time.timeScale = 0f;
                break;

            case GameState.GameOver:
                gameOverScreen.SetActive(true);
                finalScoreText.text = "Final Score: " + Mathf.FloorToInt(score) + "m";
                Time.timeScale = 0f;
                break;
        }
    }

    void UpdateStage()
    {
        //geht durch jede Stage durch
        for (int i = 0; i < stageZone.Length - 1; i++)
        {
            //wenn aktuelle Score größer als aktuelle und kleiner als nächste Stage ist
            if (score >= stageZone[i] && score < stageZone[i + 1])
            {
                currentStage = i + 1; //Stage wird erhöht
                return;
            }
        }

        //wenn Score größer ist als die letzte Stage
        if (score >= stageZone[stageZone.Length - 1])
        {
            currentStage = 5; //letzte Stage
        }
    }

    void stagePause()
    {
        ObstacleSpawner spawner = FindFirstObjectByType<ObstacleSpawner>();
        spawner.PauseSpawning(2f);
        spawner.UpdateSpawnTimesForStage();
    }
    
    //Methoden für die Buttons und GameOver
    public void StartGame()
    {
        score = 0f;
        gameTime = 0f;
        currentStage = 1;
        previousStage = 1;
    
        DestroyAllObstacles();
        ResetPlayer();
        SetGameState(GameState.Playing);
    }

    public void PauseGame()
    {
        SetGameState(GameState.Paused);
    }

    public void ResumeGame()
    {
        SetGameState(GameState.Playing);
    }

    public void GameOver()
    {
        SaveScores();
        SetGameState(GameState.GameOver);
    }

    public void BackToMenu()
    {
        SetGameState(GameState.HomeMenu);
    }

    void SaveScores()
    {
        int currentScore = Mathf.FloorToInt(score);
        
        //letzter Score ist immer der aktuelle
        lastScore = currentScore;
        PlayerPrefs.SetInt("LastScore", lastScore);
        
        //Highscore
        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("HighScore", highScore);
        }
        
        //speichert die Daten auf die Festplatte
        PlayerPrefs.Save();
    }

    void DestroyAllObstacles()
    {
        //alle GameObjects mit dem Tag "Obstacle"
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        
        //löscht jedes einzelne
        foreach (GameObject obstacle in obstacles)
        {
            Destroy(obstacle);
        }
    }

    void ResetPlayer()
    {
        PlayerController player = FindFirstObjectByType<PlayerController>();

        //setzt Position zurück
        player.transform.position = new Vector3(-5f, -3f, 0f);
        
        //setzt Velocity auf 0
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
    }

    public float GetSpeedMultiplier()
    {
        return stageBaseSpeeds[currentStage - 1];
    }

    public int GetMaxObstacleCount()
    {
        return stageObstacleCounts[currentStage - 1];
    }

    public GameState GetCurrentState()
    {
        return currentState;
    }
    public int getCurrentStage()
    {
        return currentStage;
    }
}