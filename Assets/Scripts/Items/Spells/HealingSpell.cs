using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spells/Healing Spell")]
public class HealingSpell : SpellItem
{
    public int healAmount;

    public override void AttemptToCastSpell(PlayerAnimatorManager animatorManager,PlayerStats playerStats)
    {
        base.AttemptToCastSpell(animatorManager, playerStats);

        GameObject instantiatedWarmUpSpellFX = Instantiate(spellWindUpFX, animatorManager.transform);
        animatorManager.PlayTargetAnimation(spellAnimation, true);
    }

    public override void SuccessfullyCastSpell(PlayerAnimatorManager animatorManager, PlayerStats playerStats)
    {
        base.SuccessfullyCastSpell(animatorManager, playerStats);

        GameObject instantiatedSpellFX = Instantiate(spellCastFX, animatorManager.transform);
        playerStats.ReceiveHealing(healAmount);       
    }
}
