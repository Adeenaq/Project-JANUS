using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health_Turret : MonoBehaviour
{
    [SerializeField] private int _maxHp = 1000;
    [SerializeField] private int _hp;
    [SerializeField] private RuntimeAnimatorController animatorController; // Public field to assign Animator Controller in Inspector
    private bool dead = false;
    private Animator animator;

    public int Hp
    {
        get => _hp;
        private set
        {
            var isDamage = value < _hp;
            _hp = Mathf.Clamp(value, 0, _maxHp);
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
                Debug.Log("Hp is 0 or less and dead is false");
                dead = true;
                Died?.Invoke();
            }
            UpdateHealthUI();
        }
    }

    public UnityEvent<int> Healed;
    public UnityEvent<int> Damaged;
    public UnityEvent Died;

    void Start()
    {
        Hp = _maxHp;
    }

    private void Awake()
    {
        Hp = _maxHp;
        animator = GetComponent<Animator>();

        // Ensure the AnimatorController is assigned
        if (animator.runtimeAnimatorController == null && animatorController != null)
        {
            animator.runtimeAnimatorController = animatorController;
        }

        Died.AddListener(OnDeath); // Add listener for the death event
        Debug.Log("Awake: Animator and Died listener initialized");
    }

    public void Damage(int amount)
    {
        if (dead) return;

        Debug.Log($"Damage received: {amount}");
        Hp -= amount;
    }

    private void OnDeath()
    {
        Debug.Log("OnDeath: Setting Explode trigger");
        // Set the trigger for the explosion animation
        animator.SetTrigger("Explode");
        // Start coroutine to wait for the animation to finish before destroying the object
        StartCoroutine(WaitForExplosionAnimation());
    }

    private IEnumerator WaitForExplosionAnimation()
    {
        Debug.Log("Waiting for explosion animation to start...");
        yield return new WaitForEndOfFrame();

        Debug.Log("Current State: " + animator.GetCurrentAnimatorStateInfo(0).IsName("Explosion"));

        while (animator.GetCurrentAnimatorStateInfo(0).IsName("Explosion") &&
               animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            Debug.Log("Explosion animation progress: " + animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
            yield return null;
        }

        Debug.Log("Explosion animation finished. Destroying game object.");
        // Destroy the turret after the explosion animation has finished
        Destroy(gameObject);
    }

    public void Heal(int amount) => Hp += amount;

    public void HealFull() => Hp = _maxHp;

    public void Kill() => Hp = 0;

    public void Adjust(int value) => Hp = value;

    private void UpdateHealthUI()
    {
        // Update health UI if necessary
    }
}
