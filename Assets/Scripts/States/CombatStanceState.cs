using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatStanceState : State
{
    public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager)
    {
        //Check for attack range
        //potentially circle player or walk around them
        //if in attack range return attack State
        //if we are in a cool down after attacking, return this state and continue circling player
        //if the player runs out of range, retun the persue target state
        return this;
    }
}
