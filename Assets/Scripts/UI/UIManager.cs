using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Menus")]
    public GameObject selectWindow;
    public GameObject inventoryWindow;

    public void ToggleSelectWindow()
    {
        selectWindow.SetActive(!selectWindow.activeSelf);
    }

    public void ToggleInventory()
    {
        inventoryWindow.SetActive(!inventoryWindow.activeSelf);
    }
}
