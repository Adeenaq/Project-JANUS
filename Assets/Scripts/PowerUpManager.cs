using System.Collections;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum BuildType { Damage, Fitness, Adrenaline }
    public BuildType currentBuild;

    private float bulletDamageMultiplier = 1f;
    [SerializeField] private int specialAttackDamage = 100; // Example value
    [SerializeField] private float specialAttackRange = 10f; // Example value

    private bool isImmune = false;
    private int speedCounter = 0;
    private int jumpCounter = 0;
    private int damageBoostCounter = 0;

    [SerializeField] private float cumulativeSpeedMultiplier = 1f;
    [SerializeField] private float cumulativeJumpMultiplier = 1f;
    [SerializeField] private float cumulativeDamageMultiplier = 1f;

    private Thrill_Player thrillPlayer;
    private PlayerController playerController;
    private Health_Player healthPlayer;

    [SerializeField] private int seekerThrill = 50;
    [SerializeField] private int ThrillerThrill = 200;
    [SerializeField] private int RegenThrill = 300;
    [SerializeField] private int ImmunityThrill = 180;
    [SerializeField] private int JumpThrill = 100;
    [SerializeField] private int FlashThrill = 200;

    [SerializeField] private int SeekerTime = 15;
    [SerializeField] private int ThrillerTime = 1;
    [SerializeField] private int immunityTime = 15;
    [SerializeField] private int jumpTime = 20;
    [SerializeField] private int flashTime = 10; 


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
                if (thrillPlayer.Thrill >= seekerThrill)
                {
                    thrillPlayer.DecreaseThrill(seekerThrill);
                    StartCoroutine(DamageBoost());
                }
                break;
            case BuildType.Fitness:
                if (thrillPlayer.Thrill >= ImmunityThrill)
                {
                    thrillPlayer.DecreaseThrill(ImmunityThrill);
                    StartCoroutine(BecomeImmune());
                }
                break;
            case BuildType.Adrenaline:
                if (thrillPlayer.Thrill >= JumpThrill)
                {
                    thrillPlayer.DecreaseThrill(JumpThrill);
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
                if (thrillPlayer.Thrill >= ThrillerThrill)
                {
                    thrillPlayer.DecreaseThrill(ThrillerThrill);
                    StartCoroutine(SpecialAttack());
                }
                break;
            case BuildType.Fitness:
                if (thrillPlayer.Thrill >= RegenThrill)
                {
                    thrillPlayer.DecreaseThrill(RegenThrill);
                    RegenerateHealth();
                }
                break;
            case BuildType.Adrenaline:
                if (thrillPlayer.Thrill >= FlashThrill)
                {
                    thrillPlayer.DecreaseThrill(FlashThrill);
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
        yield return new WaitForSeconds(SeekerTime);
        damageBoostCounter--;
        cumulativeDamageMultiplier /= 1.05f;
        bulletDamageMultiplier = cumulativeDamageMultiplier;
    }

    private IEnumerator SpecialAttack()
    {
        yield return new WaitForSeconds(ThrillerTime);
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
        yield return new WaitForSeconds(immunityTime);
        isImmune = false;
    }

    private IEnumerator DoubleJumpHeight()
    {
        jumpCounter++;
        cumulativeJumpMultiplier *= 2f;
        playerController.JumpForce = playerController.OriginalJumpForce * cumulativeJumpMultiplier;
        yield return new WaitForSeconds(jumpTime);
        jumpCounter--;
        cumulativeJumpMultiplier /= 2f;
        playerController.JumpForce = playerController.OriginalJumpForce * cumulativeJumpMultiplier;
    }

    private IEnumerator IncreaseSpeed()
    {
        speedCounter++;
        cumulativeSpeedMultiplier *= 1.5f;
        playerController.WalkSpeed = playerController.OriginalWalkSpeed * cumulativeSpeedMultiplier;
        yield return new WaitForSeconds(flashTime);
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

    public int GetThreshHoldI()
    {
        switch (currentBuild)
        {
            case BuildType.Damage:
                return seekerThrill;
             
            case BuildType.Fitness:
                return ImmunityThrill;
               
            case BuildType.Adrenaline:
                return JumpThrill;

            default:
                return seekerThrill;
        }
    }

    public int GetThreshHoldO()
    {
        switch (currentBuild)
        {
            case BuildType.Damage:
                return ThrillerThrill;

            case BuildType.Fitness:
                return RegenThrill;

            case BuildType.Adrenaline:
                return FlashThrill;

            default:
                return ThrillerThrill;
        }
    }
}
