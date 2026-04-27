using UnityEngine;

public class MobileInput : MonoBehaviour
{
    [SerializeField] private float inputBufferTime = 0.2f;

    public float moveInput = 0f;
    public bool jumpPressed;
    public bool attackPressed;

    private float jumpBufferTimer;
    private float attackBufferTimer;

    private void Update()
    {
        if (jumpBufferTimer > 0f)
        {
            jumpBufferTimer -= Time.deltaTime;
        }

        if (attackBufferTimer > 0f)
        {
            attackBufferTimer -= Time.deltaTime;
        }

        jumpPressed = jumpBufferTimer > 0f;
        attackPressed = attackBufferTimer > 0f;
    }

    public void MoveLeft()
    {
        moveInput = -1f;
    }

    public void MoveRight()
    {
        moveInput = 1f;
    }

    public void StopMove()
    {
        moveInput = 0f;
    }

    public void Jump()
    {
        jumpBufferTimer = inputBufferTime;
        jumpPressed = true;
    }

    public void Attack()
    {
        attackBufferTimer = inputBufferTime;
        attackPressed = true;
    }

    public bool ConsumeJumpPressed()
    {
        if (!jumpPressed)
        {
            return false;
        }

        jumpBufferTimer = 0f;
        jumpPressed = false;
        return true;
    }

    public bool ConsumeAttackPressed()
    {
        if (!attackPressed)
        {
            return false;
        }

        attackBufferTimer = 0f;
        attackPressed = false;
        return true;
    }

    public void ResetJump()
    {
        // Keep the short input buffer so tap/release in the same frame still jumps.
    }

    public void ResetAttack()
    {
        attackBufferTimer = 0f;
        attackPressed = false;
    }
}
