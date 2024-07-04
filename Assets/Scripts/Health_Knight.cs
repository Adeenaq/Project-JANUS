using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health_Knight : MonoBehaviour
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
        animator.Play("knight_damage", 0, 0f);
        Hp -= amount;

        if (Hp <= 0)
        {
            dead = true;
            StartCoroutine(waiter(2));
            animator.Play("knight_dead", 0, 0f);

            Destroy(GetComponent<KnightController>());

            Rigidbody2D myrb = GetComponent<Rigidbody2D>();
            Vector2 vel = myrb.velocity;
            vel.x = 0f;

            StartCoroutine(waiter(2));

            myrb.gravityScale = 0f;
            myrb.velocity = vel;
            Destroy(GetComponent<CapsuleCollider2D>());
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
