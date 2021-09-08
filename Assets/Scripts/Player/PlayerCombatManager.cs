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

    private LayerMask backstabLayer = 1 << 13;
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
        if (playerStats.HasStamina())
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
        if (playerStats.HasStamina())
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
        if (playerStats.HasStamina())
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

    public void HandlePrimaryHeldAttackAction()
    {
        /*if (playerManager.canDoCombo)
        {
            inputHandler.comboFlag = true;
            HandleWeaponCombo(playerInventory.rightWeapon);
            inputHandler.comboFlag = false;
        }
        else
        {
            if (playerManager.isInteracting)
                return;

            if (playerManager.canDoCombo)
                return;

            AttemptBackStabOrParry();

            HandleHeavyAttack(playerInventory.rightWeapon);
        }*/
        AttemptBackStabOrParry();
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
        if (playerManager.isInteracting)
            return;
        if (playerStats.currentMagicka >= playerInventory.currentSpell.magickaCost)
            switch (weapon.weaponType)
            {
                case WeaponItem.WeaponType.healingWeapon:
                    if (playerInventory.currentSpell.spellType == SpellItem.SpellType.healingAbility)
                    {
                        //Attempt to cast spell
                        playerInventory.currentSpell.AttemptToCastSpell(playerAnimatorManager, playerStats);
                    }
                    break;
                case WeaponItem.WeaponType.casterWeapon2:
                    if (playerInventory.currentSpell.spellType == SpellItem.SpellType.spellType2)
                    {
                        //Attempt to cast spell
                    }
                    break;
                case WeaponItem.WeaponType.casterWeapon3:
                    if (playerInventory.currentSpell.spellType == SpellItem.SpellType.spellType3)
                    {
                        //Attempt to cast spell
                    }
                    break;
                case WeaponItem.WeaponType.meleeWeapon:
                    //this shouldnt happen, oh no
                    break;
                default:
                    break;
            }
        else
        {
            playerAnimatorManager.PlayTargetAnimation("Shrug", true);
        }
    }

    private void SuccessfulyCastSpell()
    {
        playerInventory.currentSpell.SuccessfullyCastSpell(playerAnimatorManager, playerStats);
    }

    private void AttemptBackStabOrParry()
    {
        RaycastHit hit;

        if (Physics.Raycast(inputHandler.criticalAttackRayCastStartPoint.position, 
            transform.TransformDirection(Vector3.forward), out hit, 0.5f, backstabLayer))
        {
            CharacterManager enemyCharacterManager = hit.transform.gameObject.GetComponentInParent<CharacterManager>();
            DamageCollider rightWeaponDamageCollider = weaponSlotManager.rightHandDamageCollider;

            if (enemyCharacterManager!=null)
            {
                //Check for team i.d (so cant back stab allies/self)
                //pull us into a transform behind the enemy so the backstab looks clean
                enemyCharacterManager.backstabCollider.LerpToPoint(playerManager.transform);
                //rotate us towards that transform
                Vector3 rotationDirection = playerManager.transform.root.eulerAngles;
                rotationDirection = hit.transform.position - playerManager.transform.position;
                rotationDirection.y = 0;
                rotationDirection.Normalize();
                Quaternion tr = Quaternion.LookRotation(rotationDirection);
                Quaternion targetRotation = Quaternion.Slerp(playerManager.transform.rotation, tr, 500 * Time.deltaTime);
                playerManager.transform.rotation = targetRotation;

                float criticalDamage = playerInventory.rightWeapon.criticalDamageMultiplier * rightWeaponDamageCollider.currentWeaponDamage;
                enemyCharacterManager.pendingCriticalDamage = criticalDamage;
                //play animation
                playerAnimatorManager.PlayTargetAnimation("Backstab", true);
                //make enemy play animation
                enemyCharacterManager.GetComponentInChildren<AnimatorManager>().PlayTargetAnimation("Backstabbed", true);
                //do damage
            }
        }
    }

    #endregion
}
