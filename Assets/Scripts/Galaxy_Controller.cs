using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Galaxy_Controller : MonoBehaviour
{
    Animator animator;
    
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        animator.Play("galaxy", 0, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
