using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarDeath_AnimationEvents : MonoBehaviour
{
    void destroyParent()
    {
        Destroy(this.transform.parent.gameObject);
    }
}
