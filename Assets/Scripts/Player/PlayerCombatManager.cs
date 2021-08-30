using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatManager : MonoBehaviour
{
    private PlayerAnimatorManager playerAnimatorManager;
    private PlayerManager playerManager;
    private PlayerInventory playerInventory;
    private InputHandler inputHandler;

    public string lastAttack;

    private WeaponSlotManager weaponSlotManager;
    private PlayerStats playerStats;
    private void Awake()
    {
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        weaponSlotManager = GetComponent<WeaponSlotManager>();

        inputHandler = GetComponentInParent<InputHandler>();
        playerStats = GetComponentInParent<PlayerStats>();
        playerManager = GetComponentInParent<PlayerManager>();
        playerInventory = GetComponentInParent<PlayerInventory>();
    }

    public void HandleWeaponCombo(WeaponItem weapon)
    {
        if (playerStats.HasEnoughStaminaForAttack())
        {
            playerStats.PutStaminaRegenOnCooldown();

            if (inputHandler.comboFlag)
            {
                playerAnimatorManager.anim.SetBool("canDoCombo", false);


                if (inputHandler.twoHandFlag)
                {
                    #region Two Handed Attacks
                    for (int i = 0; i < weapon.THLightAttacks.Count - 1; i++)
                    {
                        if (lastAttack == weapon.THLightAttacks[i])
                        {
                            lastAttack = weapon.THLightAttacks[i + 1];
                            playerAnimatorManager.PlayTargetAnimation(lastAttack, true);
                            break;
                        }
                    }
                    for (int i = 0; i < weapon.THHeavyAttacks.Count - 1; i++)
                    {
                        if (lastAttack == weapon.THHeavyAttacks[i])
                        {
                            lastAttack = weapon.THHeavyAttacks[i + 1];
                            playerAnimatorManager.PlayTargetAnimation(lastAttack, true);
                            break;
                        }
                    }
                    #endregion
                }
                else
                {
                    #region One Handed Attacks
                    for (int i = 0; i < weapon.OHLightAttacks.Count - 1; i++)
                    {
                        if (lastAttack == weapon.OHLightAttacks[i])
                        {
                            lastAttack = weapon.OHLightAttacks[i + 1];
                            playerAnimatorManager.PlayTargetAnimation(lastAttack, true);
                            break;
                        }
                    }
                    for (int i = 0; i < weapon.OHHeavyAttacks.Count - 1; i++)
                    {
                        if (lastAttack == weapon.OHHeavyAttacks[i])
                        {
                            lastAttack = weapon.OHHeavyAttacks[i + 1];
                            playerAnimatorManager.PlayTargetAnimation(lastAttack, true);
                            break;
                        }
                    }
                    #endregion
                }
            }
        }
    }

    public void HandleLightAttack(WeaponItem weapon)
    {
        if (playerStats.HasEnoughStaminaForAttack())
        {
            playerStats.PutStaminaRegenOnCooldown();
            weaponSlotManager.attackingWeapon = weapon;
            if (inputHandler.twoHandFlag)
            {
                if (weapon != null)
                {
                    playerAnimatorManager.PlayTargetAnimation(weapon.THLightAttacks[0], true);
                    lastAttack = weapon.THLightAttacks[0];
                }
            }
            else
            {
                if (weapon != null)
                {
                    playerAnimatorManager.PlayTargetAnimation(weapon.OHLightAttacks[0], true);
                    lastAttack = weapon.OHLightAttacks[0];
                }
            }
        }
    }

    public void HandleHeavyAttack(WeaponItem weapon)
    {
        if (playerStats.HasEnoughStaminaForAttack())
        {
            playerStats.PutStaminaRegenOnCooldown();
            weaponSlotManager.attackingWeapon = weapon;

            if (inputHandler.twoHandFlag)
            {
                if (weapon != null)
                {
                    playerAnimatorManager.PlayTargetAnimation(weapon.THHeavyAttacks[0], true);
                    lastAttack = weapon.THHeavyAttacks[0];
                }
            }
            else
            {
                if (weapon != null)
                {
                    playerAnimatorManager.PlayTargetAnimation(weapon.OHHeavyAttacks[0], true);
                    lastAttack = weapon.OHHeavyAttacks[0];
                }
            }
        }
    }

    #region Input Actions
    public void HandlePrimaryAttackAction()
    {
        if (playerInventory.rightWeapon.weaponType==WeaponItem.WeaponType.meleeWeapon)
        {
            PerformPrimaryMeleeAction();
        }
        else if (playerInventory.rightWeapon.weaponType == WeaponItem.WeaponType.healingWeapon
            || playerInventory.rightWeapon.weaponType == WeaponItem.WeaponType.casterWeapon2
            || playerInventory.rightWeapon.weaponType == WeaponItem.WeaponType.casterWeapon3)
        {
            PerformPrimaryMagicAction(playerInventory.rightWeapon);
        }
        else
        {
            Debug.LogWarning("The weapon type was not selected");
        }
    }
    
    #endregion

    #region Combat Actions

    private void PerformPrimaryMeleeAction()
    {
        //If current attack can perform a combo, proceed with combo
        if (playerManager.canDoCombo)
        {
            inputHandler.comboFlag = true;
            HandleWeaponCombo(playerInventory.rightWeapon);
            inputHandler.comboFlag = false;
        }
        //else, perform starting attack if possible
        else
        {
            if (playerManager.isInteracting)
                return;

            if (playerManager.canDoCombo)
                return;

            playerAnimatorManager.anim.SetBool("isUsingRightHand", true);
            HandleLightAttack(playerInventory.rightWeapon);
        }
    }
    
    private void PerformPrimaryMagicAction(WeaponItem weapon)
    {
        switch (weapon.weaponType)
        {
            case WeaponItem.WeaponType.healingWeapon:
                if (playerInventory.currentSpell.spellType==SpellItem.SpellType.healingAbility)
                {
                    //Check for mana
                    //Attempt to cast spell
                    playerInventory.currentSpell.AttemptToCastSpell(playerAnimatorManager, playerStats);
                }
                break;
            case WeaponItem.WeaponType.casterWeapon2:
                if (playerInventory.currentSpell.spellType == SpellItem.SpellType.spellType2)
                {
                    //Check for mana
                    //Attempt to cast spell
                }
                break;
            case WeaponItem.WeaponType.casterWeapon3:
                if (playerInventory.currentSpell.spellType == SpellItem.SpellType.spellType3)
                {
                    //Check for mana
                    //Attempt to cast spell
                }
                break;
            case WeaponItem.WeaponType.meleeWeapon:
                //this shouldnt happen, oh no
                break;
            default:
                break;
        }
    }

    private void SuccessfulyCastSpell()
    {
        playerInventory.currentSpell.SuccessfullyCastSpell(playerAnimatorManager, playerStats);
    }

    #endregion
}
