using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float flyForce = 2f;
    
    private Rigidbody2D rb;
    private Animator animator;
    private bool isFlying = false;
    private bool isInAir = false;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    
    void Update()
    {
        //wenn W gedrückt wird, isFlying = true
        isFlying = Input.GetKey(KeyCode.W);

        if (isFlying)
        {
            flyForce += 0.7f;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, flyForce);
            isInAir = true;
        }
        if(isFlying == false)
        {
            flyForce = 2;
        }

        //setzt isFlying Parameter (aus Animation) auf true
        animator.SetBool("isFlying", isFlying);
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        //wenn es mit dem Boden (getagged mit Ground) kollidiert und isInAir true ist
        if (collision.gameObject.tag == "Ground" && isInAir)
        {
            animator.SetTrigger("land");
            isInAir = false;
        }
        
        if (collision.gameObject.tag == "Obstacle")
        {
            Time.timeScale = 0f;
        }
    }
}