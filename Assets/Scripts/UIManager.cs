using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Image healthBar;
    [SerializeField] private Image thrillBar1; // First thrill bar for the smaller power-up
    [SerializeField] private Image thrillBar2; // Second thrill bar for the larger power-up
    [SerializeField] private TextMeshProUGUI healthText; // Reference to the health text
    [SerializeField] private TextMeshProUGUI thrillText; // Reference to the thrill text

    private PowerUp powerups;
    private int thresholdX;  // Threshold for the first power-up
    private int thresholdY; // Threshold for the second power-up

    private void Awake()
    {
        powerups = GetComponent<PowerUp>();
        thresholdX=powerups.GetThreshHoldI();
        thresholdY=powerups.GetThreshHoldO();
    }
    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        healthBar.fillAmount = currentHealth / maxHealth;
        //healthText.text = $"{currentHealth} / {maxHealth}"; // Update health text
    }

    public void UpdateThrill(int currentThrill)
    {
        // Calculate the fill amount for the first bar
        float fillAmount1 = Mathf.Clamp01((float)currentThrill / thresholdX);

        // Calculate the fill amount for the second bar, considering the remaining thrill points after filling the first bar
        float fillAmount2 = 0;
        if (currentThrill > thresholdX)
        {
            fillAmount2 = Mathf.Clamp01((float)(currentThrill - thresholdX) / (thresholdY - thresholdX));
        }

        thrillBar1.fillAmount = fillAmount1;
        thrillBar2.fillAmount = fillAmount2;

        thrillText.text = $"{currentThrill}"; // Update thrill text
    }
}
