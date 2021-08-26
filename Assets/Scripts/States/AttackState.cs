using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State
{
    public CombatStanceState combatStanceState;
    public EnemyActionAttack[] enemyAttacks;
    public EnemyActionAttack currentAttack;
    public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager)
    {
        Vector3 targetDirection = enemyManager.currentTarget.transform.position - enemyManager.transform.position;
        float viewableAngle = Vector3.Angle(targetDirection, enemyManager.transform.forward);

        if (enemyManager.isPerformingAction)
            return combatStanceState;

        if (currentAttack!=null)
        {
            //if we are too close to the enemy to perform current attack, get new attack
            if (enemyManager.distanceFromTarget<currentAttack.minimumDistanceNeededToAttack)
            {
                return this;
            }
            //if we are close enough to attack, proceed
            else if (enemyManager.distanceFromTarget<currentAttack.maximumDistanceNeededToAttack)
            {
                //if our enemy is within our attacks viewable angle, attack
                if (enemyManager.viewableAngle<=currentAttack.maximumAttackAngle && enemyManager.viewableAngle>=currentAttack.minimumAttackAngle)
                {
                    if (enemyManager.currentRecoveryTime <= 0 && enemyManager.isPerformingAction == false)
                    {
                        enemyAnimatorManager.anim.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
                        enemyAnimatorManager.anim.SetFloat("Horizontal", 0, 0.1f, Time.deltaTime);
                        enemyAnimatorManager.PlayTargetAnimation(currentAttack.actionAnimation, true);
                        enemyManager.isPerformingAction = true;
                        enemyManager.currentRecoveryTime = currentAttack.recoveryTime;
                        currentAttack = null;
                        return combatStanceState;
                    }
                }
            }

        }
        else
        {
            GetNewAttack(enemyManager);
        }

        return combatStanceState;
        //Select one of our many attacks based on attack scores
        //If the selected attack is not able to be used because of bad angle or distance, select a new attack
        //if the attack is viable, stop our movement and attack our target
        //set our recovery timer to the attacks recovery time
        //return the combat stance
    }


    private void GetNewAttack(EnemyManager enemyManager)
    {
        Vector3 targetDirection = enemyManager.currentTarget.transform.position - transform.position;
        float viewableAngle = Vector3.Angle(targetDirection, transform.forward);
        enemyManager.distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, transform.position);

        int maxScore = 0;

        for (int i = 0; i < enemyAttacks.Length; i++)
        {
            EnemyActionAttack enemyActionAttack = enemyAttacks[i];

            if(enemyManager.distanceFromTarget<=enemyActionAttack.maximumDistanceNeededToAttack
                && enemyManager.distanceFromTarget >= enemyActionAttack.minimumAttackAngle)
            {
                if (viewableAngle<=enemyActionAttack.maximumAttackAngle
                    && viewableAngle>=enemyActionAttack.minimumAttackAngle)
                {
                    maxScore += enemyActionAttack.attackScore;
                }
            }
        }

        int randomValue = Random.Range(0, maxScore+1);
        int temporaryScore = 0;

        for (int i = 0; i < enemyAttacks.Length; i++)
        {
            EnemyActionAttack enemyActionAttack = enemyAttacks[i];

            if (enemyManager.distanceFromTarget <= enemyActionAttack.maximumDistanceNeededToAttack
                && enemyManager.distanceFromTarget >= enemyActionAttack.minimumAttackAngle)
            {
                if (viewableAngle <= enemyActionAttack.maximumAttackAngle
                    && viewableAngle >= enemyActionAttack.minimumAttackAngle)
                {
                    if (currentAttack != null)
                        return;

                    temporaryScore += enemyActionAttack.attackScore;

                    if (temporaryScore>randomValue)
                    {
                        currentAttack = enemyActionAttack;
                    }
                }
            }
        }
    }
}
