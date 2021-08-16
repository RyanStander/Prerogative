using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentWindowUI : MonoBehaviour
{
    public bool[] rightHandSlotsSelected,leftHandSlotsSelected;

    HandEquipmentSlotUI[] handEquipmentSlotUIs;

    private void Awake()
    {
        handEquipmentSlotUIs = GetComponentsInChildren<HandEquipmentSlotUI>();
    }

    public void LoadWeaponsOnEquipmentScreen(PlayerInventory playerInventory)
    {
        //add weapon from players inventory to weapon slot on the players display
        foreach (HandEquipmentSlotUI handEquipmentSlot in handEquipmentSlotUIs)
        {
            if (handEquipmentSlot.isLeftSlot)
            {
                handEquipmentSlot.AddItem(playerInventory.weaponsInLeftHandSlots[handEquipmentSlot.slotNum]);
            }
            else
            {
                handEquipmentSlot.AddItem(playerInventory.weaponsInRightHandSlots[handEquipmentSlot.slotNum]);
            }
        }
    }

    public void SelectRightHandSlot(int slotNum)
    {
        rightHandSlotsSelected[slotNum+1] = true;
    }

    public void SelectLeftHandSlot(int slotNum)
    {
        leftHandSlotsSelected[slotNum+1] = true;
    }

}
