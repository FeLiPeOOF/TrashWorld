using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public MobileInput mobileInput;

    public float speed = 5f;
    public float jumpForce = 7f;

    private Rigidbody2D rb;
    private bool isGrounded;

    private Animator anim;


    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float move = mobileInput.moveInput;
        rb.velocity = new Vector2(move * speed, rb.velocity.y);

        if (mobileInput.jumpPressed && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        anim.SetFloat("Speed", Mathf.Abs(mobileInput.moveInput));
        anim.SetBool("isJumping", !isGrounded);

        // Animation control
        // Animation control
        anim.SetFloat("Speed", Mathf.Abs(mobileInput.moveInput));
        anim.SetBool("isJumping", !isGrounded);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}