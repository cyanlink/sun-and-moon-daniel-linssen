using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterMovement : MonoBehaviour
{
    [SerializeField] public LayerMask groundLayerMask;
    
    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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
        var value = ctx.ReadValue<Vector2>();
        //Horizontal Movement is always Allowed
        rb.velocity = new (value.x, rb.velocity.y);
        if (IsGrounded())
        {
            if (value.y > 0)
            {
                rb.AddForce(Vector2.up, ForceMode2D.Impulse);
            }
        }
    }

    /// <summary>
    /// Only when horizontal velocity != 0 can we allow diving
    /// </summary>
    /// <param name="ctx"></param>
    public void OnSwitchWorld(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
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
