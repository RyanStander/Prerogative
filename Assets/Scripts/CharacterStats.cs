using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header("Health")]
    public int healthLevel = 10;
    public float maxHealth, currentHealth;
    public bool isDead = false;

    [Header("Stamina")]
    public int staminaLevel = 10;
    public float maxStamina, currentStamina;
    [SerializeField] protected float staminaRegenRate = 0.1f, staminaRegenCooldownTime = 2;
    protected float staminaCDTimeStamp, staminaRegenTimeStamp;
    protected bool canRegen = true;

    [Header("Magicka")]
    public int magickaLevel = 10;
    public float maxMagicka;
    public float currentMagicka;

    [Header("Bars")]
    public SliderBarDisplayUI healthBar;
    public SliderBarDisplayUI staminaBar, magickaBar;
}
