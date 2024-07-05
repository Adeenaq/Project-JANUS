using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health_Wasp : MonoBehaviour
{
    [SerializeField] private int _maxhp = 1000;
    [SerializeField] private int _hp;
    private bool takingDamage = false;
    private bool dead = false;
    Animator animator;

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
        if (takingDamage == true)
        {
            animator.SetBool("DamageTaken", false);
            takingDamage = false;
        }
    }

    private void Awake()
    {
        Hp = _maxhp;
        animator = GetComponent<Animator>();
    }

    public void Damage(int amount)
    {
        Hp -= amount;
        takingDamage = true;

        if (Hp <= 0)
        {
            dead = true;
            StartCoroutine(waiter(2));
            animator.Play("wasp_death", 0, 0f);
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
    }
}
