using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbushState : State
{
    public bool isSleeping;
    public float detectionRadius = 2;
    public string sleepAnimation,wakeAnimation;

    public LayerMask detectionLayer;

    public PursueTargetState pursueTargetState;

    public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager)
    {

        if (isSleeping && enemyManager.isInteracting == false)
        {
            enemyAnimatorManager.PlayTargetAnimation(sleepAnimation, true);
        }

        #region Handle Target Detection

        Collider[] colliders = Physics.OverlapSphere(enemyManager.transform.position, detectionRadius, detectionLayer);

        for (int i = 0; i < colliders.Length; i++)
        {
            //get all character
            CharacterStats characterStats = colliders[i].transform.GetComponent<CharacterStats>();

            //If collided object has a character stats script
            if (characterStats!=null)
            {
                Vector3 targetsDirection = characterStats.transform.position - enemyManager.transform.position;
                float viewableAngle = Vector3.Angle(targetsDirection, enemyManager.transform.forward);

                //If the character is in front of the enemy
                if (viewableAngle>enemyManager.minimumDetectionAngle&&viewableAngle<enemyManager.maximumDetectionAngle)
                {
                    //temporary if
                    if (characterStats != GetComponentInParent<EnemyStats>())
                    {
                        enemyManager.currentTarget = characterStats;
                        isSleeping = false;
                        enemyAnimatorManager.PlayTargetAnimation(wakeAnimation, true);
                    }
                }
            }
        }

        #endregion

        #region Handle State Change

        //if a target has been spotted, swap to pursueing them
        if (enemyManager.currentTarget!=null)
        {
            return pursueTargetState;
        }
        //otherwise continue sleeping
        else
        {
            return this;
        }

        #endregion
    }
}
