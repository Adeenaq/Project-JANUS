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

    [SerializeField] private Image thresholdXOutline2; // Outline for the second bar at thresholdX
    [SerializeField] private Image thresholdYOutline2; // Outline for the second bar at thresholdY

    [SerializeField] private PowerUp powerups;
    [SerializeField] private int thresholdX;  // Threshold for the first power-up
    [SerializeField] private int thresholdY; // Threshold for the second power-up

    [SerializeField] private AudioClip thresholdXSound; // Sound to play when thresholdX is reached
    [SerializeField] private AudioClip thresholdYSound; // Sound to play when thresholdY is reached
    private AudioSource audioSource;

    private bool thresholdXReached = false;
    private bool thresholdYReached = false;

    private void Awake()
    {
        thresholdX = powerups.GetThreshHoldI();
        thresholdY = powerups.GetThreshHoldO();
        audioSource = GetComponent<AudioSource>();
    }

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        healthBar.fillAmount = currentHealth / maxHealth;
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

        // Check and play sounds when thresholds are reached
        if (currentThrill >= thresholdX && !thresholdXReached)
        {
            audioSource.PlayOneShot(thresholdXSound);
            thresholdXReached = true;
        }

        if (currentThrill >= thresholdY && !thresholdYReached)
        {
            audioSource.PlayOneShot(thresholdYSound);
            thresholdYReached = true;
        }

        // Reset the thresholds if the current thrill drops below them
        if (currentThrill < thresholdX)
        {
            thresholdXReached = false;
        }

        if (currentThrill < thresholdY)
        {
            thresholdYReached = false;
        }

        // Update the outlines for thrillBar1
        if (currentThrill >= thresholdY)
        {
            thresholdXOutline2.gameObject.SetActive(true);
            thresholdYOutline2.gameObject.SetActive(true);
        }
        else if (currentThrill < thresholdY && currentThrill >= thresholdX)
        {
            thresholdXOutline2.gameObject.SetActive(true);
            thresholdYOutline2.gameObject.SetActive(false);
        }
        else
        {
            thresholdXOutline2.gameObject.SetActive(false);
            thresholdYOutline2.gameObject.SetActive(false);
        }
    }
}
