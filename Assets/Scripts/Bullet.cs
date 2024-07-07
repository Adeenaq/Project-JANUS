using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{
    private int baseDamageAmount = 100;
    private int _damageAmount;
    private PowerUp powerUp;
    private IObjectPool<Bullet> _bulletPool;
    private bool _isReleased = false; // Flag to track if the bullet has been released

    public bool IsReleased
    {
        get { return _isReleased; }
        set { _isReleased = value; }
    }

    private void Start()
    {
        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            powerUp = player.GetComponent<PowerUp>();
            if (powerUp == null)
            {
                Debug.LogError("PowerUp component not found on the player.");
            }
        }
        else
        {
            Debug.LogError("Player object not found in the scene.");
        }
    }

    public void SetPool(IObjectPool<Bullet> pool)
    {
        _bulletPool = pool;
    }

    private void UpdateDamageAmount()
    {
        if (powerUp != null)
        {
            _damageAmount = Mathf.RoundToInt(baseDamageAmount * powerUp.GetBulletDamageMultiplier());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsReleased)
        {
            Debug.LogWarning("Attempted to process collision on a bullet already released.");
            return; // Prevent multiple releases
        }

        Debug.Log($"Bullet collided with: {collision.gameObject.name}");

        // Update damage amount based on current power-up state
        UpdateDamageAmount();

        var expectKnight = collision.gameObject.GetComponent<Health_Knight>();
        var expectTurret = collision.gameObject.GetComponent<Health_Turret>();
        var expectWasp = collision.gameObject.GetComponent<Health_Wasp>();
        var expectBoss = collision.gameObject.GetComponent<Health_Boss>();
        if (expectKnight != null)
        {
            Debug.Log("Hit knight");
            expectKnight.Damage(_damageAmount);
        }
        else if (expectTurret != null)
        {
            Debug.Log("Hit turret");
            expectTurret.Damage(_damageAmount);
        }
        else if (expectWasp != null)
        {
            Debug.Log("Hit wasp");
            expectWasp.Damage(_damageAmount);
        }
        else if (expectBoss != null)
        {
            Debug.Log("Hit boss");
            expectBoss.Damage(_damageAmount);
        }
        else
        {
            var playerHealth = collision.gameObject.GetComponent<Health_Player>();
            if (playerHealth != null)
            {
                playerHealth.Damage(_damageAmount);
            }
        }

        ReturnToPool();  // Return the bullet to the pool immediately after collision
    }

    private void OnBecameInvisible()
    {
        if (!IsReleased) // Only return to pool if not already released
        {
            Debug.Log("Bullet became invisible and will return to pool");
            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        if (IsReleased)
        {
            Debug.LogWarning("Attempted to return a bullet to the pool that has already been released.");
            return; // Prevent multiple releases
        }

        Debug.Log("Returning bullet to pool");
        IsReleased = true; // Mark bullet as released

        if (_bulletPool != null)
        {
            _bulletPool.Release(this);
            Debug.Log("Bullet released to pool");
        }
        else
        {
            Destroy(gameObject);
            Debug.Log("Bullet destroyed as no pool was found");
        }
    }

    private void OnEnable()
    {
        Debug.Log("Bullet enabled");
        IsReleased = false; // Reset release flag
        // Reset bullet state if needed
        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            Debug.Log("Bullet velocity reset to zero");
        }
    }

    private void OnDisable()
    {
        Debug.Log("Bullet disabled");
    }
}
