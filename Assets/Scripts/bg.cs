using UnityEngine;

public class EndlessBackground : MonoBehaviour
{
    public float speed = 2f;
    public Transform otherBackground; //anderer BackGround
    private float width;
    
    void Start()
    {
        //berechne Breite (mit Scale)
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        width = sr.bounds.size.x;
    }
    
    void Update()
    {
        //Links Bewegung
        transform.Translate(Vector3.left * speed * Time.deltaTime);
        
        //transform.position.x = aktuelle x des Hintergrunds
        //z.B. wenn x = -10 < width = -10 dann true
        if (transform.position.x < -width)
        {
            //neuen Vektor erstellen 
            transform.position = new Vector3(
                //x Position + Width
                otherBackground.position.x + width,
                transform.position.y, //gleich!
                transform.position.z  //gleich!
            );
        }
    }
}