using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : CharacterStats
{
    private Animator animator;

    private Color originalColor;
    private MeshRenderer meshRenderer;

    private float fade = 1f;

    private bool isDead = false;
    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer!=null)
            originalColor = meshRenderer.material.color;
    }

    private void Start()
    {
        currentHealth = maxHealth;
        if (healthBar!=null)
        healthBar.SetMaxValue(maxHealth);
    }

    private void FixedUpdate()
    {
        if (isDead)
        {
            if (fade > 0.2f)
            {
                fade -= (Time.deltaTime * fade)/2;
                if (meshRenderer != null)
                    meshRenderer.material.color = new Color(originalColor.r, originalColor.g, originalColor.b, fade);
            }
            else
            {
                Destroy(gameObject);
            }

        }
    }

    #region Health

    public void TakeDamage(float damage)
    {
        //change current health
        currentHealth = currentHealth - damage;

        //pass the current health to the health bar
        if (healthBar != null)
            healthBar.SetCurrentValue(currentHealth);

        //play animation that player has taken damage
        if (animator != null)
            animator.Play("Impact_03");

        //If player health reaches or goes pass 0, play death animation and handle death
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            isDead = true;

            if (animator != null)
                animator.Play("Death_01");

            //Handle player death
        }
    }
    #endregion
}
