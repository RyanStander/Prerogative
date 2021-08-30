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

        if (spellWindUpFX != null)
        {
            GameObject instantiatedWarmUpSpellFX = Instantiate(spellWindUpFX, animatorManager.transform.parent);
        }

        animatorManager.PlayTargetAnimation(spellAnimation, true);
    }

    public override void SuccessfullyCastSpell(PlayerAnimatorManager animatorManager, PlayerStats playerStats)
    {
        base.SuccessfullyCastSpell(animatorManager, playerStats);
        if (spellCastFX != null)
        {
            GameObject instantiatedSpellFX = Instantiate(spellCastFX, animatorManager.transform);
        }
        playerStats.ReceiveHealing(healAmount);       
    }
}
