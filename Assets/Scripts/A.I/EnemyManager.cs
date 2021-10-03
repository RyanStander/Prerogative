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

    public bool isPerformingAction,isInteracting;
    public float rotationSpeed = 15;
    public float maximumAttackRange = 1.5f;

    [Header("A.I Settings")]
    public float detectionRadius=20;

    //The higher, and lower, respectively these angles are, the greater detection field of view (eye sight)
    public float maximumDetectionAngle = 50, minimumDetectionAngle = -50;

    public float currentRecoveryTime = 0;
    private void Awake()
    {
        enemyAnimatorManager = GetComponentInChildren<EnemyAnimatorManager>();
        navmeshAgent = GetComponentInChildren<NavMeshAgent>();
        enemyStats = GetComponent<EnemyStats>();
        enemyRigidBody = GetComponent<Rigidbody>();
        enemyLocomotionManager = GetComponent<EnemyLocomotionManager>();


    }

    private void Start()
    {
        navmeshAgent.enabled = false;
        enemyRigidBody.isKinematic = false;
    }

    private void Update()
    {
        HandleRecoveryTimer();

        isInteracting = enemyAnimatorManager.anim.GetBool("isInteracting");
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
                if (enemyStats.isDead)
                    return;
                isPerformingAction = false;
            }
        }
    }


}
