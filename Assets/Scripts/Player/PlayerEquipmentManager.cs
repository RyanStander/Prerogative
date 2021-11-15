using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipmentManager : MonoBehaviour
{
    InputHandler inputHandler;
    [SerializeField] BlockingCollider blockingCollider;
    PlayerInventory playerInventory;

    private void Awake()
    {
        inputHandler = GetComponentInParent<InputHandler>();
        playerInventory = GetComponentInParent<PlayerInventory>();
    }

    public void OpenBlockingCollider()
    {
        if (inputHandler.twoHandFlag)
            blockingCollider.SetColliderDamageAbsorption(playerInventory.rightWeapon);
        else
            blockingCollider.SetColliderDamageAbsorption(playerInventory.leftWeapon);
        blockingCollider.EnableBlockingCollider();
    }

    public void CloseBlockingCollider()
    {
        blockingCollider.DisableBlockingCollider();
    }
}
