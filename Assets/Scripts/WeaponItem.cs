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

    [Header("One Handed Attack Animations")]
    public List<string> OHLightAttacks;
    public List<string> OHHeavyAttacks;
}
