using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret_AnimationEvent : MonoBehaviour
{
    private void DestroyTurret()
    {
        Transform parent = gameObject.transform.parent;
        gameObject.transform.parent = null;
        Destroy(parent.gameObject);
    }
}
