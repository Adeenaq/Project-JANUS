// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class Bullet : MonoBehaviour
// {
//     private int _damageAmount = 100;

//     private void OnCollisionEnter2D(Collision2D collision)
//     {
//         if (collision.gameObject.transform.parent.name == "Enemies")
//         {
//             collision.gameObject.TryGetComponent<Health>(out Health enemyHealth);
//             Debug.Log("Hit enemy");
//             enemyHealth.Damage(_damageAmount);
//         }

//         Destroy(gameObject);
//         Debug.Log("Bullet collided and destroyed");
        
//     }

//     private void OnBecameInvisible()
//     {
//         Destroy(gameObject);
//         Debug.Log("Bullet exited screen and destroyed");
//     }
// }

using UnityEngine;

public class Bullet : MonoBehaviour
{
    private int _damageAmount = 100;

    private void OnCollisionEnter2D(Collision2D collision)
{
    if (collision.gameObject.transform.parent != null && collision.gameObject.transform.parent.name == "Enemies")
    {
        if (collision.gameObject.TryGetComponent<Health>(out Health enemyHealth))
        {
            Debug.Log("Hit enemy");
            enemyHealth.Damage(_damageAmount);
            
        }
        else
        {
            Debug.Log("Health component not found on the enemy");
        }
    }

    if (BulletPool.Instance != null)
    {
        BulletPool.Instance.ReturnToPool(this);
        Debug.Log("Bullet collided and returned to pool");
    }
    else
    {
        Debug.LogError("BulletPool.Instance is null. Ensure BulletPool is initialized before using it.");
    }
}

private void OnBecameInvisible()
{
    if (BulletPool.Instance != null)
    {
        BulletPool.Instance.ReturnToPool(this);
        Debug.Log("Bullet exited screen and returned to pool");
    }
    else
    {
        Debug.LogError("BulletPool.Instance is null. Ensure BulletPool is initialized before using it.");
    }
}
}