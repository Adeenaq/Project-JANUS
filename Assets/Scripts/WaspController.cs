using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class WaspController : MonoBehaviour
{
    private float detectionRange = 100f;
    private float divingRange = 25f;
    private float speed = 7f;
    private float divingSpeed = 70f;
    private float stopBeforeDiveTime = 1f;
    private float impactDamageRange = 15f;
    private int explosionDamageAmount = 100;
    private int meleeDamageAmount = 10;

    [Header("Audio")]
    [SerializeField] private AudioClip moveSound;
    [SerializeField] private AudioClip impactSound;
    [Range(0f, 1f)]
    [SerializeField] private float moveVolume;
    [Range(0f, 1f)]
    [SerializeField] private float impactVolume;
    private AudioSource audioSource;

    private Transform player;
    private Transform playerToTrack1;
    private Transform playerToTrack2;
    private Animator[] animators;
    private Rigidbody2D myrb;
    private Vector2 moveInput;
    private Vector2 diveDirection;

    private bool isDiving = false;
    private float stopBeforeDiveTimer = 0f;
    private bool canMelee = true;
    private bool positionRecorded = false;
    private bool colliding = false;

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
            if (_isMoving)
            {
                PlaySound(moveSound, moveVolume);
            }
        }
    }

    private bool _isFacingRight = false;
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
        }
    }

    private void Awake()
    {
        myrb = GetComponent<Rigidbody2D>();
        animators = GetComponentsInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();

        playerToTrack1 = GameObject.Find("Player_top").transform;
        playerToTrack2 = GameObject.Find("Player_bottom").transform;

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
        if (colliding)
        {
            return;
        }

        if (player != null)
        {
            float distanceToPlayer = (transform.position - player.position).sqrMagnitude;

            if (distanceToPlayer <= detectionRange && !isDiving)
            {
                moveInput = (player.position - transform.position).normalized;
                IsMoving = true;
                MoveTowardsPlayer();
            }
            else
            {
                IsMoving = false;
            }

            if (distanceToPlayer <= divingRange && !isDiving)
            {
                moveInput = Vector2.zero;
                myrb.velocity = new Vector2(0, myrb.velocity.y); // Stop moving

                if (!positionRecorded)
                {
                    diveDirection = ((Vector2)player.position - (Vector2)gameObject.transform.position).normalized;
                    positionRecorded = true;
                }
                isDiving = true;
            }

            SetFacingDirection((player.position - transform.position).normalized);
        }

        if (isDiving)
        {
            stopBeforeDiveTimer += Time.deltaTime;

            if (stopBeforeDiveTimer >= stopBeforeDiveTime)
            {
                gameObject.transform.position += (Vector3)diveDirection * divingSpeed * Time.deltaTime;
            }
        }
    }

    private void MoveTowardsPlayer()
    {
        myrb.velocity = new Vector2(moveInput.x * speed, myrb.velocity.y);
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        colliding = true;
        moveInput = Vector2.zero;
        myrb.velocity = new Vector2(0, 0);

        if (canMelee)
        {
            Health_Player player_health = collision.gameObject.GetComponent<Health_Player>();
            if (player_health != null)
            {
                player_health.Damage(meleeDamageAmount);
                StartCoroutine(WaiterMelee(3));
            }
        }

        float sqrDistanceToPlayer = (player.position - gameObject.transform.position).sqrMagnitude;

        if (sqrDistanceToPlayer <= impactDamageRange)
        {
            Health_Player player_health = player.parent.GetComponent<Health_Player>();
            if (player_health != null)
            {
                player_health.Damage(explosionDamageAmount);
            }
            PlaySound(impactSound, impactVolume); // Play impact sound
        }

        foreach (var a in animators)
        {
            PlayAnimationIfExists(a, "wasp_death");
            PlayAnimationIfExists(a, "explosion");
        }

        StartCoroutine(DestroyAfterImpactSound());
    }

    private IEnumerator DestroyAfterImpactSound()
    {
        if (impactSound != null)
        {
            yield return new WaitForSeconds(impactSound.length);
        }
        Destroy(gameObject);
    }

    private IEnumerator WaiterMelee(int time)
    {
        canMelee = false;
        yield return new WaitForSeconds(time);
        canMelee = true;
    }

    private void PlayAnimationIfExists(Animator animator, string animationName)
    {
        if (animator.HasState(0, Animator.StringToHash(animationName)))
        {
            animator.Play(animationName, 0, 0f);
        }
    }

    private void PlaySound(AudioClip clip, float volume)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }
}
