using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenChest : Interactable
{
    [Tooltip("The point that the player will be moved to when interacting with a chest")]
    [SerializeField] private Transform playerSnapToPosition;

    private Animator animator;
    private OpenChest openChest;
    [SerializeField] private GameObject itemSpawner;
    [SerializeField] private WeaponItem itemInChest;

    private int rotationSpeed = 300;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        openChest = GetComponent<OpenChest>();
    }
    public override void Interact(PlayerManager playerManager)
    {
        PlayerAnimatorManager playerAnimatorManager;
        PlayerLocomotion playerLocomotion;

        playerAnimatorManager = playerManager.GetComponentInChildren<PlayerAnimatorManager>();
        playerLocomotion = playerManager.GetComponent<PlayerLocomotion>();

        //Rotate player towards the chest
        Vector3 rotationDirection = transform.position - playerManager.transform.position;
        rotationDirection.y = 0;
        rotationDirection.Normalize();

        //Stop the player from moving while interacting with the chest
        playerLocomotion.rigidbody.velocity = Vector3.zero;

        Quaternion tr = Quaternion.LookRotation(rotationDirection);
        Quaternion targetRotation = Quaternion.Slerp(playerManager.transform.rotation, tr, rotationSpeed * Time.deltaTime);
        playerManager.transform.rotation = targetRotation;

        //Lock transform to a certain point in front of chest
        playerManager.transform.position = playerSnapToPosition.position;

        //open the chest lid, and animate the player
        animator.Play("OpenChest");
        playerAnimatorManager.PlayTargetAnimation("PickUpItem", true);

        //spawn an item inside the chest the player can pick up
        StartCoroutine(SpawnItemInChest());
        //currently only using weaponpickup, will develope later for all
        WeaponPickup weaponPickup = itemSpawner.GetComponent<WeaponPickup>();

        if (weaponPickup!=null)
        {
            weaponPickup.weapon = itemInChest;
        }
    }

    private IEnumerator SpawnItemInChest()
    {
        yield return new WaitForSeconds(1f);
        Instantiate(itemSpawner, transform);
        Destroy(openChest);
    }
}
