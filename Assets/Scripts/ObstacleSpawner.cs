using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject obstaclePrefab; // Hindernis-Prefab
    public float spawnTime = 1.5f; // Spawnzeit
    public float spawnX = 10f; // Spawn-Position rechts
    
    private float timer = 0f;
    
    void Update()
    {
        // zählt Zeit hoch
        timer += Time.deltaTime;
        
        // Wenn genug Zeit vergangen ist, spawnt Objekt und resettet Timer
        if (timer >= spawnTime)
        {
            SpawnObstacle();
            timer = 0f;
        }
    }
    
    void SpawnObstacle()
    {
        // spawnPos zufällig in der Random Range platzieren
        float randomY = Random.Range(-3.5f, 3.5f);
        Vector3 spawnPos = new Vector3(spawnX, randomY, 0f);
        
        // Hindernis erzeugen
        Instantiate(obstaclePrefab, spawnPos, Quaternion.identity);
    }
}