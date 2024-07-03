using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.Rendering.DebugUI;

public class Thrill_Player : MonoBehaviour
{
    [SerializeField] private int _thrill;

    public int Thrill
    {
        get => _thrill;
        private set
        {
            _thrill = value;
            if (_thrill < 0)
            {
                _thrill = 0;
            }
            //UpdateHealthUI();
        }
    }

    void Start()
    {
        Thrill = 0;
    }

    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            IncreaseThrill(50);
            Debug.Log(Thrill);
        }
    }

    private void Awake()
    {
        //UpdateHealthUI();
    }

    public void DecreaseThrill(int amount)
    {
        Thrill -= amount;
        if (Thrill < 0)
        {
            Thrill = 0;
        }
    }

    public void IncreaseThrill(int amount)
    {
        Thrill += amount;
    }

    //private void UpdateHealthUI()
    //{
    //    if (uiManager != null)
    //    {
    //        uiManager.UpdateHealth(_hp);
    //    }
    //}
}
