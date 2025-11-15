using UnityEngine;

public class CoinCollector : MonoBehaviour
{
    public int coinValue = 100;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        //Prüft ob Player
        if (other.gameObject.CompareTag("Player"))
        {
            //Finde GameManager und gib Punkte
            GameManager gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager != null)
            {
                gameManager.AddScore(coinValue);
            }
            
            //Lösche Coin
            Destroy(gameObject);
        }
    }
    
void Update()
{
    //Bewegt sich nach links (GLEICHE Speed wie Obstacles)
    float baseSpeed = 3f; //Gleich wie ObstacleMove
    GameManager gameManager = FindFirstObjectByType<GameManager>();
    if (gameManager != null)
    {
        baseSpeed *= gameManager.GetSpeedMultiplier();
    }
    
    transform.position += Vector3.left * baseSpeed * Time.deltaTime;
    
    //Lösche wenn zu weit links
    if (transform.position.x < -15f)
    {
        Destroy(gameObject);
    }
}
}