using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private int _damageAmount = 100;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Enemy")
        {
            collision.gameObject.TryGetComponent<Health>(out Health enemyHealth);
            Debug.Log("Hit enemy");
            enemyHealth.Damage(_damageAmount);
        }

        Destroy(gameObject);
        Debug.Log("Bullet collided and destroyed");
        
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
        Debug.Log("Bullet exited screen and destroyed");
    }
}
