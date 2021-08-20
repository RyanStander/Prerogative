using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandEquipmentSlotUI : MonoBehaviour
{
    private UIManager uiManager;
    public Image icon;
    private WeaponItem weapon;

    public bool isLeftHandSlot=false;
    public int slotNum = 0;
    
    private void Awake()
    {
        uiManager = FindObjectOfType<UIManager>();
    }
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

    public void SelectThisSlot()
    {
        uiManager.selectedSlotNum = slotNum;
        uiManager.isSelectedSlotALeftHandSlot = isLeftHandSlot;
    }
}
