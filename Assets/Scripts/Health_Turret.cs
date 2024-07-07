using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health_Turret : MonoBehaviour
{
    [SerializeField] private int _maxhp = 1000;
    [SerializeField] private int _hp;
    [SerializeField] private int thrillValue = 10;
    [SerializeField] private Thrill_Player player;

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
            if (_hp <= 0 && !dead)
            {
                dead = true;
                Died?.Invoke();
                HandleDeath();
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
    }

    void Update()
    {
        if (takingDamage && !dead)
        {
            takingDamage = false;
        }
    }

    private void Awake()
    {
        Hp = _maxhp;
        animators = GetComponentsInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component is missing on Health_Turret.");
        }
        player = GameObject.FindWithTag("Player").GetComponent<Thrill_Player>();
        uiManager = GameObject.FindWithTag("UIManager").GetComponent<UIManager>();
    }

    public void Damage(int amount)
    {
        Hp -= amount;
        player.IncreaseThrill(thrillValue);
        if (shot != null)
        {
            PlaySound(shot, shotVolume);
        }
    }

    private void HandleDeath()
    {
        foreach (var a in animators)
        {
            PlayAnimationIfExists(a, "Explosion");
        }
        if (death != null)
        {
            PlaySound(death, deathVolume);
        }
    }

    private void PlaySound(AudioClip clip, float volume)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }

    IEnumerator waiter(int time)
    {
        yield return new WaitForSeconds(time);
    }

    private void PlayAnimationIfExists(Animator animator, string animationName)
    {
        if (animator.HasState(0, Animator.StringToHash(animationName)))
        {
            animator.Play(animationName, 0, 0f);
            StartCoroutine(WaitForExplosionAnimation(animator));
        }
    }

    private IEnumerator WaitForExplosionAnimation(Animator animator)
    {
        yield return new WaitForEndOfFrame();
        while (animator.GetCurrentAnimatorStateInfo(0).IsName("Explosion") &&
               animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }
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
}
