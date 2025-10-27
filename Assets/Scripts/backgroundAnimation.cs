using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [Header("Scrolling")]
    public float scrollSpeed = 2f; // Wie schnell scrollt dieser Layer
    
    [Header("Looping")]
    public float backgroundWidth = 20f; // Breite des Background-Sprites
    
    private Vector3 startPosition;
    
    void Start()
    {
        startPosition = transform.position;
    }
    
    void Update()
    {
        // Bewege Background nach links
        transform.position += Vector3.left * scrollSpeed * Time.deltaTime;
        
        // Wenn zu weit links → Reset Position (Seamless Loop!)
        if (transform.position.x < startPosition.x - backgroundWidth)
        {
            transform.position = startPosition;
        }
    }
}