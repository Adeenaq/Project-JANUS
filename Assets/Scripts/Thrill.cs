using UnityEngine;

public class Thrill_Player : MonoBehaviour
{
    [SerializeField] private int _thrill;
    [SerializeField] private int maxThrill = 100; // Set the maximum thrill value
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
            if (_thrill > maxThrill)
            {
                _thrill = maxThrill;
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
            uiManager.UpdateThrill(_thrill, maxThrill);
        }
    }

    public void IncreaseThrill(int amount)
    {
        Thrill += amount;
    }
}
