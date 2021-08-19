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
    public GameObject equipmentWindow;

    [Header("Weapon Inventory")]
    public GameObject weaponInventorySlotPrefab;//Prefab of the inventory slot that will be created for all weapons
    public Transform weaponInventorySlotsParent;//transform that the weapons slots will instantiate onto
    private WeaponInventorySlot[] weaponInventorySlots;

    [Header("Equipment")]
    private EquipmentWindowUI equipmentWindowUI;

    private void Awake()
    {
        //equipmentWindowUI = FindObjectOfType<EquipmentWindowUI>();
        equipmentWindowUI = equipmentWindow.GetComponent<EquipmentWindowUI>();
        //equipmentWindowUI.LoadWeaponsOnEquipmentScreen(playerInventory);
    }

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

    #region Window Enables/Disables
    public void OpenSelectWindow()
    {
        selectWindow.SetActive(true);
    }

    public void CloseSelectWindow()
    {
        selectWindow.SetActive(false);
    }

    public void OpenHUDWindow()
    {
        hudWindow.SetActive(true);
    }

    public void CloseHUDWindow()
    {
        hudWindow.SetActive(false);
    }

    public void OpenWeaponInventoryWindow()
    {
        weaponInventoryWindow.SetActive(true);
        UpdateUI();
    }

    public void CloseWeaponInventoryWindow()
    {
        weaponInventoryWindow.SetActive(false);
    }

    public void OpenEquipmentWindow()
    {
        equipmentWindow.SetActive(true);
        equipmentWindowUI.LoadWeaponsOnEquipmentScreen(playerInventory);
    }

    public void CloseEquipmentWindow()
    {
        equipmentWindow.SetActive(false);
    }
    #endregion

    //closes the windows like equipment and inventory
    public void CloseAllSecondaryWindows()
    {
        weaponInventoryWindow.SetActive(false);
        equipmentWindow.SetActive(false);
    }
}
