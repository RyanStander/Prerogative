using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickSlotsUI : MonoBehaviour
{
    public Image leftWeaponIcon, rightWeaponIcon;

    public void UpdateWeaponQuickSlotsUI(bool isLeft,WeaponItem weapon)
    {
        //Change icon for weapons based on if it is left or right weapon
        if (isLeft)
        {
            if (leftWeaponIcon != null)
            {
                if (weapon != null)
                {
                    //checks if weapon has an icon
                    if (weapon.itemIcon != null)
                    {
                        //display icon
                        leftWeaponIcon.sprite = weapon.itemIcon;
                        leftWeaponIcon.enabled = true;
                    }
                    else
                    {
                        //hide icon
                        leftWeaponIcon.sprite = null;
                        leftWeaponIcon.enabled = false;
                    }
                }
            }
            else
                Debug.LogWarning("leftWeaponIcon for Quickslots in playerUI was not assigned");
        }
        else
        {
            if (rightWeaponIcon != null)
            {
                if (weapon.itemIcon != null)
                {
                    //display icon
                    rightWeaponIcon.sprite = weapon.itemIcon;
                    rightWeaponIcon.enabled = true;
                }
                else
                {
                    //hide icon
                    rightWeaponIcon.sprite = null;
                    rightWeaponIcon.enabled = false;
                }
            }
            else
                Debug.LogWarning("rightWeaponIcon for Quickslots in playerUI was not assigned");
        }
    }
}
