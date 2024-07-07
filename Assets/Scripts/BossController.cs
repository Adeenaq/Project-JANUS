using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossController : MonoBehaviour
{
    private List<Transform> firstLevelChildren;
    private float attacktimer = 0f;
    private float attackCooldown = 5f;
    private Animator[] animators;
    private float recordtimer = 0f;
    private float recordCooldown = 2f;
    [SerializeField] private float attackRangeSqr = 400f;

    private GameObject player;
    private Vector2 lastPlayerPosition;

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

        player = GameObject.Find("Players");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Check if player is within attack range using sqrMagnitude
        float sqrDistanceToPlayer = (transform.position - player.transform.position).sqrMagnitude;
        if (sqrDistanceToPlayer >= attackRangeSqr)
        {
            return;
        }

        attacktimer += Time.deltaTime;
        recordtimer += Time.deltaTime;

        if (attacktimer >= attackCooldown)
        {
            foreach (var a in animators)
            {
                PlayAnimationIfExists(a, "boss_attack");
            }
            attacktimer = 0f;
        }

        if (recordtimer >= recordCooldown)
        {
            lastPlayerPosition = player.transform.position;
            recordtimer = 0f;
        }
    }

    public void Attack()
    {
        // Randomly choose one first-level child
        int randomIndex = Random.Range(0, firstLevelChildren.Count);
        Transform randomChild = firstLevelChildren[randomIndex];

        // Do something with the randomly chosen child
        // For example, print its name
        //Debug.Log("Randomly chosen child: " + randomChild.name);
        
        if (randomChild.name == "Beam_attack")
        {
            Vector2 attackPos = randomChild.position;
            attackPos.x = lastPlayerPosition.x;
            randomChild.position = attackPos;
            randomChild.gameObject.SetActive(true);
            attackCooldown = 4f;
        }
        else if (randomChild.name == "StarWeapon")
        {
            Vector2 attackPos = randomChild.position;
            attackPos.x = lastPlayerPosition.x;
            randomChild.position = attackPos;
            randomChild.gameObject.GetComponent<WeaponStar>().Fire();
            attackCooldown = 3f;
        }

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
