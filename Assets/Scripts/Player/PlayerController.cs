using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public MobileInput mobileInput;

    [Header("Movement")]
    public float speed = 5f;
    public float jumpForce = 14f;
    public float groundCheckRadius = 0.15f;
    public Vector2 groundCheckOffset = new Vector2(0f, -0.7f);

    private Rigidbody2D rb;
    private Collider2D playerCollider;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private bool isGrounded;
    private float lastFacingDirection = 1f;

    public bool IsGrounded => isGrounded;
    public float FacingDirection => Mathf.Sign(lastFacingDirection);

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        float moveInput = GetMoveInput();
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);

        UpdateGroundedState();
        UpdateFacing(moveInput);

        if (ConsumeJumpPressed() && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isGrounded = false;
        }

        UpdateAnimation(moveInput);
    }

    private float GetMoveInput()
    {
        float keyboardInput = Input.GetAxisRaw("Horizontal");

        if (Mathf.Abs(keyboardInput) > 0.01f)
        {
            return keyboardInput;
        }

        return mobileInput != null ? mobileInput.moveInput : 0f;
    }

    private bool ConsumeJumpPressed()
    {
        bool keyboardJump = Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);
        bool mobileJump = mobileInput != null && mobileInput.ConsumeJumpPressed();
        return keyboardJump || mobileJump;
    }

    private void UpdateGroundedState()
    {
        Vector2 checkPosition = GetGroundCheckPosition();
        Collider2D[] hits = Physics2D.OverlapCircleAll(checkPosition, groundCheckRadius);

        isGrounded = false;

        for (int i = 0; i < hits.Length; i++)
        {
            Collider2D hit = hits[i];

            if (hit.attachedRigidbody == rb)
            {
                continue;
            }

            if (hit.CompareTag("Ground"))
            {
                isGrounded = true;
                break;
            }
        }
    }

    private Vector2 GetGroundCheckPosition()
    {
        if (playerCollider == null)
        {
            return (Vector2)transform.position + groundCheckOffset;
        }

        Bounds bounds = playerCollider.bounds;
        return new Vector2(bounds.center.x, bounds.min.y - 0.05f);
    }

    private void UpdateFacing(float moveInput)
    {
        if (Mathf.Abs(moveInput) < 0.01f)
        {
            return;
        }

        lastFacingDirection = Mathf.Sign(moveInput);

        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = lastFacingDirection < 0f;
        }
    }

    private void UpdateAnimation(float moveInput)
    {
        if (anim == null)
        {
            return;
        }

        anim.SetFloat("Speed", Mathf.Abs(moveInput));
        anim.SetBool("isJumping", !isGrounded);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(GetGroundCheckPosition(), groundCheckRadius);
    }
}
