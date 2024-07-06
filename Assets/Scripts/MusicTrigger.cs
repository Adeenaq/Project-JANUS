using UnityEngine;

public class MusicTrigger : MonoBehaviour
{
    public enum MusicType { Starting, MainGameplay, BossBattle }
    [SerializeField] private MusicType musicTypeForward;
    [SerializeField] private MusicType musicTypeBackward;

    private Vector3 previousPosition;

    private void Start()
    {
        previousPosition = Vector3.zero;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Vector3 currentPosition = other.transform.position;
            Vector3 direction = currentPosition - previousPosition;
            previousPosition = currentPosition;

            if (AudioManager.Instance == null)
            {
                Debug.LogError("AudioManager instance is missing.");
                return;
            }

            if (direction.x > 0) // Moving to the right
            {
                SwitchMusic(musicTypeForward);
            }
            else if (direction.x < 0) // Moving to the left
            {
                SwitchMusic(musicTypeBackward);
            }
        }
    }

    private void SwitchMusic(MusicType musicType)
    {
        switch (musicType)
        {
            case MusicType.Starting:
                AudioManager.Instance.PlayMusic(AudioManager.Instance.StartingSectionMusic);
                break;
            case MusicType.MainGameplay:
                AudioManager.Instance.PlayMusic(AudioManager.Instance.MainGameplayMusic);
                break;
            case MusicType.BossBattle:
                AudioManager.Instance.PlayMusic(AudioManager.Instance.BossBattleMusic);
                break;
            default:
                Debug.LogError("Unknown MusicType encountered.");
                break;
        }
    }
}
