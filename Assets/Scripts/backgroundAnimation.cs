using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public float scrollSpeed = 2f;
    
    public float backgroundWidth = 20f;
    
    private Vector3 startPosition;
    
    void Start()
    {
        startPosition = transform.position;
    }
    
    void Update()
    {
        //Bewegung nach links
        transform.position += Vector3.left * scrollSpeed * Time.deltaTime;
        
        //Wenn zu weit links → Reset Position
        if (transform.position.x < startPosition.x - backgroundWidth)
        {
            transform.position = startPosition;
        }
    }
}