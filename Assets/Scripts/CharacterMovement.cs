using System;
using QFramework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterMovement : MonoSingleton<CharacterMovement>
{
    public LayerMask groundLayerMask;
    public float jumpForce = 3f;
    public float speed = 5f;
    public float accelerationFactor = 10;
    public DefaultControl DefaultControl { get; private set; }
    
    Rigidbody2D rb;
    private PlayerInput _playerInput;
    private float movement;
    private bool readyToSwitchWorld;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        _playerInput = GetComponent<PlayerInput>();
        DefaultControl = new DefaultControl();
    }

    private void Start()
    {
        //DefaultControl.Gameplay.SwitchWorld.triggered;
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
        //FIXME 不应该是velocity跟随输入突变，原作里都是按force来的
        if (movement != 0)
        {
            //rb.velocity = new(movement.x * speed, rb.velocity.y);
            rb.AddForce(movement * accelerationFactor * Vector2.right, ForceMode2D.Force);
        }
        else
        {
            rb.velocity = new(0,rb.velocity.y);
        }
        var dir = rb.velocity.x.Sign();
        var capped = Mathf.Clamp(rb.velocity.x.Abs(), 0, speed);
        rb.velocity = new(dir * capped, rb.velocity.y);
    }

    /// <summary>
    /// Touching the platform, but not inside of it, only then we allow jumping
    /// </summary>
    /// <returns></returns>
    public bool IsGrounded()
    {
        //FIXME should change to ray casting or foot detecting
        var isInDark = SwitchWorldManager.InsideDarkArea;
        var grounded = rb.IsTouchingLayers(groundLayerMask);
        return (!isInDark) && grounded;
    }
    

    public void OnMove(InputAction.CallbackContext ctx)
    {
        //Debug.Log("Moving");
        movement = ctx.ReadValue<float>();
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            //Horizontal Movement is always Allowed
            
            if (IsGrounded())
            {
                //Debug.Log($"movement.y:{movement.y}");
                rb.AddForce(jumpForce * Vector2.up, ForceMode2D.Impulse);
            }
        }else if (ctx.canceled)
        {
            //this is kind of correct
            rb.velocity = new (rb.velocity.x, 0);
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
            if (!SwitchWorldManager.InsideDarkArea)
            {
                //SwitchWorldManager.Instance.SwitchWorld();
                readyToSwitchWorld = true;
            }
        }

        if (ctx.canceled)
        {
            if (!SwitchWorldManager.InsideDarkArea)
            {
                SwitchWorldManager.Instance.ToggleDarkArea(false);
            }
            else
            {
                SwitchWorldManager.ScheduledTurnOffDarkArea = true;
            }
            readyToSwitchWorld = false;
        }
    }
}
