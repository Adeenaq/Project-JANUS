using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam_AnimationEvent : MonoBehaviour
{
    private void disableBeamAttack()
    {
        this.gameObject.SetActive(false);
        this.transform.parent.gameObject.SetActive(false);
    }
}
