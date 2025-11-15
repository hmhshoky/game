using UnityEngine;
using System.Collections.Generic;

public class ObstacleSpawner : MonoBehaviour
{
    //Prefabs
    public GameObject laserWallPrefab;
    public GameObject laserMovingPrefab;
    public GameObject coinPrefab;
    
    //Spawn Bereich
    public float spawnX = 20f;
    
    //Lane Positionen
//Lane Positionen
private float[] laneYPositions = { -2.5f, 0.5f, 3f }; // ✅ Angepasst an dein Spielfeld    
    //Timing
    private float timer = 0f;
    private float currentSpawnTime;
    public float minSpawnTime = 1.2f;
    public float maxSpawnTime = 3.5f;
    
    private bool isPaused = false;
    private float pauseTimeRemaining = 0f;
    
    //Coin Timing
    private float coinTimer = 0f;
    public float coinSpawnInterval = 7f; //alle 7 Sekunden ein Coin
    
    private GameManager gameManager;
    
    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        SetRandomSpawnTime();
    }
    
    void Update()
    {
        //Nur spawnen wenn Playing
        if (gameManager == null || gameManager.GetCurrentState() != GameManager.GameState.Playing)
        {
            return;
        }
        
        //Pause-System
        if (isPaused)
        {
            pauseTimeRemaining -= Time.deltaTime;
            if (pauseTimeRemaining <= 0f)
            {
                isPaused = false;
            }
            return;
        }

        //Obstacle Timer
        timer += Time.deltaTime;
        if (timer >= currentSpawnTime)
        {
            SpawnObstacles();
            timer = 0f;
            SetRandomSpawnTime();
        }
        
        //Coin Timer
        coinTimer += Time.deltaTime;
        if (coinTimer >= coinSpawnInterval)
        {
            SpawnCoin();
            coinTimer = 0f;
        }
    }
    
void SpawnObstacles()
{
    int currentStage = gameManager.getCurrentStage();
    int maxObstacles = gameManager.GetMaxObstacleCount();
    
    //Liste aller aktiven Obstacles
    GameObject[] existingObstacles = GameObject.FindGameObjectsWithTag("Obstacle");
    
    //Wähle zufällige Lanes
    List<int> usedLanes = new List<int>();
    int obstacleCount = Random.Range(1, maxObstacles + 1);
    
    //Stelle sicher: mindestens 1 Lane bleibt frei
    if (obstacleCount >= 3)
    {
        obstacleCount = 2; //max 2 Lanes belegen
    }
    
    for (int i = 0; i < obstacleCount; i++)
    {
        //Finde freie Lane
        int laneIndex;
        int attempts = 0;
        do
        {
            laneIndex = Random.Range(0, 3);
            attempts++;
            if (attempts > 10) break;
        } while (usedLanes.Contains(laneIndex));
        
        usedLanes.Add(laneIndex);
        
        //Prüfe ob Lane "zu voll" ist
        if (!IsLaneSafe(laneIndex, existingObstacles))
        {
            continue; //Überspringe diese Lane
        }
        
        //Spawne in dieser Lane
        SpawnObstacleInLane(laneIndex, currentStage);
    }
}

// ✅ NEU: Prüft ob Lane sicher zum Spawnen ist
bool IsLaneSafe(int laneIndex, GameObject[] existingObstacles)
{
    float laneY = laneYPositions[laneIndex];
    float minDistance = 4f; //Mindestabstand in X-Richtung
    
    foreach (GameObject obstacle in existingObstacles)
    {
        //Prüfe nur Obstacles in der gleichen Lane (ähnliche Y-Position)
        if (Mathf.Abs(obstacle.transform.position.y - laneY) < 1.5f)
        {
            //Prüfe X-Abstand
            float distance = spawnX - obstacle.transform.position.x;
            if (distance < minDistance)
            {
                return false; //Zu nah!
            }
        }
    }
    
    return true; //Lane ist sicher
}
    
    void SpawnObstacleInLane(int laneIndex, int stage)
    {
        Vector3 spawnPos = new Vector3(spawnX, laneYPositions[laneIndex], 0f);
        float randomRotation = Random.Range(0f, 360f);
        Quaternion rotation = Quaternion.Euler(0f, 0f, randomRotation);
        
        //Stage 1-2: Nur normale Laser
        if (stage < 3)
        {
            Instantiate(laserWallPrefab, spawnPos, rotation);
        }
        //Stage 3+: Mix aus normalen und beweglichen
        else
        {
            //50% Chance für bewegliche Laser
            if (Random.value > 0.5f && laserMovingPrefab != null)
            {
                Instantiate(laserMovingPrefab, spawnPos, rotation);
            }
            else
            {
                Instantiate(laserWallPrefab, spawnPos, rotation);
            }
        }
    }
    
    void SpawnCoin()
    {
        //Wähle zufällige Lane
        int laneIndex = Random.Range(0, 3);
        Vector3 spawnPos = new Vector3(spawnX, laneYPositions[laneIndex], 0f);
        
        if (coinPrefab != null)
        {
            Instantiate(coinPrefab, spawnPos, Quaternion.identity);
            Debug.Log("Coin gespawnt in Lane " + laneIndex);
        }
    }
    
    public void PauseSpawning(float duration)
    {
        isPaused = true;
        pauseTimeRemaining = duration;
    }
    
    public void UpdateSpawnTimesForStage()
    {
        //Kann später für stage-spezifische Anpassungen genutzt werden
    }
    
    void SetRandomSpawnTime()
    {
        currentSpawnTime = Random.Range(minSpawnTime, maxSpawnTime);
    }
}