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

    private GameState currentState = GameState.HomeMenu;

    //UI Panels
    public GameObject homeScreen;
    public GameObject pauseScreen;
    public GameObject gameOverScreen;

    //UI Texte
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI stageText;
    public TextMeshProUGUI gameOverScoreText; //für GameOver Screen

    //Score und Zeit
    private float score = 0f;
    private float gameTime = 0f;

    //Stage System
    private int currentStage = 1;
    private int previousStage = 1;

    private float[] stageZone = { 0f, 300f, 800f, 1400f, 2000f };
    private float[] stageBaseSpeeds = { 3f, 3.1f, 3.3f, 3.4f, 3.5f };
    private float[] stageScoreMultipliers = { 1.0f, 1.2f, 1.4f, 1.6f, 1.7f };
    private int[] stageObstacleCounts = { 1, 1, 2, 3, 3 };

    void Start()
    {
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

    public void SetGameState(GameState newState)
    {
        currentState = newState;

        // Alle Panels ausschalten
        homeScreen.SetActive(false);
        pauseScreen.SetActive(false);
        gameOverScreen.SetActive(false);

        // Je nach State das richtige Panel einschalten
        switch (currentState)
        {
            case GameState.HomeMenu:
                homeScreen.SetActive(true);
                Time.timeScale = 0f;
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
                gameOverScoreText.text = "Final Score: " + Mathf.FloorToInt(score) + "m";
                Time.timeScale = 0f;
                break;
        }
    }
    
    public void StartGame()
    {
        //resetted alles
        score = 0f;
        gameTime = 0f;
        currentStage = 1;
        previousStage = 1;

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
        SetGameState(GameState.GameOver);
    }

    public void BackToMenu()
    {
        SetGameState(GameState.HomeMenu);
    }

    void UpdateStage()
    {
        for (int i = 0; i < stageZone.Length - 1; i++)
        {
            if (score >= stageZone[i] && score < stageZone[i + 1])
            {
                currentStage = i + 1;
                return;
            }
        }

        if (score >= stageZone[stageZone.Length - 1])
        {
            currentStage = 5;
        }
    }

    void stagePause()
    {
        ObstacleSpawner spawner = FindFirstObjectByType<ObstacleSpawner>();
        if (spawner != null)
        {
            spawner.PauseSpawning(2f);
            spawner.UpdateSpawnTimesForStage();
        }
    }

    public float GetSpeedMultiplier()
    {
        return stageBaseSpeeds[currentStage - 1];
    }

    public int GetMaxObstacleCount()
    {
        return stageObstacleCounts[currentStage - 1];
    }

    public int GetCurrentStage()
    {
        return currentStage;
    }
}