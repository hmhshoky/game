using UnityEngine;

public class EndlessBackground : MonoBehaviour
{
    public float speed = 2f;
    public Transform otherBackground; //der zweite Background (Transform = Position)
    private float width; //Background Breite
    
    private Vector3 startPosition;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        width = spriteRenderer.bounds.size.x; //wie breit das Bild ist
        
        startPosition = transform.position; //setzt die Startposition
    }

    void Update()
    {
        //Bewegung nach links mit 2f als Speed
        transform.Translate(Vector3.left * speed * Time.deltaTime);
        
        //wenn x Position kleiner als width
        if (transform.position.x < -width)
        {
            transform.position = new Vector3(
                otherBackground.position.x + width, //x + Breite = rechts daneben
                transform.position.y,
                transform.position.z
            );
        }
    }
    
    public void ResetPosition()
    {
        //setzt transform.position zurück zum start
        transform.position = startPosition;
    }
    

    public void SetColor(Color targetColor, float duration)
    {
        //StartCoroutine läuft über mehrere Frames -> wichtig für Fade
        if (duration <= 0f)
        {
            //setze die Farbe direkt, ohne Coroutine.
            {
                spriteRenderer.color = targetColor;
            }
        }
        else
        {
            //andernfalls starte den Fade-Effekt
            StartCoroutine(FadeToColor(targetColor, duration));
        }
    }
    
    // System.Collections.IEnumerator = Typ für Coroutinen
    private System.Collections.IEnumerator FadeToColor(Color targetColor, float duration)
    {
        Color startColor = spriteRenderer.color;
        float timePassed = 0f;
        
        while (timePassed < duration)
        {
            timePassed += Time.deltaTime; //erhöht Zeit
            float t = timePassed / duration; // Fortschritt (0.5 / 2 = 0.25 -> 25%)
            
            //Übergabe StartFarbe, Endfarbe, Fortschritt
            spriteRenderer.color = Color.Lerp(startColor, targetColor, t);
            
            //wichtig damit es bis zumnächsten Frame wartet anstatt alles aufeinmal zu machen
            yield return null;
        }
    }
}