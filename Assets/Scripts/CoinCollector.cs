using UnityEngine;

public class CoinCollector : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        //prüft ob Player
        if (other.gameObject.CompareTag("Player"))
        {
            //gibt GameManager.score 100 Extra Punkte
            GameManager gameManager = FindFirstObjectByType<GameManager>();
            gameManager.score += 100 ;
            
            //lösche Coin
            Destroy(gameObject);
        }
    }
    
    void Update()
    {
        //Bewegung nach links
        float baseSpeed = 3f; //gleich wie ObstacleMove
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        baseSpeed *=  gameManager.GetSpeedMultiplier();
        
        transform.position += Vector3.left * baseSpeed * Time.deltaTime;
        
        //löschen wenn zu weit links
        if (transform.position.x < -15f)
        {
            Destroy(gameObject);
        }
    }
}