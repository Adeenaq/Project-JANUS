using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.Rendering.DebugUI;

public class Health_Player : MonoBehaviour
{
    [SerializeField]
    private int _maxhp = 1000;
    [SerializeField] private int _hp;
    Animator[] animators;
    [SerializeField] private bool takingDamage = false;
    [SerializeField] private bool dead = false;
    [SerializeField] private AudioClip shot;
    [SerializeField] private AudioClip death;
    [SerializeField][Range(0, 1)] private float shotVolume = 1.0f; // Volume control for shot sound
    [SerializeField][Range(0, 1)] private float deathVolume = 1.0f; // Volume control for death sound

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
            }
            UpdateHealthUI();
        }
    }

    
    public UnityEvent<int> Healed;
    public UnityEvent<int> Damaged;
    public UnityEvent Died;

    public UIManager uiManager; // Reference to the UIManager

    void Start()
    {
        Hp = _maxhp;
        UpdateHealthUI();
    }

    void Update()
    {
        if (takingDamage == true && dead == false)
        {
            foreach (Animator a in animators)
            {
                a.SetBool("DamageTaken", false);
            }
            takingDamage = false;
        }
        
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            Damage(50);
        }
    }

    private void Awake()
    {
        Hp = _maxhp;
        UpdateHealthUI();
        animators = GetComponentsInChildren<Animator>();
        audioSource=GetComponent<AudioSource>();
    }

    public void Damage(int amount)
    {
        if (!dead)
        {
            foreach (Animator a in animators)
            {
                PlayAnimationIfExists(a, "player_past_damage");
                PlayAnimationIfExists(a, "player_future_damage");
            }
            takingDamage = true;
            Hp -= amount;
            if (shot != null)
            {
                PlaySound(shot, shotVolume);
            }

        }
        

        if (Hp <= 0 && !dead)
        {
            dead = true;
            foreach (Animator a in animators)
            {
                PlayAnimationIfExists(a, "player_past_dead");
                PlayAnimationIfExists(a, "player_future_dead");
            }
        
        if (death != null)
        {
            PlaySound(death, deathVolume);
        }

            Destroy(GetComponent<PlayerController>());

            Rigidbody2D myrb = GetComponent<Rigidbody2D>();
            Vector2 vel = myrb.velocity;
            vel.x = 0f;

            StartCoroutine(waiter(2));

            myrb.gravityScale = 0f;
            myrb.velocity = vel;

            CapsuleCollider2D[] mycols = GetComponentsInChildren<CapsuleCollider2D>();
            foreach (var col in mycols)
            {
                Destroy(col);
            }

            // some gameover UI function to be added here
        }
    }

    IEnumerator waiter(int time)
    {
        yield return new WaitForSeconds(time);
    }

    public void Heal(int amount) => Hp += amount;

    public void HealFull() => Hp = _maxhp;

    public void Kill() => Hp = 0;

    public void Adjust(int value) => Hp = value;

    private void UpdateHealthUI()
    {
        if (uiManager != null)
        {
            uiManager.UpdateHealth(_hp);
        }
    }

    void PlayAnimationIfExists(Animator animator, string animationName)
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
