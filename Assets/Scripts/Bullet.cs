using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        ObjectPool.Instance.ReturnToPool(gameObject);
        Debug.Log("Bullet collided and returned to pool");
    }
}
