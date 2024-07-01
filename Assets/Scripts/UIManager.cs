using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText; // Assign this in the Inspector

    public void UpdateHealth(int currentHealth)
    {
        healthText.text = "Health: " + currentHealth;
    }
}
