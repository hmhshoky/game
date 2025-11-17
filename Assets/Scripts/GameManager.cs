using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Rendering;

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
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI lastScoreText;

    //Score Interaktionen
    private Color ogScoreColor;
    private Color ogStageColor;
    private float scoreColorTimer = 0f;
    private bool isScoreYellow = false;

    //Score und Zeit
    public float score = 0f;
    private float gameTime = 0f;
    private int highScore = 0;
    private int lastScore = 0;

    //Stage System
    private int currentStage = 1;
    private int previousStage = 1;
    private float[] stageZone = { 0f, 1000f, 2000f, 3000f, 4000f, 5000f, 6000f, 7000f, 8000f, 10000f };
    private float[] stageBaseSpeeds = { 3f, 3.3f, 3.4f, 3.8f, 4f, 4.5f, 5f, 5.3f, 5.7f, 6f };
    private float[] stageScoreMultipliers = { 1.0f, 1.2f, 1.3f, 1.4f, 1.5f, 1.6f, 1.7f, 1.8f, 1.9f, 2f };

    //Audio
    public AudioClip buttonsfx;    
    public AudioClip skyNoise;
    public AudioClip collisionsfx;
    public AudioClip track1;
    public AudioClip track2;
    private AudioSource sfx; //für sfx
    private AudioSource music; //für Musik
     private AudioSource ambient;
    private bool isTrack2Playing = false;

    //Hintergrund Farbe pro Stage
    private Color[] stageBackgroundColors = new Color[]
    {
        new Color(1f, 1f, 1f),
        new Color(0.6f, 0.85f, 1f),     
        new Color(0.4f, 0.75f, 1f),     
        new Color(1f, 0.85f, 0.5f),     
        new Color(1f, 0.6f, 0.3f),      
        new Color(1f, 0.4f, 0.25f),    
        new Color(0.8f, 0.3f, 0.6f),   
        new Color(0.5f, 0.2f, 0.6f),    
        new Color(0.3f, 0.15f, 0.5f),   
        new Color(0.15f, 0.15f, 0.35f), 
    };

    void Start()
    {
        sfx = gameObject.AddComponent<AudioSource>();

        //Original Farben speichern
        ogScoreColor = scoreText.color;
        ogStageColor = stageText.color;

        music = gameObject.AddComponent<AudioSource>();
        music.loop = true; //loopen
        music.volume = 0.4f; //Lautstärke

        ambient = gameObject.AddComponent<AudioSource>();
        ambient.loop = true;
        ambient.volume = 0.3f;

        PlaySkyNoise();
        LoadScores();
        UpdateHomeScreenUI(); 
        
        SetGameState(GameState.HomeMenu);
    }

    void Update()
    {
        if (currentState == GameState.Playing)
        {
            //Zeit läuft
            gameTime += Time.deltaTime;

            //wieviele Punkte pro Sekunde * StageScoreMultiplier
            float currentScoreRate = 10 * stageScoreMultipliers[currentStage - 1];
            score += currentScoreRate * Time.deltaTime; //hochaddieren

            UpdateStage();

            //wenn Stage gewechselt wurde
            if (currentStage != previousStage)
            {
                StagePause();
                ChangeBackgroundColor(); 
                previousStage = currentStage;
            }

            //wenn Score gelb aufgeleuchet ist
            if (isScoreYellow)
            {
                scoreColorTimer -= Time.deltaTime;
                if (scoreColorTimer <= 0f)
                {
                    scoreText.color = ogScoreColor;
                    isScoreYellow = false;
                }
            }

            //Score Anzeige
            scoreText.text = "Score: " + Mathf.FloorToInt(score) + "m";
            stageText.text = "Stage: " + currentStage;

            //Debug Keys
            if (Input.GetKey(KeyCode.U)) score = 1000;
            if (Input.GetKey(KeyCode.I)) score = 4000;
            if (Input.GetKey(KeyCode.O)) score = 8000;
            if (Input.GetKey(KeyCode.P)) score = 10000;
        }

        //wenn Escape und Spielend
        if (Input.GetKeyDown(KeyCode.Escape) && currentState == GameState.Playing)
        {
            PauseGame();
        }
    }

    void LoadScores()
    {
        //holt Score aus Unitys Cookies sonst 0
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

        //alles standartmäßig aus
        homeScreen.SetActive(false);
        pauseScreen.SetActive(false);
        gameOverScreen.SetActive(false);

        switch (currentState)
        {
            case GameState.HomeMenu:
                homeScreen.SetActive(true);
                UpdateHomeScreenUI();
                Time.timeScale = 0f;
                break;

            case GameState.Playing:
                Time.timeScale = 1f;
                break;

            case GameState.Paused:
                pauseScreen.SetActive(true);
                Time.timeScale = 0f;
                music.Pause();
                ambient.Pause();
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
        //geht durch jede StageZone
        for (int i = 0; i < stageZone.Length - 1; i++)
        {
            //wenn Score kleiner als StageZone und kleiner als nächste StageZone
            if (score >= stageZone[i] && score < stageZone[i + 1])
            {
                currentStage = i + 1;
                return;
            }
        }

        //wenn höchste Stage
        if (score >= stageZone[stageZone.Length - 1])
        {
            currentStage = 10; //Stageanzahl anpassen (derzeit 10)
        }
    }

    void StagePause()
    {
        ObstacleSpawner spawner = FindFirstObjectByType<ObstacleSpawner>();
        spawner.PauseSpawning(2f);
    }
    
    public void StartGame()
    {
        score = 0f;
        gameTime = 0f;
        currentStage = 1;
        previousStage = 1;
        PlaySkyNoise();
        StartTrack1();
        SetGameState(GameState.Playing);
    }

    public void PauseGame()
    {
        ButtonSfx();
        SaveScores();
        SetGameState(GameState.Paused);
    }

    public void ResumeGame()
    {
        SetGameState(GameState.Playing);
        music.UnPause(); //Musik unpausieren
        ambient.UnPause();
    }

    public void GameOver()
    {
        CollisionSfx();
        music.Stop();
        SaveScores();
        SetGameState(GameState.GameOver);
    }

    public void BackToMenu()
    {
        DestroyAllObstacles();
        ResetPlayer();
        ResetBackground();

        music.Stop();

        scoreText.text = "";
        stageText.text = "";
        SetGameState(GameState.HomeMenu);
    }

    public void RestartGame()
    {
        //reset alles wie bei StartGame
        score = 0f;
        gameTime = 0f;
        currentStage = 1;
        previousStage = 1;
        
        DestroyAllObstacles();
        ResetPlayer();
        ResetBackground();
        StartTrack1();
        PlaySkyNoise();
        
        SetGameState(GameState.Playing);
    }

    

    void SaveScores()
    {
        int currentScore = Mathf.FloorToInt(score);
        
        //letzter Score erstellen
        lastScore = currentScore;
        PlayerPrefs.SetInt("LastScore", lastScore);
        
        //wenn derzeitiger CSore größer ist als Highscore
        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("HighScore", highScore);
        }
        
        //speichern
        PlayerPrefs.Save();
    }

    void DestroyAllObstacles()
    {
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        
        //alle obstacles in obstacle-Array löschen
        foreach (GameObject obstacle in obstacles)
        {
            Destroy(obstacle);
        }
        GameObject[] coins = GameObject.FindGameObjectsWithTag("Coin");
        foreach (GameObject coin in coins)
        {
            Destroy(coin);
        }
    }

    void ResetPlayer()
    {
        PlayerController player = FindFirstObjectByType<PlayerController>();
        player.transform.position = new Vector3(-5f, -3f, 0f); //SpawnPosition Player
        
        //Velocity resetten
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
    }

    public float GetSpeedMultiplier()
    {
        return stageBaseSpeeds[currentStage - 1];
    }

    public GameState GetCurrentState()
    {
        return currentState;
    }
    
    public int getCurrentStage()
    {
        return currentStage;
    }
    
    public void AddScore(float amount)
    {
        score += amount;
        StartCoroutine(ScoreBlink());
        
        //wenn Stage größer als 8
        if (currentStage == 8 && !isTrack2Playing)
        {
            float randomChance = Random.Range(1, 11); // 1:10 Chance
            
            if (randomChance == 1)
            {
                startTrack2();
            }
        }
    }
    
    void StartTrack1()
    {
        // Stop + Play = Restart
        music.Stop();
        music.clip = track1;
        music.Play();
        //Track2 zurücksetzen
        isTrack2Playing = false;
    }

    void startTrack2()
    {
        music.Stop();
        music.clip = track2;
        music.Play();
        isTrack2Playing = true;
    }

    void PlaySkyNoise()
    {
        ambient.clip = skyNoise;
        ambient.Play();
    }
    void StopSkyNoise()
    {
        ambient.Stop();
    }

    //Ienumerator ist der Typ for Coroutinen
    IEnumerator ScoreBlink()
    {
        scoreText.color = Color.yellow;
        scoreColorTimer = 0.4f;
        isScoreYellow = true;
        yield return null;
    }
    
    public void ButtonSfx()
    {
        sfx.PlayOneShot(buttonsfx, 0.3f);
    }

    public void CollisionSfx()
    {
        sfx.PlayOneShot(collisionsfx, 0.5f);
    }

    void ChangeBackgroundColor()
    {
        //Farbe für die aktuelle Stage
        Color targetColor = stageBackgroundColors[currentStage - 1];
        
        //alle Backgrounds und Farbe ändern
        EndlessBackground[] backgrounds = FindObjectsByType<EndlessBackground>(FindObjectsSortMode.None);
        
        foreach (EndlessBackground bg in backgrounds)
        {   
            //targetColor=oben die Colors, 2 = duration
            bg.SetColor(targetColor, 2);
        }
    }

    void ResetBackground()
    {
        EndlessBackground[] backgrounds = FindObjectsByType<EndlessBackground>(FindObjectsSortMode.None);
        
        foreach (EndlessBackground bg in backgrounds)
        {
            bg.ResetPosition();
            //setzt Farbe sofort auf Stage 1 zurück
            bg.SetColor(stageBackgroundColors[0], 0f);
        }
    }
}