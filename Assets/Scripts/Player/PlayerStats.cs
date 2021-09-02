using UnityEngine;
using System.Collections;

public class PlayerStats : CharacterStats
{
    private PlayerAnimatorManager playerAnimatorManager;
    private PlayerManager playerManager;
    private void Awake()
    {
        playerAnimatorManager = GetComponentInChildren<PlayerAnimatorManager>();
        playerManager = GetComponent<PlayerManager>();
    }

    private void Start()
    {
        maxHealth = SetMaxHealthFromHealthLevel();
        currentHealth = maxHealth;
        healthBar.SetMaxValue(maxHealth);

        maxStamina = SetMaxStaminaFromStaminaLevel();
        currentStamina = maxStamina;
        staminaBar.SetMaxValue(maxStamina);

        maxMagicka = SetMaxMagickaFromMagickaLevel();
        currentMagicka = maxMagicka;
        magickaBar.SetMaxValue(maxMagicka);

    }
    private void FixedUpdate()
    {
        HandleStaminaRegeneration();
    }

    #region Health
    private float SetMaxHealthFromHealthLevel()
    {
        //calculates the players health based on health level
        return healthLevel * 10;
    }

    public void TakeDamage(float damage)
    {
        if (playerManager.isInvulnerable)
            return;

        if (isDead)
            return;

        //change current health
        currentHealth -= damage;

        //pass the current health to the health bar
        healthBar.SetCurrentValue(currentHealth);

        //play animation that player has taken damage
        playerAnimatorManager.PlayTargetAnimation("Impact_03", true);

        //If player health reaches or goes pass 0, play death animation and handle death
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            playerAnimatorManager.PlayTargetAnimation("Death_01",true);

            isDead = true;
            //Handle player death
        }
    }

    public void ReceiveHealing(float healingAmount)
    {
        if (isDead)
            return;

        //change current health
        currentHealth += healingAmount;

        //limit health gain to not go over max
        if (currentHealth>maxHealth)
        {
            currentHealth = maxHealth;
        }

        //pass the current health to the health bar
        healthBar.SetCurrentValue(currentHealth);
    }
    #endregion

    #region Stamina

    public void HandleStaminaRegeneration()
    {
        if (canRegen && currentStamina != maxHealth)
        {
            if (currentStamina < maxStamina && staminaRegenTimeStamp <= Time.time)
            {
                RegenerateStamina();
            }     
        }

        if (staminaCDTimeStamp <= Time.time)
            canRegen = true;
    }

    public bool HasEnoughStaminaForAttack()
    {
        if (currentStamina>0)
            return true;
        else
            return false;
    }

    private float SetMaxStaminaFromStaminaLevel()
    {
        //calculates the players health based on health level
        return staminaLevel * 10;
    }

    public void DrainStamina(float drain)
    {
        //change current stamina
        currentStamina = currentStamina - drain;
        staminaBar.SetCurrentValue(currentStamina);
    }
    
    private void RegenerateStamina()
    {
        currentStamina++;
        staminaRegenTimeStamp = Time.time + staminaRegenRate;
        staminaBar.SetCurrentValue(currentStamina);
    }

    public void PutStaminaRegenOnCooldown()
    {
        staminaCDTimeStamp = Time.time + staminaRegenCooldownTime;
        canRegen = false;
    }

    #endregion

    #region Magicka

    private float SetMaxMagickaFromMagickaLevel()
    {
        //calculates the players magicka based on magicka level
        return magickaLevel * 10;
    }

    public void ConsumeMagicka(int magickaCost)
    {
        currentMagicka -= magickaCost;
        if (currentMagicka<0)
            currentMagicka = 0;
        magickaBar.SetCurrentValue(currentMagicka);
    }

    #endregion
}
