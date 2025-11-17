using UnityEngine;

public class CoinCollector : MonoBehaviour
{
    public int coinValue = 100;
    public float baseSpeed = 3f;

    public AudioClip coinSound;
    
    //wenn eingesammelt wird
    void OnTriggerEnter2D(Collider2D other)
    {
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        
        AudioSource.PlayClipAtPoint(coinSound, transform.position, 0.3f);

        //added im GameManger 100 auf den Score
        gameManager.AddScore(coinValue);

        //löscht Coin
        Destroy(gameObject);
    }
    
    void Update()
    {
        float currentSpeed = baseSpeed;
        
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        //bewegt sich nach den Stage Speeds
        currentSpeed *= gameManager.GetSpeedMultiplier();

        //Bewegung nach links
        transform.position += Vector3.left * currentSpeed * Time.deltaTime;
        
        //löscht Coin wenn zu weit links
        if (transform.position.x < -15f)
        {
            Destroy(gameObject);
        }
    }
}