using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatManager : MonoBehaviour
{
    private PlayerAnimatorManager playerAnimatorManager;
    private InputHandler inputHandler;

    public string lastAttack;

    private WeaponSlotManager weaponSlotManager;
    private PlayerStats playerStats;
    private void Awake()
    {
        playerAnimatorManager = GetComponentInChildren<PlayerAnimatorManager>();
        weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
        inputHandler = GetComponent<InputHandler>();
        playerStats = GetComponent<PlayerStats>();
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
}
