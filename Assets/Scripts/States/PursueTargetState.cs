using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PursueTargetState : State
{
    public CombatStanceState combatStanceState;
    public IdleState idleState;
    public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager)
    {
        if (enemyManager.currentTarget == null)
            return idleState;

        float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position);

        if (enemyManager.isPerformingAction)
        {
            enemyAnimatorManager.anim.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
            return this;
        }

        Vector3 targetDirection = enemyManager.currentTarget.transform.position - enemyManager.transform.position;
        distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position);
        float viewableAngle = Vector3.Angle(targetDirection, enemyManager.transform.forward);

        if (distanceFromTarget > enemyManager.maximumAttackRange)
        {
            enemyAnimatorManager.anim.SetFloat("Vertical", 1, 0.1f, Time.deltaTime);
        }

        HandleRotateTowardsTarget(enemyManager);

        //Resets nav mesh agent
        enemyManager.navmeshAgent.transform.localPosition = Vector3.zero;
        enemyManager.navmeshAgent.transform.localRotation = Quaternion.identity;
        //Chase the target
        //If within attack range, return combat stance state
        //if target is out of range, return this state and continue to chase target

        if (distanceFromTarget<=enemyManager.maximumAttackRange)
        {
            return combatStanceState;
        }
        else
        {

        }
        return this;
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
