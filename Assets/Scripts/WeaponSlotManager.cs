using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSlotManager : MonoBehaviour
{
    public WeaponItem attackingWeapon;

    private WeaponHolderSlot leftHandSlot, rightHandSlot;

    private DamageCollider leftHandDamageCollider, rightHandDamageCollider;

    private Animator animator;

    private QuickSlotsUI quickSlotsUI;

    private PlayerStats playerStats;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        quickSlotsUI = FindObjectOfType<QuickSlotsUI>();
        playerStats = GetComponentInParent<PlayerStats>();

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
        }
    }

    public void LoadWeaponOnSlot(WeaponItem weaponItem,bool isLeft)
    {
        if (isLeft)
        {
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
            rightHandSlot.LoadWeaponModel(weaponItem);
            if (rightHandSlot != null)
                LoadRightWeaponDamageCollider();
            quickSlotsUI.UpdateWeaponQuickSlotsUI(false, weaponItem);
            #region Weapon Idle Anim
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

    public void OpenRightHandDamageCollider()
    {
        rightHandDamageCollider.EnableDamageCollider();
    }

    public void OpenLeftHandDamageCollider()
    {
        leftHandDamageCollider.EnableDamageCollider();
    }

    public void CloseRightHandDamageCollider()
    {
        rightHandDamageCollider.DisableDamageCollider();
    }

    public void CloseLeftHandDamageCollider()
    {
        leftHandDamageCollider.DisableDamageCollider();
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
