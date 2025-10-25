using UnityEngine;

public class ObstacleMove : MonoBehaviour
{
    public float baseSpeed = 5f;
    private float currentSpeed;
    
    void Start()
    {
        //Speed beim Spawnen
        GameManager gameManager = FindObjectOfType<GameManager>();
        currentSpeed = baseSpeed * gameManager.GetSpeedMultiplier();
    }
    
    void Update()
    {
        transform.position += Vector3.left * currentSpeed * Time.deltaTime;
        
        //lösche wenn zu weit links
        if (transform.position.x < -15f)
        {
            Destroy(gameObject);
        }
    }
}