using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 7f;
    public float jumpForce = 10f; // Add a jump force variable
    public Transform[] groundChecks; // Array to hold ground check transforms for both players
    public LayerMask groundLayer; // Layer mask to specify what is considered ground
    private Vector2 moveInput;
    public Rigidbody2D[] rbs; // Array to hold Rigidbody2D components for both players
    public Weapon weapon;

    private bool isGrounded; // Variable to track if either player is on the ground
    private bool canJump = true; // Cooldown to prevent multiple jumps in quick succession
    public float jumpCooldown = 0.1f; // Cooldown duration

    public bool IsMoving { get; private set; }

    private bool _isFacingRight = true;
    public bool IsFacingRight
    {
        get { return _isFacingRight; }
        private set
        {
            if (_isFacingRight != value)
            {
                foreach (var rb in rbs)
                {
                    rb.transform.localScale = new Vector2(-rb.transform.localScale.x, rb.transform.localScale.y);
                }
            }
            _isFacingRight = value;
            UpdateWeaponDirection();
        }
    }

    private PlayerInputActions inputActions;

    private void Awake()
    {
        rbs = GetComponentsInChildren<Rigidbody2D>();
        inputActions = new PlayerInputActions();

        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;
        inputActions.Player.Attack.performed += ctx => { Debug.Log("Attack performed"); weapon.Fire(); };
        inputActions.Player.Jump.performed += ctx => Jump(); // Add jump action handling
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        IsMoving = moveInput != Vector2.zero;
        Debug.Log($"Move Input: {moveInput}");
        SetFacingDirection(moveInput);
    }

    private void SetFacingDirection(Vector2 moveInput)
    {
        if (moveInput.x > 0 && !IsFacingRight)
        {
            IsFacingRight = true;
        }
        else if (moveInput.x < 0 && IsFacingRight)
        {
            IsFacingRight = false;
        }
    }

    private void UpdateWeaponDirection()
    {
        if (weapon != null)
        {
            weapon.SetFirepointDirection(IsFacingRight);
        }
        else
        {
            Debug.LogWarning("Weapon is not assigned to the PlayerController.");
        }
    }

    private void FixedUpdate()
    {
        foreach (var rb in rbs)
        {
            rb.velocity = new Vector2(moveInput.x * walkSpeed, rb.velocity.y);
        }
        CheckGrounded(); // Check if either player is on the ground
    }

    private void CheckGrounded()
    {
        isGrounded = false;

        foreach (Transform groundCheck in groundChecks)
        {
            if (Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer))
            {
                isGrounded = true;
                break;
            }
        }
    }

    private void Jump()
    {
        if (isGrounded && canJump)
        {
            foreach (var rb in rbs)
            {
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
            Debug.Log("Jump performed");
            StartCoroutine(JumpCooldown());
        }
    }

    private IEnumerator JumpCooldown()
    {
        canJump = false;
        yield return new WaitForSeconds(jumpCooldown);
        canJump = true;
    }
}
