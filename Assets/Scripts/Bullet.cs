using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private int baseDamageAmount = 100;
    private int _damageAmount;
    private PowerUp powerUp;

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

    private void UpdateDamageAmount()
    {
        if (powerUp != null)
        {
            _damageAmount = Mathf.RoundToInt(baseDamageAmount * powerUp.GetBulletDamageMultiplier());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"Bullet collided with: {collision.gameObject.name}");

        // Update damage amount based on current power-up state
        UpdateDamageAmount();

        // Check if the collided object has the Health_Enemy1 component
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
            // Check if the collided object has the Health_Player component
            var playerHealth = collision.gameObject.GetComponent<Health_Player>();
            if (playerHealth != null)
            {
                playerHealth.Damage(_damageAmount);
            }
        }

        Destroy(gameObject);
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
        Debug.Log("Bullet exited screen and destroyed");
    }
}


//using UnityEngine;

//public class Bullet : MonoBehaviour
//{
//    private int _damageAmount = 100;
//    public int DamageAmount
//    {
//        get { return _damageAmount; }
//        set { _damageAmount = value; }
//    }

//    private void OnCollisionEnter2D(Collision2D collision)
//{
//    if (collision.gameObject.transform.parent != null && (collision.gameObject.transform.parent.name == "Enemies") || (collision.gameObject.transform.parent.name == "Player"))
//    {
//        if (collision.gameObject.TryGetComponent<Health_Enemy1>(out Health_Enemy1 enemyHealth))
//        {
//            Debug.Log("Hit enemy");
//            enemyHealth.Damage(_damageAmount);

//        }
//        else if (collision.gameObject.GetComponentInParent<Health_Player>())
//        {
//            Debug.Log("Hit player");
//            collision.gameObject.GetComponentInParent<Health_Player>().Damage(_damageAmount);

//        }
//        else
//        {
//            Debug.Log("Health component not found on the enemy");
//        }
//    }

//    if (BulletPool.Instance != null)
//    {
//        BulletPool.Instance.ReturnToPool(this);
//        Debug.Log("Bullet collided and returned to pool");
//    }
//    else
//    {
//        Debug.LogError("BulletPool.Instance is null. Ensure BulletPool is initialized before using it.");
//    }
//}

//private void OnBecameInvisible()
//{
//    if (BulletPool.Instance != null)
//    {
//        BulletPool.Instance.ReturnToPool(this);
//        Debug.Log("Bullet exited screen and returned to pool");
//    }
//    else
//    {
//        Debug.LogError("BulletPool.Instance is null. Ensure BulletPool is initialized before using it.");
//    }
//}
//}