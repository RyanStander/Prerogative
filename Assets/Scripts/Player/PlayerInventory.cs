using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private WeaponSlotManager weaponSlotManager;

    public WeaponItem rightWeapon, leftWeapon;

    private void Awake()
    {
        weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
    }

    private void Start()
    {
        if (rightWeapon!=null)
            weaponSlotManager.LoadWeaponOnSlot(rightWeapon, false);
        if (leftWeapon != null)
            weaponSlotManager.LoadWeaponOnSlot(leftWeapon, true);
    }
}
