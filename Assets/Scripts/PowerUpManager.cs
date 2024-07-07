using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum BuildType { Damage, Fitness, Adrenaline }
    public BuildType currentBuild;

    private Animator[] animators;

    [SerializeField] private float bulletDamageMultiplier = 1.05f;
    [SerializeField] private int specialAttackDamage = 100;
    [SerializeField] private float specialAttackRange = 10f;
    [SerializeField] private float healthRegenMultiplier = 0.3f;
    [SerializeField] private float JumpMultiplier = 2f;
    [SerializeField] private float SpeedMultiplier = 1.5f;

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

    private AudioSource audioSource;
    
    [SerializeField] private AudioClip seekerClip;
    [Range(0f, 1f)]
    [SerializeField] private float seekerVol = 1f;
    
    [SerializeField] private AudioClip thrillerClip;
    [Range(0f, 1f)]
    [SerializeField] private float thrillerVol = 1f;
    
    [SerializeField] private AudioClip healClip;
    [Range(0f, 1f)] 
    [SerializeField] private float healVol = 1f;

    [SerializeField] private AudioClip immunityClip;
    [Range(0f, 1f)]
    [SerializeField] private float immunityVol = 1f;

    [SerializeField] private AudioClip jumpClip;
    [Range(0f, 1f)]
    [SerializeField] float jumpVol = 1f;

    [SerializeField] private AudioClip flashClip;
    [Range(0f, 1f)]
    [SerializeField] float flashVol = 1f;
    


    private void Start()
    {
        animators = GetComponentsInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
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
        if (Input.GetKeyDown(KeyCode.Q))
        {
            TriggerPowerUpQ();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            TriggerPowerUpE();
        }
    }

    private void TriggerPowerUpQ()
    {
        switch (currentBuild)
        {
            case BuildType.Damage:
                if (thrillPlayer.Thrill >= seekerThrill)
                {
                    foreach (var a in animators)
                    {
                        PlayAnimationIfExists(a, "player_future_powerup");
                        PlayAnimationIfExists(a, "player_past_powerup");
                    }
                    thrillPlayer.DecreaseThrill(seekerThrill);
                    PlaySound(seekerClip, seekerVol);
                    StartCoroutine(DamageBoost());
                }
                break;
            case BuildType.Fitness:
                if (thrillPlayer.Thrill >= ImmunityThrill)
                {
                    foreach (var a in animators)
                    {
                        PlayAnimationIfExists(a, "player_future_powerup");
                        PlayAnimationIfExists(a, "player_past_powerup");
                    }
                    thrillPlayer.DecreaseThrill(ImmunityThrill);
                    PlaySound(immunityClip, immunityVol);
                    StartCoroutine(BecomeImmune());
                }
                break;
            case BuildType.Adrenaline:
                if (thrillPlayer.Thrill >= JumpThrill)
                {
                    foreach (var a in animators)
                    {
                        PlayAnimationIfExists(a, "player_future_powerup");
                        PlayAnimationIfExists(a, "player_past_powerup");
                    }
                    thrillPlayer.DecreaseThrill(JumpThrill);
                    PlaySound(jumpClip, jumpVol);
                    StartCoroutine(DoubleJumpHeight());
                }
                break;
        }
        
    }

    private void TriggerPowerUpE()
    {
        switch (currentBuild)
        {
            case BuildType.Damage:
                if (thrillPlayer.Thrill >= ThrillerThrill)
                {
                    foreach (var a in animators)
                    {
                        PlayAnimationIfExists(a, "player_future_powerup");
                        PlayAnimationIfExists(a, "player_past_powerup");
                    }
                    thrillPlayer.DecreaseThrill(ThrillerThrill);
                    PlaySound(thrillerClip, thrillerVol);
                    StartCoroutine(SpecialAttack());
                }
                break;
            case BuildType.Fitness:
                if (thrillPlayer.Thrill >= RegenThrill)
                {
                    foreach (var a in animators)
                    {
                        PlayAnimationIfExists(a, "player_future_powerup");
                        PlayAnimationIfExists(a, "player_past_powerup");
                    }
                    thrillPlayer.DecreaseThrill(RegenThrill);
                    PlaySound(healClip, healVol);
                    RegenerateHealth();
                }
                break;
            case BuildType.Adrenaline:
                if (thrillPlayer.Thrill >= FlashThrill)
                {
                    foreach (var a in animators)
                    {
                        PlayAnimationIfExists(a, "player_future_powerup");
                        PlayAnimationIfExists(a, "player_past_powerup");
                    }
                    thrillPlayer.DecreaseThrill(FlashThrill);
                    PlaySound(flashClip, flashVol);
                    StartCoroutine(IncreaseSpeed());
                }
                break;
        }
    }

    private IEnumerator DamageBoost()
    {
        damageBoostCounter++;
        cumulativeDamageMultiplier *= bulletDamageMultiplier;
        bulletDamageMultiplier = cumulativeDamageMultiplier;
        yield return new WaitForSeconds(SeekerTime);
        damageBoostCounter--;
        cumulativeDamageMultiplier /= bulletDamageMultiplier;
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
        healthPlayer.Heal(Mathf.Min(healthPlayer.MaxHp * healthRegenMultiplier, healthPlayer.MaxHp));
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
        cumulativeJumpMultiplier *= JumpMultiplier;
        playerController.JumpForce = playerController.OriginalJumpForce * cumulativeJumpMultiplier;
        yield return new WaitForSeconds(jumpTime);
        jumpCounter--;
        cumulativeJumpMultiplier /= JumpMultiplier;
        playerController.JumpForce = playerController.OriginalJumpForce * cumulativeJumpMultiplier;
    }

    private IEnumerator IncreaseSpeed()
    {
        speedCounter++;
        cumulativeSpeedMultiplier *= SpeedMultiplier;
        playerController.WalkSpeed = playerController.OriginalWalkSpeed * cumulativeSpeedMultiplier;
        yield return new WaitForSeconds(flashTime);
        speedCounter--;
        cumulativeSpeedMultiplier /= SpeedMultiplier;
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

    private void PlaySound(AudioClip clip, float volume)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }

    void PlayAnimationIfExists(Animator animator, string animationName)
    {
        if (animator.HasState(0, Animator.StringToHash(animationName)))
        {
            animator.Play(animationName, 0, 0f);
        }
    }
}

