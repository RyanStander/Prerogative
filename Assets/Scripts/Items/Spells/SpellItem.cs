using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellItem : MonoBehaviour
{
    public GameObject spellWindUpFX;//The wind up of the spell before actually being cast (this exists so that a spell can be interupted)
    public GameObject spellCastFX;//the actual spell cast when successful
    public string spellAnimation;

    [Header("Spell Type")]
    public SpellType spellType;//the type of spell being cast, could be useful for having synergies (like fire damage type, or focussing a school of magic)

    [Header("Spell Description")]
    [TextArea]
    public string spellDescription;//description of what the spell do

    public virtual void AttemptToCastSpell()
    {
        Debug.Log("Attempting spell cast!");
    }

    public virtual void SuccessfullyCastSpell()
    {
        Debug.Log("Spell cast has succeeded");
    }



    public enum SpellType
    {
        spellType1,
        spellType2,
        spellType3
    }
}
