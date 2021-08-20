using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponInventorySlot : MonoBehaviour
{
    private PlayerInventory playerInventory;
    private WeaponSlotManager weaponSlotManager;
    private UIManager uiManager;
    public Image icon;
    private WeaponItem item;

    private void Awake()
    {
        playerInventory = FindObjectOfType<PlayerInventory>();
        weaponSlotManager = FindObjectOfType<WeaponSlotManager>();
        uiManager = FindObjectOfType<UIManager>();
    }

    public void AddItem(WeaponItem newItem)
    {
        if (icon != null)
        {
            item = newItem;
            icon.sprite = item.itemIcon;
            //icon.enable=true;
            gameObject.SetActive(true);
        }
    }

    public void ClearInventorySlot()
    {
        item = null;
        icon.sprite = null;
        //icon.enable=false;
        gameObject.SetActive(false);
    }
    
    public void EquipThisItem()
    {
        if (uiManager.selectedSlotNum != -1)
        {
            if (uiManager.isSelectedSlotALeftHandSlot)
            {
                //Add current item to inventory
                WeaponItem weaponItem = playerInventory.weaponsInLeftHandSlots[uiManager.selectedSlotNum];
                if (weaponItem != null)
                {
                    playerInventory.weaponsInventory.Add(weaponItem);
                }
                //Equip this new item
                playerInventory.weaponsInLeftHandSlots[uiManager.selectedSlotNum] = item;
                //Remove this item from inventory
                playerInventory.weaponsInventory.Remove(item);

            }
            else
            {
                //Add current item to inventory
                WeaponItem weaponItem = playerInventory.weaponsInRightHandSlots[uiManager.selectedSlotNum];
                if (weaponItem != null)
                {
                    playerInventory.weaponsInventory.Add(weaponItem);
                }
                //Equip this new item
                playerInventory.weaponsInRightHandSlots[uiManager.selectedSlotNum] = item;
                //Remove this item from inventory
                playerInventory.weaponsInventory.Remove(item);
            }
            //Display new weapon ui
            playerInventory.leftWeapon = playerInventory.weaponsInLeftHandSlots[playerInventory.currentLeftWeaponIndex];
            playerInventory.rightWeapon = playerInventory.weaponsInRightHandSlots[playerInventory.currentRightWeaponIndex];
            //Load new weapon model
            weaponSlotManager.LoadWeaponOnSlot(playerInventory.rightWeapon, false);
            weaponSlotManager.LoadWeaponOnSlot(playerInventory.leftWeapon, true);

            uiManager.ReloadEquipmentWindow();
            uiManager.ResetAllSelectedSlots();
        }
    }
}
