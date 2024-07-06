using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health_Knight : MonoBehaviour
{
    [SerializeField] private int _maxhp = 1000;
    [SerializeField] private int _hp;
    [SerializeField] private int thrillValue = 10; 

    private bool takingDamage = false;
    private bool dead = false;
    Animator animator;
    [SerializeField] private AudioClip death;
    [SerializeField] private AudioClip shot;
    [SerializeField][Range(0, 1)] private float shotVolume = 1.0f; // Volume control for shot sound
    [SerializeField][Range(0, 1)] private float deathVolume = 1.0f; // Volume control for death sound
    [SerializeField] private Thrill_Player player;


    private AudioSource audioSource;

    public int Hp
    {
        get => _hp;
        private set
        {
            var isDamage = value < _hp;
            _hp = Mathf.Clamp(value, 0, _maxhp);
            if (isDamage)
            {
                Damaged?.Invoke(_hp);
            }
            else
            {
                Healed?.Invoke(_hp);
            }
            if (_hp <= 0)
            {
                Died?.Invoke();
                HandleDeath();
            }
            UpdateHealthUI();
          
        }
    }

    private UnityEvent<int> Healed;
    private UnityEvent<int> Damaged;
    private UnityEvent Died;

    public UIManager uiManager; // Reference to the UIManager

    void Start()
    {
        Hp = _maxhp;
    }

    void Update()
    {
        if (takingDamage == true && dead == false)
        {
            animator.SetBool("DamageTaken", false);
            takingDamage = false;
        }
    }

    private void Awake()
    {
        Hp = _maxhp;
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    public void Damage(int amount)
    {
        animator.Play("knight_damage", 0, 0f);
        Hp -= amount;
        player.IncreaseThrill(thrillValue);
        if (shot != null)
        {
            PlaySound(shot, shotVolume);
        }

        if (Hp <= 0 && !dead)
        {
            dead = true;
            HandleDeath();
        }
    }

    private void HandleDeath()
    {
        animator.Play("knight_dead", 0, 0f);

        if (death != null)
        {
            PlaySound(death, deathVolume);
        }

        Destroy(GetComponent<KnightController>());

        Rigidbody2D myrb = GetComponent<Rigidbody2D>();
        Vector2 vel = myrb.velocity;
        vel.x = 0f;

        myrb.gravityScale = 0f;
        myrb.velocity = vel;
        Destroy(GetComponent<CapsuleCollider2D>());

        StartCoroutine(waiter(2));
    }

    IEnumerator waiter(int time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject); // Destroy the knight GameObject after waiting
    }

    public void Heal(int amount) => Hp += amount;

    public void HealFull() => Hp = _maxhp;

    public void Kill() => Hp = 0;

    public void Adjust(int value) => Hp = value;

    private void UpdateHealthUI()
    {
        // Implementation for updating health UI
    }

    private void PlaySound(AudioClip clip, float volume)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }
}
