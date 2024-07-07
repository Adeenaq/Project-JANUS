using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Galaxy_AnimationEvent : MonoBehaviour
{
    void enableBeam()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
    }
}
