using UnityEngine;

public class MobileInput : MonoBehaviour
{
    public float moveInput = 0f;
    public bool jumpPressed = false;
    public bool attackPressed = false;

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
        jumpPressed = true;
    }

    public void Attack()
    {
        attackPressed = true;
    }

    public void ResetJump()
    {
        jumpPressed = false;
    }

    public void ResetAttack()
    {
        attackPressed = false;
    }
}