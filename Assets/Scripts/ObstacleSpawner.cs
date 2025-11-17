using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class ObstacleSpawner : MonoBehaviour
{
    //Prefabs
    public GameObject laserWallPrefab;
    public GameObject coinPrefab;
    
    //Spawn Bereich
    public float spawnX = 10f;
    
    //Lane Positionen
    private float[] laneYPositions = { -2.5f, 0.5f, 3.5f };
    
    //Timing
    private float obstacleTimer = 0f;
    private float obstacleSpawnTime;
    public float minSpawnTime = 1f;
    public float maxSpawnTime = 3f;

    private float coinSpawnTime;
    public float minCoinSpawnTime = 4f;
    public float maxCoinSpawnTime = 7f;
    
    private bool isPaused = false;
    private float pauseTimeRemaining = 0f;
    
    //Coin Timing
    private float coinTimer = 0f;
    
    private GameManager gameManager;
    
    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        SetObstacleTime();
        SetCoinTime();
    }
    
    void Update()
    {
        //Nur spawnen wenn Playing
        if (gameManager.GetCurrentState() != GameManager.GameState.Playing)
        {
            return;
        }
        
        //wenn pausiert
        if (isPaused)
        {
            //Pausenzeit bzw. von unten Duration runter
            pauseTimeRemaining -= Time.deltaTime;
            if (pauseTimeRemaining <= 0f)
            {
                isPaused = false;
            }
            return;
        }

        //Obstacle Timer
        obstacleTimer += Time.deltaTime;
        if (obstacleTimer >= obstacleSpawnTime)
        {
            SpawnObstacles();
            obstacleTimer = 0f;
            SetObstacleTime();
        }
        
        //Coin Timer
        coinTimer += Time.deltaTime;
        if (coinTimer >= coinSpawnTime)
        {
            SpawnCoin();
            coinTimer = 0f;
            SetCoinTime();
        }
    }
        
    void SpawnObstacles()
    {
        int currentStage = gameManager.getCurrentStage();
        
        //Stage 1-4 = maximal 1 Obstacle, Stage 5+ = max 2
        int maxObstacles = (currentStage >= 5) ? 2 : 1;
        int obstacleCount = Random.Range(0, maxObstacles + 1); //Zufall
        
        //Liste aller aktiven Obstacles
        GameObject[] existingObstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        
        //wähle zufällige Lanes
        List<int> usedLanes = new List<int>();
        
        for (int i = 0; i < obstacleCount; i++)
        {
            //Finde freie Lane
            int lane;
            int attempts = 0;
            do
            {
                lane = Random.Range(0, 3); //0, 1 oder 2
                attempts++;
                if (attempts > 10) break;
            } while (usedLanes.Contains(lane)); //wenn Lane benutzt wurde dann nochmal
            
            usedLanes.Add(lane);
            
            //wenn Lane nicht sicher ist
            //Abstand 3 mind.
            if (!IsLaneSafe(lane, existingObstacles, 3f))
            {
                continue; //gehe zu nächsten Schleifendurchlauf
            }
            
            //spawne LaserWall in dieser Lane
            SpawnObstacleInLane(lane);
        }
    }

    bool IsLaneSafe(int laneIndex, GameObject[] existingObstacles, float minDistance)
    {
        //y Position enthnehmen
        float laneY = laneYPositions[laneIndex];
        
        foreach (GameObject obstacle in existingObstacles)
        {
            //y vom obstacle: obstacle.transform.position.y
            // Mathf.Abs macht das Ergebnis positiv
            //guckt ob das Obstacle auf der jeweiligen (1-3) Lane ist 
            //1.5 = Toleranzwert basierend auf Lane Positionen
            if (Mathf.Abs(obstacle.transform.position.y - laneY) < 1.5f)
            {
                //gucken ob Abstand von Obstacle und dem Spawn Punkt zu nah
                //ist für die jeweilige Lane
                float distance = spawnX - obstacle.transform.position.x;
                if (distance < minDistance)
                {
                    return false;
                }
            }
        }
        return true;
    }
    
    void SpawnObstacleInLane(int laneIndex)
    {   
        //x, y, z
        Vector3 spawnPos = new Vector3(spawnX, laneYPositions[laneIndex], 0f);

        //Rotation RNG und Übergabe
        float randomRotation = Random.Range(0f, 360f);
        Quaternion rotation = Quaternion.Euler(0f, 0f, randomRotation);
        
        Instantiate(laserWallPrefab, spawnPos, rotation);
    }
    
    void SpawnCoin()
    {
        GameObject[] existingObstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        
        List<int> safeLanes = new List<int>();
        
        for (int i = 0; i < 3; i++)
        {   
            //Abstand 7 mind.
            if (IsLaneSafe(i, existingObstacles, 15f))
            {
                safeLanes.Add(i);
            }
        }
        
        //wenn keine SafeLanes dann Abbruch Funktion
        if (safeLanes.Count == 0)
        {
            return;
        }
        
        //random Safe Lane aussuchen
        // 0 - 2
        int laneIndex = safeLanes[Random.Range(0, safeLanes.Count)];
        Vector3 spawnPos = new Vector3(spawnX, laneYPositions[laneIndex], 0f);
        Instantiate(coinPrefab, spawnPos, Quaternion.identity);
    }
    
    //Spawnen Pausieren und es true setzen und Zeitlänge setzen
    public void PauseSpawning(float duration)
    {
        isPaused = true;
        pauseTimeRemaining = duration;
    }
    
    void SetObstacleTime()
    {
        obstacleSpawnTime = Random.Range(minSpawnTime, maxSpawnTime);
    }

    void SetCoinTime()
    {
        coinSpawnTime = Random.Range(minCoinSpawnTime, maxCoinSpawnTime);
    }
}