using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack_AnimationEvent : MonoBehaviour
{
    BossController parentScript;

    private void Start()
    {
        parentScript = GetComponentInParent<BossController>();
    }

    void doAnAttack()
    {
        if (parentScript != null)
        {
            // Call the function from the parent's script
            parentScript.Attack();
        }
        else
        {
            Debug.LogError("ParentScript not found in parent GameObject");
        }
    }
}
