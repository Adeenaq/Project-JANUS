using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Galaxy_Controller : MonoBehaviour
{
    Animator animator;
    [SerializeField] private AudioClip beamClip;
    [Range(0f,1f)][SerializeField] private float beamVol;
    private AudioSource audioSource;
    
    // Start is called before the first frame update
    void Start()
    {   
        audioSource= GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        gameObject.SetActive(false);
        
    }

    private void OnEnable()
    {
        audioSource.PlayOneShot(beamClip, beamVol);
        animator.Play("galaxy", 0, 0f);

    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
