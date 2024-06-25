using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRecoil : MonoBehaviour
{
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component missing!");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            rb.AddForce(Vector2.left * 1000f, ForceMode2D.Impulse);
            Debug.Log("Force applied: 1000");
        }
    }
}
