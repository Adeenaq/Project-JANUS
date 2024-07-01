using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health_Enemy1 : MonoBehaviour
{
    [SerializeField] private int _maxhp = 1000;
    [SerializeField] private int _hp;

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
    }

    private void Awake()
    {
        Hp = _maxhp;
        UpdateHealthUI();
    }

    public void Damage(int amount)
    {
        Hp -= amount;

        if (Hp <= 0)
        {
            Destroy(gameObject);
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
