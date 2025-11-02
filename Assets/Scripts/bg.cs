using UnityEngine;

public class EndlessBackground : MonoBehaviour
{
    public float speed = 2f;
    public Transform otherBackground; // Der andere Background!
    private float width;
    
    void Start()
    {
        // Berechne die ECHTE Breite (mit Scale!)
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        width = sr.bounds.size.x;
    }
    
    void Update()
    {
        // Nach links
        transform.Translate(Vector3.left * speed * Time.deltaTime);
        
        // Wenn komplett links raus
        if (transform.position.x < -width)
        {
            // Setze es RECHTS vom anderen!
            transform.position = new Vector3(
                otherBackground.position.x + width,
                transform.position.y,
                transform.position.z  // Z bleibt gleich!
            );
        }
    }
}