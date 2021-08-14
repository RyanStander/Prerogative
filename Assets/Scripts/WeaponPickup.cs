using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : Interactable
{
    public WeaponItem weapon;

    public override void Interact(PlayerManager playerManager)
    {
        base.Interact(playerManager);

        PickUpItem(playerManager);
    }

    private void PickUpItem(PlayerManager playerManager)
    {
        PlayerInventory playerInventory;
        PlayerLocomotion playerLocomotion;
        AnimatorHandler animatorHandler;

        playerInventory = playerManager.GetComponent<PlayerInventory>();
        playerLocomotion = playerManager.GetComponent<PlayerLocomotion>();
        animatorHandler = playerManager.GetComponentInChildren<AnimatorHandler>();

        //Stops player from moving while picking up item
        playerLocomotion.rigidbody.velocity = Vector3.zero;
        //Plays the animation of picking up item
        animatorHandler.PlayTargetAnimation("PickUpItem", true);
        playerInventory.weaponsInventory.Add(weapon);
        Destroy(gameObject);
    }
}
