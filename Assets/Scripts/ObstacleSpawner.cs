using UnityEngine;
using System.Collections.Generic;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject laserWallPrefab;
    
    public float minX = 15f;
    public float maxX = 25f;
    public float minY = -3f;
    public float maxY = 4f;
    public float minDistanceBetweenLasers = 2.5f;
    
    private float timer = 0f;
    private float currentSpawnTime;
    private GameManager gameManager;
    
    private bool isPaused = false;
    private float pauseTimeRemaining = 0f;

    public float minSpawnTime = 0.5f;
    public float maxSpawnTime = 2f;
    
    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        UpdateSpawnTimesForStage();
        SetRandomSpawnTime();
    }
    
    void Update()
    {
        //wenn pausiert
        //läuft Zeit runter
        //wenn runtergelaufene Zeit = 0 ist, dann wird isPause false gesetzt und Update methode beginnt neu
        if (isPaused)
        {
            pauseTimeRemaining -= Time.deltaTime;
            if (pauseTimeRemaining <= 0f)
            {
                isPaused = false;
            }
            return;
        }

        timer += Time.deltaTime;
        
        //wenn Timer größer ist als die derzeitige Spawntime:
        //spawnt Obstacle und setzt neue random Spawntime aus der Stage
        if (timer >= currentSpawnTime)
        {
            SpawnLaserWalls();
            timer = 0f;
            SetRandomSpawnTime();
        }
    }
    
    public void UpdateSpawnTimesForStage()
    {
        
    }
    
    public void PauseSpawning(float duration)
    {
        isPaused = true;
        pauseTimeRemaining = duration;
    }
    
    void SetRandomSpawnTime()
    {
        currentSpawnTime = Random.Range(minSpawnTime, maxSpawnTime);
    }
    
    void SpawnLaserWalls()
    {
        //maximale Anzahl für aktuelle Stage
        int maxLaserWallCount = GetMaxLaserCountForCurrentStage();

        //+1 weil die letzte Zahl ist exklusiv
        int laserCount = Random.Range(1, maxLaserWallCount + 1);
        
        //Liste für Positions der Obstacles
        List<Vector3> positions = new List<Vector3>();
        
        for (int i = 0; i < laserCount; i++)
        {
            //sucht geeignete SpawnPoint-Position
            Vector3 spawnPos = FindValidSpawnPosition(positions);
            //added den SpawnPoint zur Liste
            positions.Add(spawnPos);
            
            float randomRotation = Random.Range(0f, 360f);
            Quaternion rotation = Quaternion.Euler(0f, 0f, randomRotation);
            
            Instantiate(laserWallPrefab, spawnPos, rotation);
        }
    }
    
    Vector3 FindValidSpawnPosition(List<Vector3> positions)
    {
        Vector3 newPos;
        int attempts = 0;
        int maxAttempts = 30;
        
        do
        {
            //random Position erstellen
            float randomX = Random.Range(minX, maxX);
            float randomY = Random.Range(minY, maxY);
            newPos = new Vector3(randomX, randomY, 0f);
            attempts++; //maximal 30 Attempts sonst break unten
            
            if (attempts >= maxAttempts)
                break;
        //mache das wenn isTooCloseToOthers=true dann nochmal
        } while (checkIfClose(newPos, positions)); //while erst zum Schluss damit "do" einmal eine Position erstellt
        
        return newPos;
    }
    
    bool checkIfClose(Vector3 newPos, List<Vector3> positions)
    {
        //guckt in jede Position in der Liste positions
        foreach (Vector3 position in positions)
        {
            //Distanz zwischen neuer Position(newPos) und Positionen aus der Liste
            float distance = Vector3.Distance(newPos, position);
            //wenn Distanz kleiner ist als die Mindestdistanz, dann wird wieder eine neue Position erstellt
            if (distance < minDistanceBetweenLasers)
                return true;
        }
        return false;
    }
    
    int GetMaxLaserCountForCurrentStage()
    {
        return gameManager.GetMaxObstacleCount();
    }
}