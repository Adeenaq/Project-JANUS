using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponKnight : MonoBehaviour
{
    [SerializeField] private GameObject[] bulletPrefab; // Prefabs of the bullet
    [SerializeField] private Transform[] firepoints; // Array to hold multiple firepoints
    private float fireforce = 10f;

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
            GameObject bullet = Instantiate(bulletPrefab[i], firepoint.position, firepoint.rotation);
            if (bullet.GetComponent<Rigidbody2D>() == null)
            {
                Debug.LogError("Rigidbody2D component not found in the bullet prefab.");
                return;
            }
            bullet.GetComponent<Rigidbody2D>().velocity = Vector2.zero; // Reset velocity
            bullet.GetComponent<Rigidbody2D>().AddForce(firepoint.right * fireforce, ForceMode2D.Impulse);
            Debug.Log("Bullet fired from " + firepoint.name + " with force: " + fireforce);
            i++;
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
