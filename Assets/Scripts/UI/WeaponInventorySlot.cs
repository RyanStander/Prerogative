using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponInventorySlot : MonoBehaviour
{
    public Image icon;
    private WeaponItem item;

    public void AddItem(WeaponItem newItem)
    {
        item = newItem;
        icon.sprite = item.itemIcon;
        //icon.enable=true;
        gameObject.SetActive(true);
    }

    public void ClearInventorySlot()
    {
        item = null;
        icon.sprite = null;
        //icon.enable=false;
        gameObject.SetActive(false);
    }
}
