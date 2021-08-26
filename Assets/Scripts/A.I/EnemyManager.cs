using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : CharacterManager
{
    private EnemyLocomotionManager enemyLocomotionManager;
    private EnemyAnimatorManager enemyAnimatorManager;
    private EnemyStats enemyStats;
    
    public NavMeshAgent navmeshAgent;
    public State currentState;
    public CharacterStats currentTarget;
    public Rigidbody enemyRigidBody;

    public bool isPerformingAction;
    public float distanceFromTarget;
    public float rotationSpeed = 15;
    public float maximumAttackRange = 1.5f;

    [Header("A.I Settings")]
    public float detectionRadius=20;
    
    //The higher, and lower, respectively these angles are, the greater detection field of view (eye sight)
    [Range(0,180)]public float maximumDetectionAngle = 50, minimumDetectionAngle = -50,viewableAngle;

    public float currentRecoveryTime = 0;
    private void Awake()
    {
        enemyLocomotionManager = GetComponent<EnemyLocomotionManager>();
        enemyAnimatorManager = GetComponentInChildren<EnemyAnimatorManager>();
        enemyStats = GetComponent<EnemyStats>();
        navmeshAgent = GetComponentInChildren<NavMeshAgent>();
        enemyRigidBody = GetComponent<Rigidbody>();
        
    }

    private void Start()
    {
        navmeshAgent.enabled = false;
        enemyRigidBody.isKinematic = false;
    }

    private void Update()
    {
        HandleRecoveryTimer();
    }

    private void FixedUpdate()
    {
        HandleStateMachine();
    }

    private void HandleStateMachine()
    {
        if (currentState!=null)
        {
            State nextState = currentState.Tick(this,enemyStats, enemyAnimatorManager);

            if (nextState!=null)
            {
                SwitchToNextState(nextState);
            }
        }
    }

    private void SwitchToNextState(State state)
    {
        currentState = state;
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


}
