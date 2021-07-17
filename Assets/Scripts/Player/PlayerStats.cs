using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public float healthLevel = 10, maxHealth,currentHealth;

    public HealthBar healthBar;

    private AnimatorHandler animatorHandler;

    private void Awake()
    {
        animatorHandler = GetComponentInChildren<AnimatorHandler>();
    }

    private void Start()
    {
        maxHealth = SetMaxHealthFromHealthLevel();
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
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
        healthBar.SetCurrentHealth(currentHealth);

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
}
