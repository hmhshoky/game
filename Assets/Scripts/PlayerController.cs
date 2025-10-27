using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float flyForce = 1f;
    
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
        //wenn W gedürckt wird, isFlying = true
        isFlying = Input.GetKey(KeyCode.W);

        if (isFlying)
        {
            flyForce = flyForce + 0.08f;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, flyForce);
            isInAir = true;
        }
        if(isFlying == false)
        {
            flyForce = 1f;
        }

        // Animation: Fly
        animator.SetBool("isFlying", isFlying);
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            if (isInAir)
            {
                animator.SetTrigger("land");
                isInAir = false;
            }
        }
        
        if (collision.gameObject.tag == "Obstacle")
        {
            Time.timeScale = 0f;
        }
    }
}