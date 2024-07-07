using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering;

public class Star_Bullet : MonoBehaviour
{
    private int _damageAmount = 100;
    private Animator[] animator;
    private float impactDamageRange = 4f;
    private AudioSource audioSource;
    [SerializeField] private AudioClip Explosionclip;
    [Range(0f, 1f)]
    [SerializeField] private float volume=1f;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponentsInChildren<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"Bullet collided with: {collision.gameObject.name}");

        // Check if the collided object has the Health_Player component
        var playerHealth = collision.gameObject.GetComponent<Health_Player>();
        if (playerHealth != null)
        {
            Debug.Log("Hit player");
            playerHealth.Damage(_damageAmount);
        }

        PlaySound(Explosionclip, volume);
        PlaySound(Explosionclip, volume);
        foreach (var a in animator)
        {
            PlayAnimationIfExists(a, "star_death");
        }
        
    }

    private void OnBecameInvisible()
    {
    }

    void PlayAnimationIfExists(Animator animator, string animationName)
    {
        if (animator.HasState(0, Animator.StringToHash(animationName)))
        {
            animator.Play(animationName, 0, 0f);
        }
    }
    
    void PlaySound(AudioClip clip, float vol)
    {
        audioSource.PlayOneShot(clip, vol);
    }
}
