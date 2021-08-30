using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Items/Weapon Item")]
public class WeaponItem : Item
{
    public GameObject modelPrefab;
    public bool isUnarmed;

    [Header("Idle Animations")]
    public string rightHandIdle;
    public string leftHandIdle;
    public string twoHandIdle;

    [Header("One Handed Attack Animations")]
    public List<string> OHLightAttacks;
    public List<string> THLightAttacks;
    public List<string> OHHeavyAttacks;
    public List<string> THHeavyAttacks;

    [Header("Stamina Costs")]
    public int baseStaminaCost;
    public float lightAttackMultiplier;
    public float heavyAttackMultiplier;

    [Header("WeaponType")]
    public WeaponType weaponType;
    
    public enum WeaponType
    {
        healingAbility,
        spellType2,
        spellType3,
        meleeWeapon
    }
}
