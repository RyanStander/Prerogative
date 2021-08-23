using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : CharacterManager
{
    private EnemyLocomotionManager enemyLocomotionManager;
    private bool isPerformingAction;

    [Header("A.I Settings")]
    public float detectionRadius=20;
    
    //The higher, and lower, respectively these angles are, the greater detection field of view (eye sight)
    [Range(0,180)]public float maximumDetectionAngle = 50, minimumDetectionAngle = -50;
    private void Awake()
    {
        enemyLocomotionManager = GetComponent<EnemyLocomotionManager>();
    }

    private void Update()
    {
        HandleCurrentAction();
    }

    private void HandleCurrentAction()
    {
        if (enemyLocomotionManager)
        {
            enemyLocomotionManager.HandleDetection();
        }
    }
}
