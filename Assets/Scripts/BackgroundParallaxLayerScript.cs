using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [SerializeField] private float parallaxFactor = 1.0f;
    [SerializeField] private float maxDownwardOffset = 1.5f;
    private Vector3 previousCameraPosition;
    private float originalPositionY;

    void Start()
    {
        originalPositionY = transform.position.y;
        previousCameraPosition = Camera.main.transform.position;
    }

    void Update()
    {
        Vector3 deltaMovement = Camera.main.transform.position - previousCameraPosition;
        float parallaxFactorY = (transform.position.y <= originalPositionY-maxDownwardOffset) ? 1.0f : parallaxFactor;
        transform.position += new Vector3(deltaMovement.x * parallaxFactor, deltaMovement.y * parallaxFactorY, 0);
        previousCameraPosition = Camera.main.transform.position;
    }

    public void setParallaxFactor(float factor)
    {
        parallaxFactor = factor;
    }
}
