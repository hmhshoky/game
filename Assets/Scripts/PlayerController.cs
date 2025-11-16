using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float flyForce = 2f;

    private Rigidbody2D rb;
    private Animator animator;
    private GameManager gameManager;

    private bool isFlying = false;
    private bool isInAir = false;
    private float fallVelocity = 0f;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        gameManager = FindFirstObjectByType<GameManager>();
    }

    void Update()
    {
        //wenn W gedrückt wird -> true
        isFlying = Input.GetKey(KeyCode.W);

        if (isFlying)
        {
            flyForce += 0.6f * Time.deltaTime * 60f;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, flyForce);
            isInAir = true;
        }else{
            flyForce = 2;
        }
        //übergibt isFlying den true Wert für Flug Animation
        animator.SetBool("isFlying", isFlying);
        
        if (isInAir)
        {
            fallVelocity = rb.linearVelocity.y;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground" && isInAir && fallVelocity < -17f)
        {
            animator.SetTrigger("land");
            isInAir = false;
            fallVelocity = 0f;
        }

        if (collision.gameObject.tag == "Obstacle")
        {
            gameManager.GameOver();
        }
    }
}
