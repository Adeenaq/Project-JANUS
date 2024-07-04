using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretScript : MonoBehaviour
{
    [SerializeField] private float Range;
    private bool detected = false; // Changed to private
    private Vector2 Direction;
    [SerializeField] private float RotationSpeed = 5f; // Speed at which the barrel rotates
    private Transform Target;
    private Transform Barrel;
    private LayerMask TargetLayer; // Layer mask for the target

    public bool Detected
    {
        get { return detected; }
        private set { detected = value; }
    }

    void Start()
    {
        // Automatically find the player by tag
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            Target = player.transform;
            // Set the target layer to the player's layer
            TargetLayer = 1 << player.layer;
        }
        else
        {
            Debug.LogError("Player with tag 'Player' not found in the scene.");
        }

        // Find the Barrel child object
        Barrel = transform.Find("Barrel");
        if (Barrel == null)
        {
            Debug.LogError("Barrel object not found as a child of the turret.");
        }
    }

    void Update()
    {
        if (Target == null || Barrel == null)
        {
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
        angle += 180; // Adjust the angle by 180 degrees to correct the direction
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
        Debug.Log($"Current barrel rotation: {Barrel.rotation.eulerAngles.z}, Target angle: {angle}");
        Barrel.rotation = Quaternion.Lerp(Barrel.rotation, targetRotation, Time.deltaTime * RotationSpeed);
        Debug.Log($"Rotating barrel to angle: {angle}");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Range);
    }
}
