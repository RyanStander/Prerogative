using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header("Health")]
    public float healthLevel = 10;
    public float maxHealth, currentHealth;

    [Header("Stamina")]
    public float staminaLevel = 10;
    public float maxStamina, currentStamina;
    [SerializeField] protected float staminaRegenRate = 0.1f, staminaRegenCooldownTime = 2;
    protected float staminaCDTimeStamp, staminaRegenTimeStamp;
    protected bool canRegen = true;

    [Header("Bars")]
    public SliderBarDisplayUI healthBar;
    public SliderBarDisplayUI staminaBar;
}
