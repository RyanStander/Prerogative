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
        float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position);

        HandleRotateTowardsTarget(enemyManager);

        if (enemyManager.isPerformingAction)
            return combatStanceState;

        if (currentAttack!=null)
        {
            //if we are too close to the enemy to perform current attack, get new attack
            if (distanceFromTarget<currentAttack.minimumDistanceNeededToAttack)
            {
                return this;
            }
            //if we are close enough to attack, proceed
            else if (distanceFromTarget<currentAttack.maximumDistanceNeededToAttack)
            {
                //if our enemy is within our attacks viewable angle, attack
                if (viewableAngle<=currentAttack.maximumAttackAngle && viewableAngle>=currentAttack.minimumAttackAngle)
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
        Vector3 targetDirection = enemyManager.currentTarget.transform.position - enemyManager.transform.position;
        float viewableAngle = Vector3.Angle(targetDirection, enemyManager.transform.forward);
        float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position);
        distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position);

        int maxScore = 0;

        for (int i = 0; i < enemyAttacks.Length; i++)
        {
            EnemyActionAttack enemyActionAttack = enemyAttacks[i];

            if(distanceFromTarget<=enemyActionAttack.maximumDistanceNeededToAttack
                && distanceFromTarget >= enemyActionAttack.minimumAttackAngle)
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

            if (distanceFromTarget <= enemyActionAttack.maximumDistanceNeededToAttack
                && distanceFromTarget >= enemyActionAttack.minimumAttackAngle)
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

    private void HandleRotateTowardsTarget(EnemyManager enemyManager)
    {
        //nav mesh agent always rotates towards what is the correct path. With manual it will rotate directly towards target, ignoring obstacles. Manual rotation
        //serves the function of allowing enemy to attack at a target without caring for obstructions.
        //Rotate manually
        if (enemyManager.isPerformingAction)
        {
            Vector3 direction = enemyManager.currentTarget.transform.position - enemyManager.transform.position;
            direction.y = 0;
            direction.Normalize();

            if (direction == Vector3.zero)
            {
                direction = enemyManager.transform.forward;
            }

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            enemyManager.transform.rotation = Quaternion.Slerp(enemyManager.transform.rotation, targetRotation, enemyManager.rotationSpeed / Time.deltaTime);
        }
        //Rotate with pathfinding (navmesh)
        else
        {
            Vector3 relativeDirection = enemyManager.transform.InverseTransformDirection(enemyManager.navmeshAgent.desiredVelocity);
            Vector3 targetVelocity = enemyManager.enemyRigidBody.velocity;

            enemyManager.navmeshAgent.enabled = true;
            enemyManager.navmeshAgent.SetDestination(enemyManager.currentTarget.transform.position);
            enemyManager.enemyRigidBody.velocity = targetVelocity;
            enemyManager.transform.rotation = Quaternion.Slerp(enemyManager.transform.rotation, enemyManager.navmeshAgent.transform.rotation, enemyManager.rotationSpeed / Time.deltaTime);
        }
    }
}
