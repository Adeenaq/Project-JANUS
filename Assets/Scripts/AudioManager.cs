using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip _startingSectionMusic;
    [SerializeField] private AudioClip _mainGameplayMusic;
    [SerializeField] private AudioClip _bossBattleMusic;

    [SerializeField] private float fadeTime = 1.0f;
    [SerializeField] private float fadeDelay = 0.1f;
    [Range(0f, 1f)][SerializeField] private float startingSectionVolume = 1.0f;
    [Range(0f, 1f)][SerializeField] private float mainGameplayVolume = 1.0f;
    [Range(0f, 1f)][SerializeField] private float bossBattleVolume = 1.0f;

    public AudioClip StartingSectionMusic => _startingSectionMusic;
    public AudioClip MainGameplayMusic => _mainGameplayMusic;
    public AudioClip BossBattleMusic => _bossBattleMusic;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError("AudioSource component is missing. Please attach an AudioSource component to the AudioManager GameObject.");
            }
        }

        if (_startingSectionMusic == null || _mainGameplayMusic == null || _bossBattleMusic == null)
        {
            Debug.LogError("One or more AudioClip references are missing in the AudioManager.");
        }
    }

    private void Start()
    {
        // Start playing the initial music with a fade-in
        PlayInitialMusic(_startingSectionMusic, startingSectionVolume);
    }

    private void PlayInitialMusic(AudioClip clip, float volume)
    {
        if (clip == null)
        {
            Debug.LogError("Attempted to play a null AudioClip.");
            return;
        }

        StartCoroutine(FadeIn(clip, volume, fadeTime, fadeDelay));
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogError("Attempted to play a null AudioClip.");
            return;
        }

        float volume = 1.0f;
        if (clip == _startingSectionMusic)
        {
            volume = startingSectionVolume;
        }
        else if (clip == _mainGameplayMusic)
        {
            volume = mainGameplayVolume;
        }
        else if (clip == _bossBattleMusic)
        {
            volume = bossBattleVolume;
        }

        StartCoroutine(FadeOutAndIn(clip, volume, fadeTime, fadeDelay));
    }

    private IEnumerator FadeOutAndIn(AudioClip newClip, float targetVolume, float fadeDuration, float delay)
    {
        if (audioSource.isPlaying)
        {
            for (float vol = audioSource.volume; vol >= 0; vol -= Time.deltaTime / fadeDuration)
            {
                audioSource.volume = vol;
                yield return new WaitForSeconds(delay);
            }
        }

        audioSource.clip = newClip;
        audioSource.Play();

        for (float vol = 0; vol <= targetVolume; vol += Time.deltaTime / fadeDuration)
        {
            audioSource.volume = vol;
            yield return new WaitForSeconds(delay);
        }
    }

    private IEnumerator FadeIn(AudioClip newClip, float targetVolume, float fadeDuration, float delay)
    {
        audioSource.clip = newClip;
        audioSource.volume = 0;
        audioSource.Play();

        for (float vol = 0; vol <= targetVolume; vol += Time.deltaTime / fadeDuration)
        {
            audioSource.volume = vol;
            yield return new WaitForSeconds(delay);
        }
    }
}
