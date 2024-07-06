using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Image healthBar;
    [SerializeField] private Image thrillBar;
    [SerializeField] private TextMeshProUGUI healthText; 
    [SerializeField] private TextMeshProUGUI thrillText; 

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        healthBar.fillAmount = currentHealth / maxHealth;
        //healthText.text = $"{currentHealth} / {maxHealth}"; 
    }

    public void UpdateThrill(float currentThrill, float maxThrill)
    {
        thrillBar.fillAmount = currentThrill / maxThrill;
        thrillText.text = $"{currentThrill}"; 
    }
}
