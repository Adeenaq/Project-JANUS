using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    public float parallaxFactor;

    private Vector3 previousCameraPosition;

    void Start()
    {
        previousCameraPosition = Camera.main.transform.position;
    }

    void Update()
    {
        Vector3 deltaMovement = Camera.main.transform.position - previousCameraPosition;
        transform.position += new Vector3(deltaMovement.x * parallaxFactor, deltaMovement.y * parallaxFactor, 0);
        previousCameraPosition = Camera.main.transform.position;
    }
}
