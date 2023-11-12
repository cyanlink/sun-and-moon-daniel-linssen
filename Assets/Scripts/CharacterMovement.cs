using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterMovement : MonoBehaviour
{
    public LayerMask groundLayerMask;
    public float jumpForce = 3f;
    public float speed = 5f;
    
    Rigidbody2D rb;
    private PlayerInput _playerInput;
    private Vector2 movement;
    private bool readyToSwitchWorld;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        _playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        if (rb.velocity.y != 0 && readyToSwitchWorld)
        {
            readyToSwitchWorld = false;
            SwitchWorldManager.Instance.SwitchWorld();
        }
    }

    private void FixedUpdate()
    {
        if(movement.x != 0)
            rb.velocity = new(movement.x * speed, rb.velocity.y);
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
                    rb.AddForce(jumpForce * Vector2.up, ForceMode2D.Impulse);
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
            if (!SwitchWorldManager.InsideDarkArea)
            {
                //SwitchWorldManager.Instance.SwitchWorld();
                readyToSwitchWorld = true;
            }
        }

        if (ctx.performed)
        {
            SwitchWorldManager.ScheduledTurnOffDarkArea = true;
            readyToSwitchWorld = false;
        }
    }
}
