using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float flyForce = 1f;
    private Rigidbody2D rb;

    void Start(){
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, flyForce);
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        //collision.gameOject = Kollisionsobjekt, davon der tag mit .tag
        //und dann vergleichen ob es mit dem Obstacle Tag (selbst erstellt)
        if (collision.gameObject.tag == "Obstacle")
        {
            Time.timeScale = 0f; //stoppt das Spiel
        }
    }
}
