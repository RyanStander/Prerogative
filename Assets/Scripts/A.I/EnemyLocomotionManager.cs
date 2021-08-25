using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyLocomotionManager : MonoBehaviour
{
    private EnemyManager enemyManager;
    private EnemyAnimatorManager enemyAnimatorManager;
    private NavMeshAgent navmeshAgent;

    public Rigidbody enemyRigidBody;
    public LayerMask detectionLayer;

    public float distanceFromTarget;
    public float stoppingDistance = 1f;

    public float rotationSpeed = 15;

    private void Awake()
    {
        enemyManager = GetComponent<EnemyManager>();
        enemyAnimatorManager = GetComponentInChildren<EnemyAnimatorManager>();
        navmeshAgent = GetComponentInChildren<NavMeshAgent>();
        enemyRigidBody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        navmeshAgent.enabled = false;
        enemyRigidBody.isKinematic = false;
    }

    public void HandleMoveToTarget()
    {
        if (enemyManager.isPerformingAction)
            return;

        Vector3 targetDirection = enemyManager.currentTarget.transform.position - transform.position;
        distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, transform.position);
        float viewableAngle = Vector3.Angle(targetDirection, transform.forward);

        //if performing an action, stop movement
        if (enemyManager.isPerformingAction)
        {
            enemyAnimatorManager.anim.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
            navmeshAgent.enabled = false;
        }
        else
        {
            if (distanceFromTarget > stoppingDistance)
            {
                enemyAnimatorManager.anim.SetFloat("Vertical", 1, 0.1f, Time.deltaTime);
            }
            else if (distanceFromTarget <= stoppingDistance)
            {
                enemyAnimatorManager.anim.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
            }
        }

        HandleRotateTowardsTarget();

        //Resets nav mesh agent
        navmeshAgent.transform.localPosition = Vector3.zero;
        navmeshAgent.transform.localRotation = Quaternion.identity;
    }

    private void HandleRotateTowardsTarget()
    {
        //nav mesh agent always rotates towards what is the correct path. With manual it will rotate directly towards target, ignoring obstacles. Manual rotation
        //serves the function of allowing enemy to attack at a target without caring for obstructions.
        //Rotate manually
        if (enemyManager.isPerformingAction)
        {
            Vector3 direction = enemyManager.currentTarget.transform.position - transform.position;
            direction.y = 0;
            direction.Normalize();

            if (direction==Vector3.zero)
            {
                direction = transform.forward;
            }

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed/Time.deltaTime);
        }
        //Rotate with pathfinding (navmesh)
        else
        {
            Vector3 relativeDirection = transform.InverseTransformDirection(navmeshAgent.desiredVelocity);
            Vector3 targetVelocity = enemyRigidBody.velocity;

            navmeshAgent.enabled = true;
            navmeshAgent.SetDestination(enemyManager.currentTarget.transform.position);
            enemyRigidBody.velocity = targetVelocity;
            transform.rotation = Quaternion.Slerp(transform.rotation, navmeshAgent.transform.rotation, rotationSpeed / Time.deltaTime);
        }
    }
}
