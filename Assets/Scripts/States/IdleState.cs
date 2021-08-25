using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State
{
    public LayerMask detectionLayer;
    public PursueTargetState pursueTargetState;

    public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager)
    {
        #region Handle Enemy Target Detection
        Collider[] colliders = Physics.OverlapSphere(transform.position, enemyManager.detectionRadius, detectionLayer);

        //Searches through objects of a specific layer
        for (int i = 0; i < colliders.Length; i++)
        {
            //checks if it is a character/creature
            CharacterStats characterStats = colliders[i].transform.GetComponent<CharacterStats>();

            if (characterStats != null)
            {
                //Check for faction id

                //check if its within enemies view
                Vector3 targetDirection = characterStats.transform.position - transform.position;
                float viewableAngle = Vector3.Angle(targetDirection, transform.forward);

                if (viewableAngle > enemyManager.minimumDetectionAngle && viewableAngle < enemyManager.maximumDetectionAngle)
                {
                    //temporary if
                    if (characterStats != GetComponentInParent<EnemyStats>())
                    {
                        enemyManager.currentTarget = characterStats; 
                    }
                }
            }
        }
        #endregion

        #region Handle Switching To Next State
        if (enemyManager.currentTarget!=null)
        {
            return pursueTargetState;
        }
        else
        {
            return this;
        }
        #endregion
    }
}
