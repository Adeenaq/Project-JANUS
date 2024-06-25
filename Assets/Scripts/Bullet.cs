using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
        Debug.Log("Bullet collided and destroyed");
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
        Debug.Log("Bullet exited screen and destroyed");
    }
}
