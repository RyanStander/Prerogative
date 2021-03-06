using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Items/Weapon Item")]
public class WeaponItem : Item
{
    public GameObject modelPrefab;
    public bool isUnarmed;

    [Header("Damage")]
    public int baseDamage= 25;
    public int criticalDamageMultiplier = 4;

    [Header("Absorption")]
    public float physicalDamageAbsorption;

    [Header("Idle Animations")]
    public string rightHandIdle;
    public string leftHandIdle;
    public string twoHandIdle;

    [Header("Attack Animations")]
    public List<string> OHLightAttacks;
    public List<string> THLightAttacks;
    public List<string> OHHeavyAttacks;
    public List<string> THHeavyAttacks;

    [Header("Weapon Art")]
    public string weaponArt;

    [Header("Stamina Costs")]
    public int baseStaminaCost;
    public float lightAttackMultiplier;
    public float heavyAttackMultiplier;

    [Header("WeaponType")]
    public WeaponType weaponType;
    
    public enum WeaponType
    {
        healingWeapon,
        casterWeapon2,
        casterWeapon3,
        meleeWeapon,
        shieldWeapon,
    }
}
