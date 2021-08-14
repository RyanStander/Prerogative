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

    public List<WeaponItem> weaponsInventory;
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
        //Changes weapon based on current index
        currentRightWeaponIndex = currentRightWeaponIndex + 1;

        //iterates through all weapons
        for (int i = 0; i < weaponsInRightHandSlots.Length; i++)
        {
            //if the weapon to swap to is not null
            if (currentRightWeaponIndex == i && weaponsInRightHandSlots[i] != null)
            {
                //swap to that weapon
                rightWeapon = weaponsInRightHandSlots[currentRightWeaponIndex];
                weaponSlotManager.LoadWeaponOnSlot(weaponsInRightHandSlots[currentRightWeaponIndex], false);
            }
            //else if the weapon to swap to is null
            else if (currentRightWeaponIndex == i && weaponsInRightHandSlots[i] == null)
            {
                //change to next index
                currentRightWeaponIndex = currentRightWeaponIndex + 1;
            }
        }

        //if the index is larger than total amount of weapons
        if (currentRightWeaponIndex > weaponsInRightHandSlots.Length - 1)
        {
            //change to -1 and swap to unarmed weapon
            currentRightWeaponIndex = -1;
            rightWeapon = unarmedWeapon;
            weaponSlotManager.LoadWeaponOnSlot(unarmedWeapon,false);
        }
    }
    public void ChangeLeftWeapon()
    {
        //Changes weapon based on current index
        currentLeftWeaponIndex = currentLeftWeaponIndex + 1;

        //iterates through all weapons
        for (int i = 0; i < weaponsInLeftHandSlots.Length; i++)
        {
            //if the weapon to swap to is not null
            if (currentLeftWeaponIndex == i && weaponsInLeftHandSlots[i] != null)
            {
                //swap to that weapon
                leftWeapon = weaponsInLeftHandSlots[currentLeftWeaponIndex];
                weaponSlotManager.LoadWeaponOnSlot(weaponsInLeftHandSlots[currentLeftWeaponIndex], true);
            }
            //else if the weapon to swap to is null
            else if (currentLeftWeaponIndex == i && weaponsInLeftHandSlots[i] == null)
            {
                //change to next index
                currentLeftWeaponIndex = currentLeftWeaponIndex + 1;
            }
        }

        //if the index is larger than total amount of weapons
        if (currentLeftWeaponIndex > weaponsInLeftHandSlots.Length - 1)
        {
            //change to -1 and swap to unarmed weapon
            currentLeftWeaponIndex = -1;
            leftWeapon = unarmedWeapon;
            weaponSlotManager.LoadWeaponOnSlot(unarmedWeapon, true);
        }
    }
}
