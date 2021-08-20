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
    public GameObject equipmentScreenWindow;

    [Header("Weapon Inventory")]
    public GameObject weaponInventorySlotPrefab;//Prefab of the inventory slot that will be created for all weapons
    public Transform weaponInventorySlotsParent;//transform that the weapons slots will instantiate onto
    private WeaponInventorySlot[] weaponInventorySlots;

    [Header("Equipment")]
    private EquipmentWindowUI equipmentWindowUI;

    [Header("Equipment Window Slot Selected")]
    public bool isSelectedSlotALeftHandSlot;
    public int selectedSlotNum=-1;

    private void Awake()
    {
        //equipmentWindowUI = FindObjectOfType<EquipmentWindowUI>();
        equipmentWindowUI = equipmentScreenWindow.GetComponent<EquipmentWindowUI>();
        //equipmentWindowUI.LoadWeaponsOnEquipmentScreen(playerInventory);
    }

    private void Start()
    {
        weaponInventorySlots = weaponInventorySlotsParent.GetComponentsInChildren<WeaponInventorySlot>();       
    }
    public void UpdateUI()
    {
        #region Weapon Inventory Slots
        for (int i = 0; i < weaponInventorySlots.Length; i++)
        {
            if (i<playerInventory.weaponsInventory.Count)
            {
                //if there are less weapon inventory slots than there are weapons in the players inventory
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

    public void ReloadEquipmentWindow()
    {
        equipmentWindowUI.LoadWeaponsOnEquipmentScreen(playerInventory);
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
        equipmentScreenWindow.SetActive(true);
        ReloadEquipmentWindow();
    }

    public void CloseEquipmentWindow()
    {
        equipmentScreenWindow.SetActive(false);
    }
    #endregion

    //closes the windows like equipment and inventory
    public void CloseAllSecondaryWindows()
    {
        ResetAllSelectedSlots();
        weaponInventoryWindow.SetActive(false);
        equipmentScreenWindow.SetActive(false);
    }

    public void ResetAllSelectedSlots()
    {
        selectedSlotNum = -1;
    }
}
