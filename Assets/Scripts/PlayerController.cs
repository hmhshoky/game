using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float flyForce = 1f;
    private Rigidbody2D rb;

    void Start(){
        rb = GetComponent<Rigidbody2D>();
    }

    void Update(){
        if (Input.GetKey(KeyCode.Space)){
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, flyForce);
        }
    }
}
