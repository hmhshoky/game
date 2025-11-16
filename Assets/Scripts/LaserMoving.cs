using UnityEngine;

public class LaserMoving : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float moveRange = 2f;
    
    private float startY;
    private int direction = 1;

    void Start()
    {
        startY = transform.position.y;
    }
    
    void Update()
    {
        //Bewegung hoch/runter
        transform.position += Vector3.up * direction * moveSpeed * Time.deltaTime;
        
        //prüfe ob Grenze erreicht
        float distanceFromStart = transform.position.y - startY;
        
        if (distanceFromStart > moveRange)
        {
            direction = -1;
        }
        else if (distanceFromStart < -moveRange)
        {
            direction = 1;
        }
    }
}