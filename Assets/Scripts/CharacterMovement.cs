using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterMovement : MonoBehaviour
{
    public LayerMask groundLayerMask;
    public float JumpForce = 3;
    
    Rigidbody2D rb;
    private Vector2 movement;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        rb.velocity = new(movement.x, rb.velocity.y);

    }

    /// <summary>
    /// Touching the platform, but not inside of it, only then we allow jumping
    /// </summary>
    /// <returns></returns>
    public bool IsGrounded()
    {
        var isInDark = SwitchWorldManager.InsideDarkArea;
        var grounded = rb.IsTouchingLayers(groundLayerMask);
        return (!isInDark) && grounded;
    }
    

    public void OnMove(InputAction.CallbackContext ctx)
    {
        //Debug.Log("Moving");
        movement = ctx.ReadValue<Vector2>();

        if (ctx.performed)
        {
            //Horizontal Movement is always Allowed
            
            if (IsGrounded())
            {
                if (movement.y > 0)
                {
                    rb.AddForce(JumpForce * Vector2.up, ForceMode2D.Impulse);
                }
            }
        }
    }

    /// <summary>
    /// Only when horizontal velocity != 0 can we allow diving
    /// </summary>
    /// <param name="ctx"></param>
    public void OnSwitchWorld(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            //anyway we just dont schedule a turn off as long as you press it
            SwitchWorldManager.ScheduledTurnOffDarkArea = false;
            if (rb.velocity.y != 0 && !SwitchWorldManager.InsideDarkArea)
            {
                SwitchWorldManager.Instance.SwitchWorld();
            }
        }

        if (ctx.canceled)
        {
            SwitchWorldManager.ScheduledTurnOffDarkArea = true;
        }
    }
}
