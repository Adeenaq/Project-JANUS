using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using System.Collections;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;
using System.Linq;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 7f;
    [SerializeField] private float jumpForce = 10f; // Add a jump force variable
    [SerializeField] private Transform[] groundChecks; // Array to hold ground check transforms for both players
    [SerializeField] private float cameraOffset;
    [SerializeField] private LayerMask groundLayer; // Layer mask to specify what is considered ground
    private Vector2 moveInput;
    private Rigidbody2D myrb;
    [SerializeField] private Rigidbody2D[] rbs; // Array to hold Rigidbody2D components for both players
    [SerializeField] private Weapon weapon;
    Animator[] animators;

    private bool isGrounded; // Variable to track if either player is on the ground
    private bool canJump = true; // Cooldown to prevent multiple jumps in quick succession
    private float jumpCooldown = 0.1f; // Cooldown duration

    private CinemachineVirtualCamera virtCamTop;
    private CinemachineVirtualCamera virtCamBot;
    private CinemachineFramingTransposer framTranTop;
    private CinemachineFramingTransposer framTranBot;

    [SerializeField] private bool _isMoving = false;
    public bool IsMoving 
    {   get 
        {
            return _isMoving;
        } 
        private set 
        {
            _isMoving = value;
            foreach (Animator a in animators)
            {
                a.SetBool("IsMoving", value);
            }
        } 
    }

    private bool _isFacingRight = true;
    public bool IsFacingRight
    {
        get { return _isFacingRight; }
        private set
        {
            if (_isFacingRight != value)
            {   
                if (rbs != null && rbs.Length > 0)
                    {
                        foreach (var rb in rbs)
                        {
                            rb.transform.localScale = new Vector2(-rb.transform.localScale.x, rb.transform.localScale.y);
                        }
                    }
                    else
                    {
                        Debug.LogError("No Rigidbody2D components found in children.");
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
        animators = GetComponentsInChildren<Animator>();
    
        inputActions = new PlayerInputActions();

        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;
        inputActions.Player.Attack.performed += ctx => 
        {
            foreach (Animator a in animators)
            {
                PlayAnimationIfExists(a, "player_past_fire");
                PlayAnimationIfExists(a, "player_future_fire");
            }
            Debug.Log("Attack performed");
            weapon.Fire();

        };
        inputActions.Player.Jump.performed += ctx => Jump(); // Add jump action handling

        myrb = GetComponent<Rigidbody2D>();
        framTranTop = GameObject.Find("VirtualCameraTop").GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>();
        framTranBot = GameObject.Find("VirtualCameraBottom").GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    void PlayAnimationIfExists(Animator animator, string animationName)
    {
        if (animator.HasState(0, Animator.StringToHash(animationName)))
        {
            animator.Play(animationName, 0, 0f);
        }
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
            framTranTop.m_ScreenX = 0.5f - cameraOffset;
            framTranBot.m_ScreenX = 0.5f - cameraOffset;
        }
        else if (moveInput.x < 0 && IsFacingRight)
        {
            IsFacingRight = false;
            framTranTop.m_ScreenX = 0.5f + cameraOffset;
            framTranBot.m_ScreenX = 0.5f + cameraOffset;
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
        if (rbs != null && rbs.Length > 0)
        {
               foreach (var rb in rbs)
                {
                    rb.velocity = new Vector2(moveInput.x * walkSpeed, rb.velocity.y);
                }
                CheckGrounded(); // Check if either player is on the ground
        }
        else
        {
            Debug.LogError("No Rigidbody2D components found in children.");
        }

        //if (myrb.velocity.y == 0)
        //{    
        //}
        if (myrb.velocity.y > 0.1)
        {
            foreach (Animator a in animators)
            {
                a.SetBool("Jumping", true);
            }
        }
        else if (myrb.velocity.y < -0.1)
        {
            foreach (Animator a in animators)
            {
                //a.SetBool("Jumping", false);
                a.SetBool("Falling", true);
            }
        }
    }

    private void CheckGrounded()
    {
        isGrounded = false;

        foreach (Transform groundCheck in groundChecks)
        {
            if (Physics2D.OverlapCircle(groundCheck.position, 0.5f, groundLayer))
            {
                isGrounded = true;
                foreach (Animator a in animators)
                {
                    a.SetBool("Jumping", false);
                    a.SetBool("Falling", false);
                }
                break;
            }
        }
    }

    private void Jump()
    {
        if (isGrounded && canJump)
        {   
            if (rbs != null && rbs.Length > 0)
            {
                   foreach (var rb in rbs)
                    {
                        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                    }
            }
            else
            {
                Debug.LogError("No Rigidbody2D components found in children.");
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
