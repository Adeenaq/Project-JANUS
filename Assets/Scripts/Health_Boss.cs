using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health_Boss : MonoBehaviour
{
    private int _maxhp = 20000;
    [SerializeField] private int _hp;
    private bool takingDamage = false;
    private bool dead = false;
    Animator[] animators;
    [SerializeField] private AudioClip death;
    [SerializeField] private AudioClip shot;
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
            //animator.SetBool("DamageTaken", false);
            takingDamage = false;
        }
    }

    private void Awake()
    {
        Hp = _maxhp;
        animators = GetComponentsInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    public void Damage(int amount)
    {
        foreach (var a in animators)
        {
            //PlayAnimationIfExists(a, "boss_damage");
        }
        Hp -= amount;
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
        foreach (var a in animators)
        {
            //PlayAnimationIfExists(a, "boss_dead");
        }

        if (death != null)
        {
            PlaySound(death, deathVolume);
        }

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
