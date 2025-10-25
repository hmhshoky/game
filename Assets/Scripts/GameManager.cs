using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI stageText;

    public float baseScorePerSecond = 10f;
    private float score = 0f;
    private float gameTime = 0f;

    private int currentStage = 1;
    private int previousStage = 1; 

    private float[] stageZone = { 0f, 300f, 800f, 1400f, 2000f };
    private float[] stageBaseSpeeds = { 3f, 3.5f, 3.7f, 4f, 5f };
    //Score wächst mit Stage
    private float[] stageScoreMultipliers = { 1.0f, 1.2f, 1.4f, 1.6f, 1.7f };
    private int[] stageObstacleCounts = { 1, 1, 2, 2, 2 };
    void Update()
    {
        //Zeit läuft hoch
        gameTime += Time.deltaTime;
        //Score basierend auf Stage
        float currentScoreRate = baseScorePerSecond * stageScoreMultipliers[currentStage - 1];
        score += currentScoreRate * Time.deltaTime; //fps-unabhängig machen

        UpdateStage();

        //wenn Stage gewechselt wurde, dann kurze Pause
        if (currentStage != previousStage)
        {
            stagePause();
            //previousStage wird geupdated
            previousStage = currentStage;
        }

        //UI
        scoreText.text = "Score: " + Mathf.FloorToInt(score) + "m";
        stageText.text = "Stage: " + currentStage;

        if (Input.GetKey(KeyCode.Space))
        {
            gameTime += 1;
            score += 1;
        }
    }
    
    void UpdateStage()
    {
        //geht durch alle Stages
        for (int i = 0; i < stageZone.Length - 1; i++)
        {
            //wenn score kleiner grösser als current stage(i) und kleiner als nächste stage (i+1)
            if (score >= stageZone[i] && score < stageZone[i + 1])
            {
                currentStage = i + 1;
                return;
            }
        }
        
        //für letzte Stage
        if (score >= stageZone[stageZone.Length - 1])
        {
            currentStage = 5;
        }
    }

    void stagePause()
    {
        //kurze Spawn-Pause
        ObstacleSpawner spawner = FindObjectOfType<ObstacleSpawner>();
        if (spawner != null)
        {
            spawner.PauseSpawning(2f);
            spawner.UpdateSpawnTimesForStage();
        }
    }
    
    
    public float GetSpeedMultiplier()
    {
        //baseSpeed für derzeitige Stage
        float baseSpeed = stageBaseSpeeds[currentStage - 1];

        return baseSpeed;
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