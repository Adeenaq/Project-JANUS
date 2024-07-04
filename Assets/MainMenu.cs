using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject powerUpSelectionPanel;
    public GameObject controlsPanel; // Add reference to the controls panel

    [System.Serializable]
    public struct PowerUpPair
    {
        public string powerUp1;
        public string powerUp2;
    }

    public static PowerUpPair selectedPowerUpPair;
    private Button selectedButton;
    public Color selectedColor = Color.green;
    public Color defaultColor = Color.white;

    public void PlayGame()
    {
        mainMenuPanel.SetActive(false);
        powerUpSelectionPanel.SetActive(true);
    }

    public void ShowControls()
    {
        mainMenuPanel.SetActive(false);
        controlsPanel.SetActive(true);
    }

    public void HideControls()
    {
        controlsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void SelectPowerUpPair1(Button button)
    {
        SelectPowerUpPair(button, "Seeker", "Thriller");
    }

    public void SelectPowerUpPair2(Button button)
    {
        SelectPowerUpPair(button, "Regeneration", "Immunity");
    }

    public void SelectPowerUpPair3(Button button)
    {
        SelectPowerUpPair(button, "Double Jump", "Flash");
    }

    private void SelectPowerUpPair(Button button, string powerUp1, string powerUp2)
    {
        selectedPowerUpPair = new PowerUpPair { powerUp1 = powerUp1, powerUp2 = powerUp2 };
        Debug.Log("Selected PowerUp Pair: " + selectedPowerUpPair.powerUp1 + " and " + selectedPowerUpPair.powerUp2);

        if (selectedButton != null)
        {
            selectedButton.GetComponent<Image>().color = defaultColor;
        }

        selectedButton = button;
        selectedButton.GetComponent<Image>().color = selectedColor;
    }

    public void StartGame()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
