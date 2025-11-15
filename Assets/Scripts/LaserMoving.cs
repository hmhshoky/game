using UnityEngine;

public class LaserMoving : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float moveRange = 3f;
    
    private float startY;
    private int direction = 1;
    
    // ✅ NEU: Line Renderer für Path
    private LineRenderer lineRenderer;
    
    void Start()
    {
        startY = transform.position.y;
        
        // ✅ Erstelle visuelle Linie
        CreatePathLine();
    }
    
    void CreatePathLine()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.positionCount = 2;
        
        //Farbe (semi-transparent rot/orange)
        lineRenderer.startColor = new Color(1f, 0.5f, 0f, 0.3f);
        lineRenderer.endColor = new Color(1f, 0.5f, 0f, 0.3f);
        
        //Material
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        
        UpdatePathLine();
    }
    
    void Update()
    {
        //Bewegung hoch/runter
        transform.position += Vector3.up * direction * moveSpeed * Time.deltaTime;
        
        //Prüfe ob Grenze erreicht
        float distanceFromStart = transform.position.y - startY;
        
        if (distanceFromStart > moveRange)
        {
            direction = -1;
        }
        else if (distanceFromStart < -moveRange)
        {
            direction = 1;
        }
        
        // ✅ Update Linie
        UpdatePathLine();
    }
    
    void UpdatePathLine()
    {
        if (lineRenderer != null)
        {
            //Zeige Linie von aktueller Position bis Ende des Bewegungsbereichs
            Vector3 currentPos = transform.position;
            Vector3 endPos = new Vector3(currentPos.x, startY + (moveRange * direction), currentPos.z);
            
            lineRenderer.SetPosition(0, currentPos);
            lineRenderer.SetPosition(1, endPos);
        }
    }
}