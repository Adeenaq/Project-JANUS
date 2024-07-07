using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Bullet_Knight : MonoBehaviour
{
    private int _damageAmount = 100;
    private IObjectPool<Bullet_Knight> _bulletPool;
    private bool _isReleased = false;

    public bool IsReleased
    {
        get { return _isReleased; }
        set { _isReleased = value; }
    }

    public void SetPool(IObjectPool<Bullet_Knight> pool)
    {
        _bulletPool = pool;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsReleased)
        {
            Debug.LogWarning("Attempted to process collision on a bullet already released.");
            return;
        }

        Debug.Log($"Bullet collided with: {collision.gameObject.name}");

        // Check if the collided object has the Health_Enemy1 component
        var enemyHealth = collision.gameObject.GetComponent<Health_Knight>();
        if (enemyHealth != null)
        {
            Debug.Log("Hit enemy");
            enemyHealth.Damage(_damageAmount);
        }
        else
        {
            // Check if the collided object has the Health_Player component
            var playerHealth = collision.gameObject.GetComponent<Health_Player>();
            if (playerHealth != null)
            {
                var circleCheck = collision.gameObject.GetComponent<CircleCollider2D>();
                if (circleCheck == null)
                {
                    Debug.Log("Hit player");
                    playerHealth.Damage(_damageAmount);
                }
            }
        }

        ReturnToPool();
    }

    private void OnBecameInvisible()
    {
        if (!IsReleased)
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
            return;
        }

        Debug.Log("Returning bullet to pool");
        IsReleased = true;

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
        IsReleased = false;
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
