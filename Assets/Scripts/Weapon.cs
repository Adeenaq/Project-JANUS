using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Weapon : MonoBehaviour
{
    [SerializeField] private GameObject[] bulletPrefab; // Prefabs of the bullet
    [SerializeField] private Transform[] firepoints; // Array to hold multiple firepoints
    [SerializeField] private float fireforce = 20f;
    [SerializeField] private AudioClip fireClip;
    [SerializeField][Range(0f, 1f)] private float firevolume;
    private AudioSource audioSource;

    private ObjectPool<Bullet>[] bulletPools;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        // Initialize object pools for each bullet prefab
        bulletPools = new ObjectPool<Bullet>[bulletPrefab.Length];
        for (int i = 0; i < bulletPrefab.Length; i++)
        {
            int index = i; // Capture index for the closure
            bulletPools[i] = new ObjectPool<Bullet>(
                createFunc: () => CreateBullet(index),
                actionOnGet: bullet =>
                {
                    bullet.gameObject.SetActive(true);
                    bullet.GetComponent<Rigidbody2D>().velocity = Vector2.zero; // Reset velocity
                    bullet.IsReleased = false; // Ensure the bullet is not marked as released
                    Debug.Log($"Bullet {bullet.gameObject.name} acquired from pool.");
                },
                actionOnRelease: bullet =>
                {
                    bullet.gameObject.SetActive(false);
                    Debug.Log($"Bullet {bullet.gameObject.name} returned to pool.");
                },
                actionOnDestroy: bullet => Destroy(bullet.gameObject),
                defaultCapacity: 10,
                maxSize: 20
            );
        }
    }

    private Bullet CreateBullet(int index)
    {
        GameObject bulletObject = Instantiate(bulletPrefab[index]);
        Bullet bullet = bulletObject.GetComponent<Bullet>();
        if (bullet == null)
        {
            Debug.LogError("Bullet component not found in the bullet prefab.");
            return null;
        }
        bullet.SetPool(bulletPools[index]);
        return bullet;
    }

    public void Fire()
    {
        if (firepoints == null || firepoints.Length == 0)
        {
            Debug.LogError("Firepoints are not assigned in the Weapon script.");
            return;
        }

        int i = 0;
        foreach (var firepoint in firepoints)
        {
            if (i >= bulletPools.Length) break;

            Bullet bullet = bulletPools[i].Get();
            if (bullet != null)
            {
                bullet.transform.position = firepoint.position;
                bullet.transform.rotation = firepoint.rotation;

                Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.velocity = Vector2.zero; // Reset velocity
                    rb.AddForce(firepoint.right * fireforce, ForceMode2D.Impulse);
                    Debug.Log($"Bullet fired from {firepoint.name} with force: {fireforce}");
                }
            }
            i++;
        }
        PlaySound(fireClip, firevolume);
    }

    private void PlaySound(AudioClip clip, float volume)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }

    public void SetFirepointDirection(bool isFacingRight)
    {
        foreach (var firepoint in firepoints)
        {
            firepoint.localRotation = isFacingRight ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 0, 180);
            Debug.Log(firepoint.name + " direction set to: " + (isFacingRight ? "Right" : "Left"));
        }
    }
}
