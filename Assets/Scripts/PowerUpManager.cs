using System.Collections;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum BuildType { Damage, Fitness, Adrenaline }
    public BuildType currentBuild;

    private float bulletDamageMultiplier = 1f;
    private int specialAttackDamage = 100; // Example value
    private float specialAttackRange = 10f; // Example value

    private bool isImmune = false;
    private int speedCounter = 0;
    private int jumpCounter = 0;
    private int damageBoostCounter = 0;

    private float cumulativeSpeedMultiplier = 1f;
    private float cumulativeJumpMultiplier = 1f;
    private float cumulativeDamageMultiplier = 1f;

    private Thrill_Player thrillPlayer;
    private PlayerController playerController;
    private Health_Player healthPlayer;

    private void Start()
    {
        thrillPlayer = GetComponent<Thrill_Player>();
        if (thrillPlayer == null)
        {
            Debug.LogError("Thrill_Player component not found on the player.");
        }

        playerController = GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("PlayerController component not found on the player.");
        }

        healthPlayer = GetComponent<Health_Player>();
        if (healthPlayer == null)
        {
            Debug.LogError("Health_Player component not found on the player.");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            TriggerPowerUpI();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            TriggerPowerUpO();
        }
    }

    private void TriggerPowerUpI()
    {
        switch (currentBuild)
        {
            case BuildType.Damage:
                if (thrillPlayer.Thrill >= 50)
                {
                    thrillPlayer.DecreaseThrill(50);
                    StartCoroutine(DamageBoost());
                }
                break;
            case BuildType.Fitness:
                if (thrillPlayer.Thrill >= 300)
                {
                    thrillPlayer.DecreaseThrill(300);
                    RegenerateHealth();
                }
                break;
            case BuildType.Adrenaline:
                if (thrillPlayer.Thrill >= 100)
                {
                    thrillPlayer.DecreaseThrill(100);
                    StartCoroutine(DoubleJumpHeight());
                }
                break;
        }
    }

    private void TriggerPowerUpO()
    {
        switch (currentBuild)
        {
            case BuildType.Damage:
                if (thrillPlayer.Thrill >= 200)
                {
                    thrillPlayer.DecreaseThrill(200);
                    StartCoroutine(SpecialAttack());
                }
                break;
            case BuildType.Fitness:
                if (thrillPlayer.Thrill >= 180)
                {
                    thrillPlayer.DecreaseThrill(180);
                    StartCoroutine(BecomeImmune());
                }
                break;
            case BuildType.Adrenaline:
                if (thrillPlayer.Thrill >= 200)
                {
                    thrillPlayer.DecreaseThrill(200);
                    StartCoroutine(IncreaseSpeed());
                }
                break;
        }
    }

    private IEnumerator DamageBoost()
    {
        damageBoostCounter++;
        cumulativeDamageMultiplier *= 1.05f;
        bulletDamageMultiplier = cumulativeDamageMultiplier;
        yield return new WaitForSeconds(15);
        damageBoostCounter--;
        cumulativeDamageMultiplier /= 1.05f;
        bulletDamageMultiplier = cumulativeDamageMultiplier;
    }

    private IEnumerator SpecialAttack()
    {
        yield return new WaitForSeconds(1);
        Collider[] enemies = Physics.OverlapSphere(transform.position, specialAttackRange);
        foreach (Collider enemy in enemies)
        {
            if (enemy.GetComponent<Health_Knight>() != null)
            {
                enemy.GetComponent<Health_Knight>().Damage(specialAttackDamage);
            }
            else if (enemy.GetComponent<Health_Wasp>() != null)
            {
                enemy.GetComponent<Health_Wasp>().Damage(specialAttackDamage);
            }
        }
    }

    private void RegenerateHealth()
    {
        healthPlayer.Heal(healthPlayer.MaxHp * 0.3f);
    }

    private IEnumerator BecomeImmune()
    {
        isImmune = true;
        yield return new WaitForSeconds(15);
        isImmune = false;
    }

    private IEnumerator DoubleJumpHeight()
    {
        jumpCounter++;
        cumulativeJumpMultiplier *= 2f;
        playerController.JumpForce = playerController.OriginalJumpForce * cumulativeJumpMultiplier;
        yield return new WaitForSeconds(20);
        jumpCounter--;
        cumulativeJumpMultiplier /= 2f;
        playerController.JumpForce = playerController.OriginalJumpForce * cumulativeJumpMultiplier;
    }

    private IEnumerator IncreaseSpeed()
    {
        speedCounter++;
        cumulativeSpeedMultiplier *= 1.5f;
        playerController.WalkSpeed = playerController.OriginalWalkSpeed * cumulativeSpeedMultiplier;
        yield return new WaitForSeconds(10);
        speedCounter--;
        cumulativeSpeedMultiplier /= 1.5f;
        playerController.WalkSpeed = playerController.OriginalWalkSpeed * cumulativeSpeedMultiplier;
    }

    // Methods to get multipliers for use in other scripts
    public float GetBulletDamageMultiplier()
    {
        return bulletDamageMultiplier;
    }

    public bool IsImmune()
    {
        return isImmune;
    }

    public float GetMoveSpeed()
    {
        return playerController.WalkSpeed;
    }

    public float GetJumpHeight()
    {
        return playerController.JumpForce;
    }

    public float GetCurrentHP()
    {
        return healthPlayer.HP;
    }
}
