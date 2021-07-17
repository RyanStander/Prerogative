using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatManager : MonoBehaviour
{
    private AnimatorHandler animatorHandler;
    private InputHandler inputHandler;

    public string lastAttack;

    private void Awake()
    {
        animatorHandler = GetComponentInChildren<AnimatorHandler>();
        inputHandler = GetComponent<InputHandler>();
    }

    public void HandleWeaponCombo(WeaponItem weapon)
    {
        if (inputHandler.comboFlag)
        {
            animatorHandler.anim.SetBool("canDoCombo", false);
            for (int i = 0; i < weapon.OHLightAttacks.Count-1; i++)
            {     
                if (lastAttack == weapon.OHLightAttacks[i])
                {
                    lastAttack = weapon.OHLightAttacks[i + 1];
                    animatorHandler.PlayTargetAnimation(lastAttack, true);
                    break;
                }
            }
            for (int i = 0; i < weapon.OHHeavyAttacks.Count - 1; i++)
            {
                if (lastAttack == weapon.OHHeavyAttacks[i])
                {
                    lastAttack = weapon.OHHeavyAttacks[i + 1];
                    animatorHandler.PlayTargetAnimation(lastAttack, true);
                    break;
                }
            }
        }
    }

    public void HandleLightAttack(WeaponItem weapon)
    {
        if (weapon != null)
        {
            animatorHandler.PlayTargetAnimation(weapon.OHLightAttacks[0], true);
            lastAttack = weapon.OHLightAttacks[0];
        }
    }

    public void HandleHeavyAttack(WeaponItem weapon)
    {
        if (weapon != null)
        {
            animatorHandler.PlayTargetAnimation(weapon.OHHeavyAttacks[0], true);
            lastAttack = weapon.OHHeavyAttacks[0];
        }
    }
}
