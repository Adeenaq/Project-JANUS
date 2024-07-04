using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health_Turret : MonoBehaviour
{
    [SerializeField] private int _maxhp = 1000;
    [SerializeField] private int _hp;
    private bool takingDamage = false;
    private bool dead = false;
    Animator[] animators;

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
    }

    public void Damage(int amount)
    {
        Hp -= amount;

        if (Hp <= 0 && !dead)
        {
            dead = true;
            //StartCoroutine(waiter(2));
            foreach (var a in animators)
            {
                PlayAnimationIfExists(a, "Explosion");
            }
        }
    }

    IEnumerator waiter(int time)
    {
        yield return new WaitForSeconds(time);
    }

    void PlayAnimationIfExists(Animator animator, string animationName)
    {
        if (animator.HasState(0, Animator.StringToHash(animationName)))
        {
            animator.Play(animationName, 0, 0f);
            StartCoroutine(WaitForExplosionAnimation(animator));
            //Destroy(GetComponent<SpriteRenderer>());
            //Destroy(gameObject);

        }
    }

    private IEnumerator WaitForExplosionAnimation(Animator animator)
    {
        //Debug.Log("Waiting for explosion animation to start...");
        yield return new WaitForEndOfFrame();

        //Debug.Log("Current State: " + animator.GetCurrentAnimatorStateInfo(0).IsName("Explosion"));

        while (animator.GetCurrentAnimatorStateInfo(0).IsName("Explosion") &&
               animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            //Debug.Log("Explosion animation progress: " + animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
            yield return null;
        }

        // Destroy the turret after the explosion animation has finished
        Destroy(gameObject);
    }

    public void Heal(int amount) => Hp += amount;

    public void HealFull() => Hp = _maxhp;

    public void Kill() => Hp = 0;

    public void Adjust(int value) => Hp = value;

    private void UpdateHealthUI()
    {
    }
}
