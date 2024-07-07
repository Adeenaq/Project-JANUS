using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class TurretWeapon : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab; // Prefab of the bullet
    [SerializeField] private Transform firepoint; // Firepoint
    [SerializeField] private float fireForce = 10f;
    [SerializeField] private float fireInterval = 2f; // Time interval between shots
    private TurretScript turretScript;
    [SerializeField] private AudioClip fireClip;
    [SerializeField][Range(0f, 1f)] private float firevolume;
    private AudioSource audioSource;

    private ObjectPool<Bullet_Turret> bulletPool;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component is missing on TurretWeapon.");
        }

        // Initialize object pool for the bullet prefab
        bulletPool = new ObjectPool<Bullet_Turret>(
            createFunc: CreateBullet,
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

    private Bullet_Turret CreateBullet()
    {
        GameObject bulletObject = Instantiate(bulletPrefab);
        Bullet_Turret bullet = bulletObject.GetComponent<Bullet_Turret>();
        if (bullet == null)
        {
            Debug.LogError("Bullet_Turret component not found in the bullet prefab.");
            return null;
        }
        bullet.SetPool(bulletPool);
        return bullet;
    }

    private void Start()
    {
        turretScript = GetComponentInParent<TurretScript>();
        if (turretScript == null)
        {
            Debug.LogError("TurretScript not found in parent.");
            return;
        }

        StartCoroutine(FireRoutine());
    }

    private IEnumerator FireRoutine()
    {
        while (true)
        {
            if (turretScript.Detected)
            {
                Fire();
            }
            yield return new WaitForSeconds(fireInterval);
        }
    }

    public void Fire()
    {
        if (firepoint == null)
        {
            Debug.LogError("Firepoint is not assigned in the TurretWeapon script.");
            return;
        }

        Bullet_Turret bullet = bulletPool.Get();
        if (bullet != null)
        {
            bullet.transform.position = firepoint.position;
            bullet.transform.rotation = firepoint.rotation;

            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.zero; // Reset velocity
                rb.AddForce(firepoint.right * fireForce, ForceMode2D.Impulse);
                Debug.Log($"Bullet fired from {firepoint.name} with force: {fireForce}");
            }

            PlaySound(fireClip, firevolume); // Play the fire sound
        }
    }

    private void PlaySound(AudioClip clip, float volume)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }
}
