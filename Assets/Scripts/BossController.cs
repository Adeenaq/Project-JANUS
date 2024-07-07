using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    private List<Transform> firstLevelChildren;
    private float timer = 0f;
    private float attackCooldown = 5f;
    private Animator[] animators;

    // Start is called before the first frame update
    void Start()
    {
        firstLevelChildren = new List<Transform>();
        animators = GetComponentsInChildren<Animator>();

        // Store all the first-level child GameObjects
        foreach (Transform child in transform)
        {
            if (child.name != "AttackAnim")
            {
                firstLevelChildren.Add(child);
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timer += Time.deltaTime;

        if (timer >= attackCooldown)
        {
            foreach (var a in animators)
            {
                PlayAnimationIfExists(a, "boss_attack");
            }
            timer = 0f;
        }
    }

    public void Attack()
    {
        // Randomly choose one first-level child
        int randomIndex = Random.Range(0, firstLevelChildren.Count);
        Transform randomChild = firstLevelChildren[randomIndex];

        // Do something with the randomly chosen child
        // For example, print its name
        Debug.Log("Randomly chosen child: " + randomChild.name);
        randomChild.gameObject.SetActive(true);

        // Change attack cooldown based on type of attack chosen
        // Change x position of child depending on last saved player position
    }

    void PlayAnimationIfExists(Animator animator, string animationName)
    {
        if (animator.HasState(0, Animator.StringToHash(animationName)))
        {
            animator.Play(animationName, 0, 0f);
        }
    }

    IEnumerator waiter(float time)
    {
        yield return new WaitForSeconds(time);
    }
}
