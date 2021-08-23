using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSlotManager : MonoBehaviour
{
    private PlayerManager playerManager;

    public WeaponItem attackingWeapon;

    private WeaponHolderSlot leftHandSlot, rightHandSlot, backSlot;

    private DamageCollider leftHandDamageCollider, rightHandDamageCollider;

    private Animator animator;

    private QuickSlotsUI quickSlotsUI;

    private PlayerStats playerStats;

    private InputHandler inputHandler;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        quickSlotsUI = FindObjectOfType<QuickSlotsUI>();
        playerStats = GetComponentInParent<PlayerStats>();
        inputHandler = GetComponentInParent<InputHandler>();
        playerManager = GetComponentInParent<PlayerManager>();

        WeaponHolderSlot[] weaponHolderSlots = GetComponentsInChildren<WeaponHolderSlot>();
        foreach (WeaponHolderSlot weaponHolderSlot in weaponHolderSlots)
        {
            if (weaponHolderSlot.isLeftHandSlot)
            {
                leftHandSlot = weaponHolderSlot;
            }
            else if (weaponHolderSlot.isRightHandSlot)
            {
                rightHandSlot = weaponHolderSlot;
            }
            else if (weaponHolderSlot.isBackSlot)
            {
                backSlot = weaponHolderSlot;
            }
        }
    }

    public void LoadWeaponOnSlot(WeaponItem weaponItem,bool isLeft)
    {
        if (isLeft)
        {
            leftHandSlot.currentWeapon = weaponItem;
            leftHandSlot.LoadWeaponModel(weaponItem);
            if (leftHandSlot != null)
                LoadLeftWeaponDamageCollider();
            quickSlotsUI.UpdateWeaponQuickSlotsUI(true,weaponItem);

            #region Weapon Idle Anim
            if (weaponItem != null)
            {
                animator.CrossFade(weaponItem.leftHandIdle, 0.2f);
            }
            else
            {
                animator.CrossFade("Left Arm Empty", 0.2f);
            }
            #endregion
        }
        else
        {
            if (inputHandler.twoHandFlag)
            {

                //Move current left hand weapon to the back
                backSlot.LoadWeaponModel(leftHandSlot.currentWeapon);
                leftHandSlot.UnloadWeaponAndDestroy();

                animator.CrossFade(weaponItem.twoHandIdle, 0.2f);
            }
            else
            {
                #region Weapon Idle Anim           
                
                animator.CrossFade("Both Arms Empty", 0.2f);

                backSlot.UnloadWeaponAndDestroy();

                if (weaponItem != null)
                {
                    animator.CrossFade(weaponItem.rightHandIdle, 0.2f);
                }
                else
                {
                    animator.CrossFade("Right Arm Empty", 0.2f);
                }
                #endregion
            }
            rightHandSlot.currentWeapon = weaponItem;
            rightHandSlot.LoadWeaponModel(weaponItem);
            if (rightHandSlot != null)
                LoadRightWeaponDamageCollider();
            quickSlotsUI.UpdateWeaponQuickSlotsUI(false, weaponItem);
        }
    }

    #region Damage Colliders
    private void LoadLeftWeaponDamageCollider()
    {
        if (leftHandSlot != null&& leftHandSlot.currentWeaponModel!=null)
        leftHandDamageCollider = leftHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
    }

    private void LoadRightWeaponDamageCollider()
    {
        if (rightHandSlot != null && rightHandSlot.currentWeaponModel != null)
            rightHandDamageCollider = rightHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
    }

    public void OpenDamageCollider()
    {
        if (playerManager.isUsingLeftHand)
        {
            leftHandDamageCollider.EnableDamageCollider();
        }
        else
        {
            rightHandDamageCollider.EnableDamageCollider();
        }

    }

    public void CloseDamageCollider()
    {
        if (playerManager.isUsingLeftHand)
        {
            leftHandDamageCollider.DisableDamageCollider();
        }
        else
        {
            rightHandDamageCollider.DisableDamageCollider();
        }

    }

    #endregion

    public void DrainLightStaminaAttack()
    {
        //Drains stamina based on what attack type the player is using
        playerStats.DrainStamina(attackingWeapon.baseStaminaCost * attackingWeapon.lightAttackMultiplier);
    }

    public void DrainHeavyStaminaAttack()
    {
        //Drains stamina based on what attack type the player is using
        playerStats.DrainStamina(attackingWeapon.baseStaminaCost * attackingWeapon.heavyAttackMultiplier);
    }
}
