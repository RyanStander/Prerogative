using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public PlayerInventory playerInventory;

    [Header("UI Windows")]
    public GameObject hudWindow;
    public GameObject selectWindow;
    public GameObject weaponInventoryWindow;

    [Header("Weapon Inventory")]
    public GameObject weaponInventorySlotPrefab;//Prefab of the inventory slot that will be created for all weapons
    public Transform weaponInventorySlotsParent;//transform that the weapons slots will instantiate onto
    private WeaponInventorySlot[] weaponInventorySlots;

    private void Start()
    {
        weaponInventorySlots = weaponInventorySlotsParent.GetComponentsInChildren<WeaponInventorySlot>();
    }
    public void UpdateUI()
    {
        #region Weapon Inventory Slots
        //if there are less weapon inventory slots than there are weapons in the players inventory
        for (int i = 0; i < weaponInventorySlots.Length; i++)
        {
            if (i<playerInventory.weaponsInventory.Count)
            {
                if (weaponInventorySlots.Length < playerInventory.weaponsInventory.Count)
                {
                    //create new weaponinventory slot and add to the grid
                    Instantiate(weaponInventorySlotPrefab, weaponInventorySlotsParent);
                    weaponInventorySlots = weaponInventorySlotsParent.GetComponentsInChildren<WeaponInventorySlot>();
                }
                weaponInventorySlots[i].AddItem(playerInventory.weaponsInventory[i]);
            }
            else
            {
                weaponInventorySlots[i].ClearInventorySlot();
            }
        }
        #endregion
    }

    public void ToggleSelectWindow()
    {
        selectWindow.SetActive(!selectWindow.activeSelf);
    }

    public void ToggleWeaponInventory()
    {
        weaponInventoryWindow.SetActive(!weaponInventoryWindow.activeSelf);
    }

    public void ToggleHUDWindow()
    {
        hudWindow.SetActive(!hudWindow.activeSelf);
    }

    public void CloseAllInventoryWindows()
    {
        weaponInventoryWindow.SetActive(false);
        hudWindow.SetActive(true);
    }
}
