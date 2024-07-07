using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadeController : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 1.0f;

    private static FadeController instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        if (fadeImage == null)
        {
            fadeImage = GetComponentInChildren<Image>();
        }
    }

    private void Start()
    {
        StartCoroutine(FadeIn());
    }

    public void FadeToScene(int sceneIndex)
    {
        StartCoroutine(FadeOut(sceneIndex));
    }

    private IEnumerator FadeIn()
    {
        fadeImage.color = new Color(0, 0, 0, 1);
        fadeImage.raycastTarget = true;
        float timer = 0;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeImage.color = new Color(0, 0, 0, 1 - (timer / fadeDuration));
            yield return null;
        }
        fadeImage.color = new Color(0, 0, 0, 0);
        fadeImage.raycastTarget = false; // Disable raycast target when fully transparent
    }

    private IEnumerator FadeOut(int sceneIndex)
    {
        fadeImage.color = new Color(0, 0, 0, 0);
        fadeImage.raycastTarget = true; // Enable raycast target when starting fade out
        float timer = 0;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeImage.color = new Color(0, 0, 0, timer / fadeDuration);
            yield return null;
        }
        fadeImage.color = new Color(0, 0, 0, 1);
        SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to the sceneLoaded event
        SceneManager.LoadScene(sceneIndex);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(FadeIn());
        SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe from the sceneLoaded event
    }
}
