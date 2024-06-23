using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSync : MonoBehaviour
{
    public Transform playerTop;
    public Transform playerBottom;

    private Vector3 offset;

    void Start()
    {
        // Calculate the initial offset between the two players
        offset = playerBottom.position - playerTop.position;
    }

    void FixedUpdate()
    {
        // Ensure the players maintain the initial offset
        playerBottom.position = playerTop.position + offset;

        // Optionally, you can also synchronize rotations if needed
        // playerBottom.rotation = playerTop.rotation;
    }
}
