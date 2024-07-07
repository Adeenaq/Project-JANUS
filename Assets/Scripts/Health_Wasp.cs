using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health_Wasp : MonoBehaviour
{
    [SerializeField] private int _maxhp = 1000;
    [SerializeField] private int _hp;
    [SerializeField] private int thrillValue = 10;
    [SerializeField] private Thrill_Player player;

    private bool takingDamage = false;
    private bool dead = false;
    Animator animator;
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
            if (_hp <= 0 && !dead)
            {
                dead = true;
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
        if (takingDamage)
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
        player = GameObject.FindWithTag("Player").GetComponent<Thrill_Player>();
        uiManager = GameObject.FindWithTag("UIManager").GetComponent<UIManager>();
    }

    public void Damage(int amount)
    {
        Hp -= amount;
        takingDamage = true;
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
        if (death != null)
        {
            PlaySound(death, deathVolume);
        }
        animator.Play("wasp_death", 0, 0f);
        StartCoroutine(DestroyAfterSound(death.length));
    }

    private IEnumerator DestroyAfterSound(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
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
