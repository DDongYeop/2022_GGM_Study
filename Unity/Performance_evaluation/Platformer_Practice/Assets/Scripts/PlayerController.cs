using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Player Properties")]
    public float walkSpeed = 10;
    public float gravity = 20f;
    public float jumpSpeed = 15f;
    public float wallSlideAmount = 0.1f;
    public float dashSpeed = 40f;
    public float dashTime = .2f;
    public float dashCooldownTime = 1f;

    [Header("Player Abilities")]
    public bool canWallSlide;
    public bool canAirDash;
    public bool canGroundDash;

    [Header("Player States")]
    public bool isJumping;
    public bool isWallSliding;
    public bool isDashing;

    private bool _startJump;
    private bool _realeaseJump;

    private Vector2 _input;
    private Vector2 _moveDirection;
    private CharactorController2D _charactorController;

    private void Awake()
    {
        _charactorController = GetComponent<CharactorController2D>();
    }

    private void Update()
    {
        PlayerMove();
        PlayerDash();
        PlayerJump();
        SpriteFlip();

        dashCooldownTime -= Time.deltaTime;
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        _input = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _startJump = true;
            _realeaseJump = false;
        }
        else if (context.canceled)
        {
            _startJump = false;
            _realeaseJump = true;
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (dashCooldownTime >= 0)
            return;

        dashCooldownTime = 1f;
        if (context.started)
        {
            if (canAirDash)
                StartCoroutine(Dash());
        }
    }

    private void PlayerJump()
    {
        if (_charactorController.below)
            OnGround();
        else 
            InAir();

        _charactorController.Move(_moveDirection * Time.deltaTime);
    }

    private void OnGround()
    {
        _moveDirection.y = 0;
        ClearAirAbillityFlags();

        Jump();
    }

    private void ClearAirAbillityFlags()
    {
        isJumping = false;
        isWallSliding = false;
    }


    private void Jump()
    {
        if (_startJump)
        {
            _startJump = false;
            _moveDirection.y = jumpSpeed;
            isJumping = true;
            _charactorController.DisableGroundCheck(0.1f);
        }
    }

    private void InAir()
    {
        if (_realeaseJump)
        {
            _realeaseJump = false;
            if (_moveDirection.y > 0)
                _moveDirection.y *= 0.5f;
        }

        if (_startJump)
            _startJump = false;

        GravityCalculation();
    }

    private void GravityCalculation()
    {
        if (isDashing)
            return;

        if (canWallSlide && (_charactorController.left || _charactorController.right))
        {
            if (_moveDirection.y <= 0)
                _moveDirection.y -= gravity * wallSlideAmount * Time.deltaTime;
            else
                _moveDirection.y -= gravity * Time.deltaTime;

            isWallSliding = true;
        }
        else
            _moveDirection.y -= gravity * Time.deltaTime;
    }

    private void PlayerMove()
    {
        _moveDirection.x = _input.x * walkSpeed;
    }

    private void SpriteFlip()
    {
        if (_input.x > 0)
            transform.localScale = Vector3.one;
        else if (_input.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }


    private void PlayerDash()
    {
        if (isDashing)
        {
            if (transform.localScale.x  == 1)
                _moveDirection.x = dashSpeed;
            else if (transform.localScale.x == -1)
                _moveDirection.x = -dashSpeed;
        }
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        yield return new WaitForSeconds(dashTime);
        isDashing = false;
    }
}
