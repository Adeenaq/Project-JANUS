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
    }

    public void Damage(int amount)
    {
        foreach (Animator a in animators)
        {
            a.SetBool("DamageTaken", true);
        }
        takingDamage = true;
        Hp -= amount;

        if (Hp <= 0)
        {
            dead = true;
            foreach (Animator a in animators)
            {
                a.SetBool("Dead", true);
            }
            Destroy(GetComponent<PlayerController>());
            // some gameover UI function to be added here
        }
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
}
