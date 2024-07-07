using UnityEngine;

public class Thrill_Player : MonoBehaviour
{
    [SerializeField] private int _thrill;
    [SerializeField] private UIManager uiManager;

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
            UpdateThrillUI();
        }
    }

    void Start()
    {
        Thrill = 0;
    }

    private void UpdateThrillUI()
    {
        if (uiManager != null)
        {
            uiManager.UpdateThrill(_thrill);
        }
    }

    public void IncreaseThrill(int amount)
    {
        Thrill += amount;
    }

    public void DecreaseThrill(int amount)
    {
        Thrill -= amount;
    }
}
