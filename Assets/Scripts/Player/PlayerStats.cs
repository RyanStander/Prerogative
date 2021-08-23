using UnityEngine;
using System.Collections;

public class PlayerStats : CharacterStats
{
    private AnimatorHandler animatorHandler;
    private void Awake()
    {
        animatorHandler = GetComponentInChildren<AnimatorHandler>();
    }

    private void Start()
    {
        maxHealth = SetMaxHealthFromHealthLevel();
        currentHealth = maxHealth;
        healthBar.SetMaxValue(maxHealth);

        maxStamina = SetMaxStaminaFromStaminaLevel();
        currentStamina = maxStamina;
        staminaBar.SetMaxValue(maxStamina);
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
        //change current health
        currentHealth = currentHealth - damage;

        //pass the current health to the health bar
        healthBar.SetCurrentValue(currentHealth);

        //play animation that player has taken damage
        animatorHandler.PlayTargetAnimation("Impact_03", true);

        //If player health reaches or goes pass 0, play death animation and handle death
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            animatorHandler.PlayTargetAnimation("Death_01",true);

            //Handle player death
        }
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
}
