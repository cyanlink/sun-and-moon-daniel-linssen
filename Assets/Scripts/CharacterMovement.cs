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
    public float accelerationFactorInAir = 5;
    
    public DefaultControl DefaultControl { get; private set; }
    
    Rigidbody2D rb;
    private PlayerInput _playerInput;
    private float movement;
    private bool readyToSwitchWorld;

    private bool midAirMovementChance = false;
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
        //横向不能进入的问题是由这里引起的
        //正确逻辑：只要velocity不是0向量，即可进入dark area
        if (rb.velocity != Vector2.zero && readyToSwitchWorld)
        {
            readyToSwitchWorld = false;
            SwitchWorldManager.Instance.SwitchWorld();
        }
    }

    private void FixedUpdate()
    {
        //DONE 不应该是velocity跟随输入突变，原作里都是按force来的
        //TODO 似乎原作手感逻辑是：跳起后或掉落，在空中允许一次常规加速度横向移动，而后续移动只能以小加速度，而落地、进入dark area等操作，均可以重置这个机会
        if (movement != 0)
        {
            var groundOrDive = IsTouchingGroundLayer();
            if (groundOrDive || midAirMovementChance)
            {
                //rb.velocity = new(movement.x * speed, rb.velocity.y);
                rb.AddForce(movement * accelerationFactor * Vector2.right, ForceMode2D.Force);
                midAirMovementChance = groundOrDive;
            }
            else
            {
                //mid air logic
                rb.AddForce(movement * accelerationFactorInAir * Vector2.right, ForceMode2D.Force);
            }
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
    public bool IsTouchingGroundLayer()
    {
        //FIXME should change to ray casting or foot detecting
        var grounded = rb.IsTouchingLayers(groundLayerMask);
        return grounded;
    }

    public bool IsInDarkArea()
    {
        var isInDark = SwitchWorldManager.InsideDarkArea;
        return isInDark;
    }
    

    public void OnMove(InputAction.CallbackContext ctx)
    {
        //Debug.Log("Moving");
        movement = ctx.ReadValue<float>();
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        //只有在地面上的时候，跳跃操作才有意义
        if (IsTouchingGroundLayer() && !IsInDarkArea())
        {
            if (ctx.performed)
            {
                //Horizontal Movement is always Allowed
                //Debug.Log($"movement.y:{movement.y}");
                rb.AddForce(jumpForce * Vector2.up, ForceMode2D.Impulse);
            }
            else if (ctx.canceled)
            {
                //this is kind of correct
                rb.velocity = new(rb.velocity.x, 0);
            }
        }
    }

    /// <summary>
    /// Only when horizontal velocity != 0 can we allow diving
    /// </summary>
    /// <param name="ctx"></param>
    public void OnSwitchWorld(InputAction.CallbackContext ctx)
    {
        //FIXME 现阶段，横向进入dark area有问题
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
