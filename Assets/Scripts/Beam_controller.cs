using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam_controller : MonoBehaviour
{
    private int damageAmount = 25;
    Animator animator;
    
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        gameObject.SetActive(false); 
    }

    private void OnEnable()
    {
        animator.Play("beam", 0, 0f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Health_Player player_health = collision.gameObject.GetComponent<Health_Player>();
        if (player_health != null)
        {
            player_health.Damage(damageAmount);
            StartCoroutine(waiter(2));
        }
    }

    IEnumerator waiter(int time)
    {
        yield return new WaitForSeconds(time);
    }
}
