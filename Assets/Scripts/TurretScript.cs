using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretScript : MonoBehaviour
{
    [SerializeField] private float Range;
    private bool detected = false; // Changed to private
    private Vector2 Direction;
    [SerializeField] private float RotationSpeed = 5f; // Speed at which the barrel rotates
    private Transform Target;
    private Transform Barrel;
    private LayerMask TargetLayer; // Layer mask for the target
    [SerializeField] private AudioClip rotationsound;
    private AudioSource audioSource;

    [Range(0f, 1f)]
    [SerializeField] private float rotVolume;

    public bool Detected
    {
        get { return detected; }
        private set { detected = value; }
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component is missing on TurretScript.");
        }
    }

    void Start()
    {
        // Automatically find the player by tag
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            Target = player.transform;
            // Set the target layer to the player's layer
            TargetLayer = 1 << player.layer;
        }
        else
        {
            Debug.LogError("Player with tag 'Player' not found in the scene.");
        }

        // Find the Barrel child object
        Barrel = transform.Find("Barrel");
        if (Barrel == null)
        {
            Debug.LogError("Barrel object not found as a child of the turret.");
        }
    }

    void Update()
    {
        if (Target == null || Barrel == null)
        {
            return;
        }

        Vector2 targetPos = Target.position;
        Direction = targetPos - (Vector2)transform.position;

        // Check if the target is within range
        if (Direction.magnitude <= Range)
        {
            RaycastHit2D rayInfo = Physics2D.Raycast(transform.position, Direction, Range, TargetLayer);
            Debug.DrawRay(transform.position, Direction, Color.green); // Draw the ray in the scene view for debugging

            if (rayInfo)
            {
                if (rayInfo.collider.gameObject == Target.gameObject)
                {
                    if (!Detected)
                    {
                        Detected = true;
                        Debug.Log("Target detected within range.");
                    }
                }
                else
                {
                    if (Detected)
                    {
                        Detected = false;
                        Debug.Log("Target lost.");
                    }
                }
            }
            else
            {
                if (Detected)
                {
                    Detected = false;
                    Debug.Log("Raycast did not hit any target.");
                }
            }
        }
        else
        {
            if (Detected)
            {
                Detected = false;
                Debug.Log("Target out of range.");
            }
        }

        if (Detected)
        {
            RotateBarrel(Direction);
        }
    }

    void RotateBarrel(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle += 180; // Adjust the angle by 180 degrees to correct the direction
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
        Barrel.rotation = Quaternion.Lerp(Barrel.rotation, targetRotation, Time.deltaTime * RotationSpeed);
        PlayRotationSound();
    }

    private void PlayRotationSound()
    {
        if (!audioSource.isPlaying)
        {
            PlaySound(rotationsound, rotVolume);
        }
    }

    private void PlaySound(AudioClip audioClip, float volume)
    {
        if (audioClip != null)
        {
            audioSource.PlayOneShot(audioClip, volume);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Range);
    }
}
