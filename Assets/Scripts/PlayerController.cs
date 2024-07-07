using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using System.Collections;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;
using System.Linq;
using System.Collections.Generic;
using Unity.VisualScripting;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 7f;
    [SerializeField] private float runSpeed = 14f;
    [SerializeField] private float jumpForce = 10f; // Add a jump force variable
    [SerializeField] private float crouchForce = 3f; // Add a crouch force variable
    [SerializeField] private float cameraOffset;
    [SerializeField] private LayerMask groundLayer; // Layer mask to specify what is considered ground
    private Vector2 moveInput;
    private bool isFlyMode = false;
    private Rigidbody2D myrb;
    [SerializeField] private Rigidbody2D[] rbs; // Array to hold Rigidbody2D components for both players
    [SerializeField] private Weapon weapon;
    Animator[] animators;

    private bool isGrounded; // Variable to track if either player is on the ground
    private bool isJumping;
    private bool isSneaking;
    private bool canJump = true; // Cooldown to prevent multiple jumps in quick succession
    private float jumpCooldown = 0.1f; // Cooldown duration
    private bool isRunning = false;

    private CinemachineVirtualCamera virtCamTop;
    private CinemachineVirtualCamera virtCamBot;
    private CinemachineFramingTransposer framTranTop;
    private CinemachineFramingTransposer framTranBot;

    [SerializeField] AudioClip move;
    [SerializeField] AudioClip jump;
    [SerializeField] AudioClip land;

    [Range(0f, 2f)]
    [SerializeField] float moveVol;
    [Range(0f, 2f)]
    [SerializeField] float jumpVol;
    [Range(0f, 2f)]
    [SerializeField] float landVol;

    private AudioSource audioSource;

    [SerializeField] private bool _isMoving = false;

    private float originalWalkSpeed;
    private float originalJumpForce;

    // Public getters for private variables
    public float WalkSpeed
    {
        get => walkSpeed;
        set => walkSpeed = value;
    }

    public float JumpForce
    {
        get => jumpForce;
        set => jumpForce = value;
    }

    public float OriginalWalkSpeed => originalWalkSpeed;
    public float OriginalJumpForce => originalJumpForce;

    public bool IsMoving
    {
        get
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
            if (_isMoving && isGrounded)
            {
                PlaySound(move, moveVol); // Play moving sound
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

    private void ToggleFly()
    {
        isFlyMode = !isFlyMode;
        Debug.Log("FLY MODE: " + (isFlyMode ? "ON" : "OFF"));

        foreach (var rb in rbs)
        {
            if (isFlyMode)
            {
                rb.gravityScale = 0; // Disable gravity
                rb.isKinematic = true; // Enable kinematic mode to allow clipping through objects
            }
            else
            {
                rb.gravityScale = 3.5f; // Enable gravity
                rb.isKinematic = false; // Disable kinematic mode
            }
        }
    }

    private void Awake()
    {
        rbs = GetComponentsInChildren<Rigidbody2D>();
        myrb = GetComponent<Rigidbody2D>();
        animators = GetComponentsInChildren<Animator>();
        weapon = GetComponentInChildren<Weapon>();
        audioSource = GetComponent<AudioSource>();

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
            if (IsFacingRight)
            {
                myrb.AddForce(new Vector2(-500, 0));
            }
            else
            {
                myrb.AddForce(new Vector2(500, 0));
            }

        };

        virtCamBot = GameObject.Find("VirtualCameraBottom").GetComponent<CinemachineVirtualCamera>();
        virtCamTop = GameObject.Find("VirtualCameraTop").GetComponent<CinemachineVirtualCamera>();

        framTranTop = virtCamTop.GetCinemachineComponent<CinemachineFramingTransposer>();
        framTranBot = virtCamBot.GetCinemachineComponent<CinemachineFramingTransposer>();


        inputActions.Player.Jump.performed += ctx => Jump(); // Add jump action handling
        inputActions.Player.Jump.canceled += ctx => StopJump();
        inputActions.Player.Sneak.performed += ctx => Sneak();
        inputActions.Player.Sneak.canceled += ctx => StopSneak();
        inputActions.Player.FlyToggle.performed += ctx => ToggleFly();
        inputActions.Player.Run.performed += ctx => StartRunning();
        inputActions.Player.Run.canceled += ctx => StopRunning();
        inputActions.Player.Zoom.performed += ctx => StartZoom(8.0f);
        inputActions.Player.Zoom.canceled += ctx => StartZoom(6.0f);

        if ((framTranTop == null) || (framTranBot == null))
        {
            Debug.Log("One or more virtual cameras not found");
        }

        // Store the original speeds for restoring later
        originalWalkSpeed = walkSpeed;
        originalJumpForce = jumpForce;
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

        if (isFlyMode)
        {
            myrb.velocity = new Vector2(moveInput.x * (isRunning ? runSpeed : walkSpeed), myrb.velocity.y);
        }
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
        if (isFlyMode)
        {
            myrb.velocity = new Vector2(moveInput.x * (isRunning ? runSpeed : walkSpeed), isJumping ? (isRunning ? runSpeed : walkSpeed) : (isSneaking ? -(isRunning ? runSpeed : walkSpeed) : 0));
            return;
        }

        if (isSneaking)
        {
            foreach (var rb in rbs)
            {
                rb.AddForce(Vector2.down * crouchForce, ForceMode2D.Impulse);
            }
        }

        if (rbs != null && rbs.Length > 0)
        {
            foreach (var rb in rbs)
            {
                rb.velocity = new Vector2(moveInput.x * (isRunning ? runSpeed : walkSpeed), rb.velocity.y);
            }
            CheckGrounded(); // Check if either player is on the ground
        }
        else
        {
            Debug.LogError("No Rigidbody2D components found in children.");
        }

        if (myrb.velocity.y > 0.2)
        {
            foreach (Animator a in animators)
            {
                a.SetBool("Jumping", true);
            }
        }
        else if (myrb.velocity.y < -0.5 && !isSneaking)
        {
            foreach (Animator a in animators)
            {
                a.SetBool("Jumping", true);
                a.SetBool("Falling", true);
            }
        }
    }

    private void CheckGrounded()
    {
        if (isGrounded)
        {
            foreach (Animator a in animators)
            {
                a.SetBool("Jumping", false);
                a.SetBool("Falling", false);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        bool prevGrounded = isGrounded;

        GameObject expectTilemap = collision.gameObject;
        Transform expectEnemies = collision.gameObject.transform.parent;
        if ((expectTilemap != null && expectTilemap.name == "Tilemap") ||
            (expectEnemies != null && expectEnemies.name == "Enemies"))
        {
            isGrounded = true;
        }

        if (!prevGrounded && isGrounded)
        {
            foreach (Animator a in animators)
            {
                PlayAnimationIfExists(a, "player_past_land");
                PlayAnimationIfExists(a, "player_future_land");
            }

            framTranTop.m_DeadZoneHeight = 0.0f;
            framTranBot.m_DeadZoneHeight = 0.0f;
            PlaySound(land, landVol); // Play landing sound
        }
    }

    private void Jump()
    {
        isJumping = true;
        if (isFlyMode)
        {
            return;
        }

        if (isGrounded && canJump)
        {
            if (rbs != null && rbs.Length > 0)
            {
                foreach (var rb in rbs)
                {
                    rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                    isGrounded = false;
                }

                framTranTop.m_DeadZoneHeight = 1.0f;
                framTranBot.m_DeadZoneHeight = 1.0f;
            }
            else
            {
                Debug.LogError("No Rigidbody2D components found in children.");
            }

            Debug.Log("Jump performed");
            PlaySound(jump, jumpVol); // Play jumping sound
            StartCoroutine(JumpCooldown());
        }
    }

    private void Sneak()
    {
        isSneaking = true;
        // foreach (Animator a in animators)
        // {
        //     a.SetBool("IsCrouching", true);
        // }
    }

    private void StopJump()
    {
        isJumping = false;
    }

    private void StopSneak()
    {
        isSneaking = false;
        // foreach (Animator a in animators)
        // {
        //     a.SetBool("IsCrouching", false);
        // }
    }

    private void StartRunning()
    {
        Debug.Log("Running");
        isRunning = true;
        foreach (Animator a in animators)
        {
            a.speed = 1.5f; // Reset animation speed to normal
        }
    }

    private void StopRunning()
    {
        Debug.Log("Stopped running");
        isRunning = false;
        foreach (Animator a in animators)
        {
            a.speed = 1f; // Reset animation speed to normal
        }
    }

    private IEnumerator JumpCooldown()
    {
        canJump = false;
        yield return new WaitForSeconds(jumpCooldown);
        canJump = true;
    }

    private void PlaySound(AudioClip clip, float volume)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }
    private void StartZoom(float targetZoom)
    {
        StopAllCoroutines();
        StartCoroutine(SmoothZoom(targetZoom, 0.2f)); // Adjust the duration as needed
    }

    private IEnumerator SmoothZoom(float targetZoom, float duration)
    {
        float startZoomTop = virtCamTop.m_Lens.OrthographicSize;
        float startZoomBot = virtCamBot.m_Lens.OrthographicSize;
        float timeElapsed = 0f;

        framTranBot.m_YDamping = 8.0f;
        framTranBot.m_XDamping = 8.0f;
        framTranTop.m_YDamping = 8.0f;
        framTranTop.m_XDamping = 8.0f;

        while (timeElapsed < duration)
        {
            virtCamTop.m_Lens.OrthographicSize = Mathf.Lerp(startZoomTop, targetZoom, timeElapsed / duration);
            virtCamBot.m_Lens.OrthographicSize = Mathf.Lerp(startZoomBot, targetZoom, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        virtCamTop.m_Lens.OrthographicSize = targetZoom;
        virtCamBot.m_Lens.OrthographicSize = targetZoom;

        timeElapsed = 0f;
        while (timeElapsed < 0.2)
        {
            timeElapsed += Time.deltaTime;
        }

        framTranBot.m_YDamping = 2.0f;
        framTranBot.m_XDamping = 2.0f;
        framTranTop.m_YDamping = 2.0f;
        framTranTop.m_XDamping = 2.0f;
    }
}
