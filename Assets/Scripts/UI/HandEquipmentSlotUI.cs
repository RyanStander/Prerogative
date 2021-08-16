using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandEquipmentSlotUI : MonoBehaviour
{
    public Image icon;
    private WeaponItem weapon;

    public bool isLeftSlot=false;
    public int slotNum = 0;
    

    public void AddItem(WeaponItem newWeapon)
    {
        if (newWeapon != null)
        {
            weapon = newWeapon;
            icon.sprite = weapon.itemIcon;
            //icon.enabled = true;
            gameObject.SetActive(true);
        }
        else
        {
            icon.sprite = null;
        }
        
    }

    public void ClearItem()
    {
        weapon = null;
        icon.sprite = null;
        gameObject.SetActive(false);
    }
}
