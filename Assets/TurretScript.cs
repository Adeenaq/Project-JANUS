using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretScript : MonoBehaviour
{
    public float Range;
    public Transform Target;
    private bool Detected = false;
    private Vector2 Direction;
    public Transform Barrel;
    public float RotationSpeed = 5f; // Speed at which the barrel rotates
    public LayerMask TargetLayer; // Layer mask for the target

    // Update is called once per frame
    void Update()
    {
        if (Target == null)
        {
            Debug.LogWarning("Target is not assigned.");
            return;
        }

        Vector2 targetPos = Target.position;
        Direction = targetPos - (Vector2)transform.position;

        // Check if the target is within range
        if (Direction.magnitude <= Range)
        {
            RaycastHit2D rayInfo = Physics2D.Raycast(transform.position, Direction, Range, TargetLayer);
            Debug.DrawRay(transform.position, Direction, Color.green); // Draw the ray in the scene view for debugging

            if (rayInfo)
            {
                Debug.Log($"Raycast hit: {rayInfo.collider.gameObject.name}");
                if (rayInfo.collider.gameObject == Target.gameObject)
                {
                    if (!Detected)
                    {
                        Detected = true;
                        Debug.Log("Target detected within range.");
                    }
                }
                else
                {
                    if (Detected)
                    {
                        Detected = false;
                        Debug.Log("Target lost.");
                    }
                }
            }
            else
            {
                if (Detected)
                {
                    Detected = false;
                    Debug.Log("Raycast did not hit any target.");
                }
            }
        }
        else
        {
            if (Detected)
            {
                Detected = false;
                Debug.Log("Target out of range.");
            }
        }

        if (Detected)
        {
            RotateBarrel(Direction);
        }
    }

    void RotateBarrel(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle += 180;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
        Debug.Log($"Current barrel rotation: {Barrel.rotation.eulerAngles.z}, Target angle: {angle}");
        Barrel.rotation = Quaternion.Lerp(Barrel.rotation, targetRotation, Time.deltaTime * RotationSpeed);
        Debug.Log($"Rotating barrel to angle: {-1 * angle}");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Range);
    }
}
