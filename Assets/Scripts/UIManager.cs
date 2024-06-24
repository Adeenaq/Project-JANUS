using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI healthText; // Assign this in the Inspector

    public void UpdateHealth(int currentHealth)
    {
        healthText.text = "Health: " + currentHealth;
    }
}
