using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatStanceState : State
{
    public AttackState attackState;
    public PursueTargetState pursueTargetState;
    public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager)
    {
        enemyManager.distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position);
        //potentially circle player or walk around them


        //Check for attack range
        //if in attack range return attack State
        if (enemyManager.currentRecoveryTime<=0 && enemyManager.distanceFromTarget<=enemyManager.maximumAttackRange)
        {
            return attackState;
        }
        //if the player runs out of range, retun the persue target state
        else if (enemyManager.distanceFromTarget > enemyManager.maximumAttackRange)
        {
            return pursueTargetState;
        }
        //if we are in a cool down after attacking, return this state and continue circling player
        else
        {
            return this;
        }
    }
}
