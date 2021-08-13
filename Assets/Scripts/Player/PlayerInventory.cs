using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private WeaponSlotManager weaponSlotManager;

    public WeaponItem rightWeapon, leftWeapon;

    public WeaponItem unarmedWeapon;

    public WeaponItem[] weaponsInRightHandSlots = new WeaponItem[1], weaponsInLeftHandSlots = new WeaponItem[1];

    public int currentRightWeaponIndex = 0, currentLeftWeaponIndex=0;
    private void Awake()
    {
        weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
    }

    private void Start()
    {
        rightWeapon = weaponsInRightHandSlots[currentRightWeaponIndex];
        leftWeapon = weaponsInLeftHandSlots[currentLeftWeaponIndex];

        if (rightWeapon!=null)
            weaponSlotManager.LoadWeaponOnSlot(rightWeapon, false);
        if (leftWeapon != null)
            weaponSlotManager.LoadWeaponOnSlot(leftWeapon, true);
    }

    public void ChangeRightWeapon()
    {
        currentRightWeaponIndex = currentRightWeaponIndex + 1;

        for (int i = 0; i < weaponsInRightHandSlots.Length; i++)
        {
            if (currentRightWeaponIndex == i && weaponsInRightHandSlots[i] != null)
            {
                rightWeapon = weaponsInRightHandSlots[currentRightWeaponIndex];
                weaponSlotManager.LoadWeaponOnSlot(weaponsInRightHandSlots[currentRightWeaponIndex], false);
            }
            else if (currentRightWeaponIndex == i && weaponsInRightHandSlots[i] == null)
            {
                currentRightWeaponIndex = currentRightWeaponIndex + 1;
            }
        }

        if (currentRightWeaponIndex > weaponsInRightHandSlots.Length - 1)
        {
            currentRightWeaponIndex = -1;
            rightWeapon = unarmedWeapon;
            weaponSlotManager.LoadWeaponOnSlot(unarmedWeapon,false);
        }
    }
    public void ChangeLeftWeapon()
    {
        currentLeftWeaponIndex = currentLeftWeaponIndex + 1;

        for (int i = 0; i < weaponsInLeftHandSlots.Length; i++)
        {
            if (currentLeftWeaponIndex == i && weaponsInLeftHandSlots[i] != null)
            {
                leftWeapon = weaponsInLeftHandSlots[currentLeftWeaponIndex];
                weaponSlotManager.LoadWeaponOnSlot(weaponsInLeftHandSlots[currentLeftWeaponIndex], false);
            }
            else if (currentLeftWeaponIndex == i && weaponsInLeftHandSlots[i] == null)
            {
                currentLeftWeaponIndex = currentLeftWeaponIndex + 1;
            }
        }

        if (currentLeftWeaponIndex > weaponsInLeftHandSlots.Length - 1)
        {
            currentLeftWeaponIndex = -1;
            leftWeapon = unarmedWeapon;
            weaponSlotManager.LoadWeaponOnSlot(unarmedWeapon, false);
        }
    }
}
