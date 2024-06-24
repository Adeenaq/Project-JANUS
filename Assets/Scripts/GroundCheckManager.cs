using UnityEngine;

public class GroundCheckManager : MonoBehaviour
{
    public Transform[] groundChecks; // Array to hold ground check transforms
    public LayerMask groundLayer; // Layer mask to specify what is considered ground

    private bool isAnyGrounded; // Variable to track if any player is on the ground

    void Update()
    {
        CheckGrounded();
    }

    private void CheckGrounded()
    {
        isAnyGrounded = false;

        foreach (Transform groundCheck in groundChecks)
        {
            if (Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer))
            {
                isAnyGrounded = true;
                break;
            }
        }
    }

    public bool IsAnyGrounded()
    {
        return isAnyGrounded;
    }
}
