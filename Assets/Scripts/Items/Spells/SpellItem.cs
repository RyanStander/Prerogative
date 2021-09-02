using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellItem : Item
{
    public GameObject spellWindUpFX;//The wind up of the spell before actually being cast (this exists so that a spell can be interupted)
    public GameObject spellCastFX;//the actual spell cast when successful
    public string spellAnimation;

    public int magickaCost;

    [Header("Spell Type")]
    public SpellType spellType;//the type of spell being cast, could be useful for having synergies (like fire damage type, or focussing a school of magic)

    [Header("Spell Description")]
    [TextArea]
    public string spellDescription;//description of what the spell do

    public virtual void AttemptToCastSpell(PlayerAnimatorManager animatorManager, PlayerStats playerStats)
    {
        Debug.Log("Attempting spell cast!");
    }

    public virtual void SuccessfullyCastSpell(PlayerAnimatorManager animatorManager, PlayerStats playerStats)
    {
        Debug.Log("Spell cast has succeeded");
        playerStats.ConsumeMagicka(magickaCost);
    }



    public enum SpellType
    {
        healingAbility,
        spellType2,
        spellType3
    }
}
