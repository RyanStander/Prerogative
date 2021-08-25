using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : CharacterManager
{
    private EnemyLocomotionManager enemyLocomotionManager;
    private EnemyAnimatorManager enemyAnimatorManager;
    public bool isPerformingAction;

    public EnemyActionAttack[] enemyAttacks;
    public EnemyActionAttack currentAttack;

    [Header("A.I Settings")]
    public float detectionRadius=20;
    
    //The higher, and lower, respectively these angles are, the greater detection field of view (eye sight)
    [Range(0,180)]public float maximumDetectionAngle = 50, minimumDetectionAngle = -50;

    public float currentRecoveryTime = 0;
    private void Awake()
    {
        enemyLocomotionManager = GetComponent<EnemyLocomotionManager>();
        enemyAnimatorManager = GetComponentInChildren<EnemyAnimatorManager>();
    }

    private void Update()
    {
        HandleRecoveryTimer();
    }

    private void FixedUpdate()
    {
        HandleCurrentAction();
    }

    private void HandleCurrentAction()
    {
        if (enemyLocomotionManager.currentTarget != null)
        {
            enemyLocomotionManager.distanceFromTarget = Vector3.Distance(enemyLocomotionManager.currentTarget.transform.position, transform.position);
        }
        
        if (enemyLocomotionManager.currentTarget==null)
        {
            enemyLocomotionManager.HandleDetection();
        }
        else if (enemyLocomotionManager.distanceFromTarget>enemyLocomotionManager.stoppingDistance)
        {
            enemyLocomotionManager.HandleMoveToTarget();
        }
        else if (enemyLocomotionManager.distanceFromTarget <= enemyLocomotionManager.stoppingDistance)
        {
            AttackTarget();
        }
    }

    private void HandleRecoveryTimer()
    {
        if (currentRecoveryTime>0)
        {
            currentRecoveryTime -= Time.deltaTime;
        }

        if (isPerformingAction)
        {
            if (currentRecoveryTime <=0)
            {
                isPerformingAction = false;
            }
        }
    }

    #region Attacks
    private void AttackTarget()
    {
        if (isPerformingAction)
            return;

        if (currentAttack==null)
        {
            GetNewAttack();
        }
        else
        {
            isPerformingAction = true;
            currentRecoveryTime = currentAttack.recoveryTime;
            enemyAnimatorManager.PlayTargetAnimation(currentAttack.actionAnimation, true);
            currentAttack = null;
        }
    }

        private void GetNewAttack()
    {
        Vector3 targetDirection = enemyLocomotionManager.currentTarget.transform.position - transform.position;
        float viewableAngle = Vector3.Angle(targetDirection, transform.forward);
        enemyLocomotionManager.distanceFromTarget = Vector3.Distance(enemyLocomotionManager.currentTarget.transform.position, transform.position);

        int maxScore = 0;

        for (int i = 0; i < enemyAttacks.Length; i++)
        {
            EnemyActionAttack enemyActionAttack = enemyAttacks[i];

            if(enemyLocomotionManager.distanceFromTarget<=enemyActionAttack.maximumDistanceNeededToAttack
                && enemyLocomotionManager.distanceFromTarget >= enemyActionAttack.minimumAttackAngle)
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

            if (enemyLocomotionManager.distanceFromTarget <= enemyActionAttack.maximumDistanceNeededToAttack
                && enemyLocomotionManager.distanceFromTarget >= enemyActionAttack.minimumAttackAngle)
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
    #endregion
}