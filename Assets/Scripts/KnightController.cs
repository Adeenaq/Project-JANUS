using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class KnightController : MonoBehaviour
{
    private float walkSpeed = 3f;
    private float detectionRange = 20f;
    private float stopRange = 5f;
    private float fireRate = 3f;
    private int collisionDamage = 50;
    private int meleeCooldown = 3;
    private bool canMelee = true;
    private WeaponKnight weapon;
    private Transform player;
    private Transform playerToTrack1;
    private Transform playerToTrack2;
    private Rigidbody2D myrb;
    private Animator animator;
    private bool canFire = true;
    private Vector2 moveInput;
    [SerializeField] AudioClip moveSound;
    [Range(0f, 1f)]
    [SerializeField] float moveVol; 
    private AudioSource audioSource;

    private bool _isMoving = false;
    public bool IsMoving
    {
        get
        {
            return _isMoving;
        }
        private set
        {
            _isMoving = value;
            animator.SetBool("IsMoving", value);
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
                myrb.transform.localScale = new Vector2(-myrb.transform.localScale.x, myrb.transform.localScale.y);
            }
            _isFacingRight = value;
            UpdateWeaponDirection();
        }
    }

    private void Awake()
    {
        myrb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        weapon = GetComponentInChildren<WeaponKnight>();
        playerToTrack1 = GameObject.Find("Player_top").transform;
        playerToTrack2 = GameObject.Find("Player_bottom").transform;
        audioSource = gameObject.AddComponent<AudioSource>();

        if (Vector2.Distance(transform.position, playerToTrack1.position) >= Vector2.Distance(transform.position, playerToTrack2.position))
        {
            player = playerToTrack2;
        }
        else
        {
            player = playerToTrack1;
        }
    }

    private void FixedUpdate()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer <= detectionRange && distanceToPlayer > stopRange)
            {
                moveInput = (player.position - transform.position).normalized;
                IsMoving = true;
                MoveTowardsPlayer();
            }
            else
            {
                moveInput = Vector2.zero;
                myrb.velocity = new Vector2(0, myrb.velocity.y); // Stop moving
                IsMoving = false;
            }

            if (distanceToPlayer <= detectionRange)
            {
                if (canFire)
                {
                    FireWeapon();
                }
            }

            SetFacingDirection((player.position - transform.position).normalized);
        }
    }

    private void MoveTowardsPlayer()
    {
        myrb.velocity = new Vector2(moveInput.x * walkSpeed, myrb.velocity.y);
        //PlaySound(moveSound, moveVol);
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

    private void FireWeapon()
    {
        animator.Play("knight_fire", 0, 0f);
        weapon.Fire();
        StartCoroutine(FireCooldown());
    }

    private IEnumerator FireCooldown()
    {
        canFire = false;
        yield return new WaitForSeconds(fireRate);
        canFire = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (canMelee)
        {
            Health_Player player_health = collision.gameObject.GetComponent<Health_Player>();
            if (player_health != null)
            {
                animator.Play("knight_melee", 0, 0.3f);
                player_health.Damage(collisionDamage);
                waiterMelee(meleeCooldown);
            }
        }
    }

    IEnumerator waiter(int time)
    {
        yield return new WaitForSeconds(time);
    }

    IEnumerator waiterMelee(int time)
    {
        canMelee = false;
        yield return new WaitForSeconds(time);
        canMelee = true;
    }

    private void PlaySound(AudioClip clip, float volume)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }
}
