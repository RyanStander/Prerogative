using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerLocomotion : MonoBehaviour
{
    private CameraHandler cameraHandler;
    private PlayerManager playerManager;
    private Transform cameraObject;
    private InputHandler inputHandler;
    public Vector3 moveDirection;

    [HideInInspector]
    public Transform myTransform;
    [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;

    public new Rigidbody rigidbody;

    [Header("Ground & Air Detection Stats")]
    [SerializeField]
    float groundDetectionRayStartPoint = 0.5f, //where the raycast will begin
        minimumDistanceNeededToBeginFall = 1f,//distance needed for falling to begin
        groundDirectionRayDistance = 0.2f;//offset the raycast distance
    private LayerMask ignoreForGroundCheck;
    public float inAirTimer;

    [Header("Movement Stats")]
    [SerializeField] private float walkingSpeed = 3, movementSpeed = 5, 
        rotationSpeed = 10, sprintSpeed = 7, fallSpeed = 500;

    [Header("Jumping stats")]
    [Range(0,100)][SerializeField] private float jumpForce = 2f;


    private Vector3 normalVector;
    private Vector3 targetPosition;

    private void Awake()
    {
        cameraHandler = FindObjectOfType<CameraHandler>();
    }

    void Start()
    {
        playerManager = GetComponent<PlayerManager>();
        rigidbody = GetComponent<Rigidbody>();
        inputHandler = GetComponent<InputHandler>();
        playerAnimatorManager = GetComponentInChildren<PlayerAnimatorManager>();
        cameraObject = Camera.main.transform;
        myTransform = transform;
        playerAnimatorManager.Initialize();

        playerManager.isGrounded = true;
        ignoreForGroundCheck = ~(1 << 8 | 1 << 11);
    }

    #region Movement
    private void HandleRotation(float delta)
    {
        if (inputHandler.lockOnFlag)
        {
            if (inputHandler.sprintFlag || inputHandler.rollFlag)
            {
                Vector3 targetDirection = Vector3.zero;
                targetDirection = cameraHandler.cameraTransform.forward * inputHandler.vertical;
                targetDirection += cameraHandler.cameraTransform.right * inputHandler.horizontal;
                targetDirection.Normalize();
                targetDirection.y = 0;

                if (targetDirection == Vector3.zero)
                {
                    targetDirection = transform.forward;
                }

                Quaternion tr = Quaternion.LookRotation(targetDirection);
                Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, rotationSpeed * Time.deltaTime);

                transform.rotation = targetRotation;
            }
            else if (cameraHandler.currentLockOnTarget!=null)
            {
                Vector3 rotationDirection = moveDirection;
                rotationDirection = cameraHandler.currentLockOnTarget.transform.position - transform.position;
                rotationDirection.y = 0;
                rotationDirection.Normalize();
                Quaternion tr = Quaternion.LookRotation(rotationDirection);
                Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, rotationSpeed * Time.deltaTime);
                transform.rotation = targetRotation;
            }            
        }
        else
        {
            Vector3 targetDirection = Vector3.zero;
            float moveOverride = inputHandler.moveAmount;

            targetDirection = cameraObject.forward * inputHandler.vertical;
            targetDirection += cameraObject.right * inputHandler.horizontal;

            targetDirection.Normalize();
            targetDirection.y = 0;

            if (targetDirection == Vector3.zero)
            {
                targetDirection = myTransform.forward;
            }

            float rs = rotationSpeed;

            Quaternion tr = Quaternion.LookRotation(targetDirection);
            Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, tr, rs * delta);

            myTransform.rotation = targetRotation;
        }
    }

    private void CalculateMoveDirection()
    {
        //Calculates the direction of movement
        moveDirection = cameraObject.forward * inputHandler.vertical;
        moveDirection += cameraObject.right * inputHandler.horizontal;
        moveDirection.Normalize();
        moveDirection.y = 0;

        float speed = movementSpeed;

        if (inputHandler.sprintFlag && inputHandler.moveAmount > 0.5f)
        {
            speed = sprintSpeed;
            playerManager.isSprinting = true;
            moveDirection *= speed;
        }
        else
        {
            if (inputHandler.moveAmount < 0.5f)
            {
                moveDirection *= walkingSpeed;
                playerManager.isSprinting = false;
            }
            else
            {
                moveDirection *= speed;
                playerManager.isSprinting = false;
            }
        }
        //Amplifies the direction moving by the speed


        //Moves the object based on a plane
        if (!playerManager.isInAir)
        rigidbody.velocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
    }

    public void HandleRollingAndSprinting(float delta)
    {
        if (playerAnimatorManager.anim.GetBool("isInteracting"))
            return;

        if (inputHandler.rollFlag)
        {
            moveDirection = cameraObject.forward * inputHandler.vertical;
            moveDirection += cameraObject.right * inputHandler.horizontal;

            if (inputHandler.moveAmount > 0)
            {
                playerAnimatorManager.PlayTargetAnimation("Roll", true);
                moveDirection.y = 0;
                Quaternion rollRotation = Quaternion.LookRotation(moveDirection);
                myTransform.rotation = rollRotation;
            }
            else
            {
                playerAnimatorManager.PlayTargetAnimation("Backstep", true);
            }
        }
    }

    public void HandleFalling(float delta, Vector3 moveDirection)
    {
        playerManager.isGrounded = false;
        RaycastHit hit;
        Vector3 origin = myTransform.position;
        origin.y += groundDetectionRayStartPoint;

        if (Physics.Raycast(origin, myTransform.forward, out hit, 0.4f))
        {
            moveDirection = Vector3.zero;
        }

        if (playerManager.isInAir)
        {
            rigidbody.AddForce(-Vector3.up * fallSpeed);
            rigidbody.AddForce(moveDirection * fallSpeed / 10f);
        }

        Vector3 dir = moveDirection;
        dir.Normalize();
        origin = origin + dir * groundDirectionRayDistance;

        targetPosition = myTransform.position;

        Debug.DrawRay(origin, -Vector3.up * minimumDistanceNeededToBeginFall, Color.red, 0.1f, false);
        if (Physics.Raycast(origin, -Vector3.up, out hit, minimumDistanceNeededToBeginFall, ignoreForGroundCheck))
        {
            normalVector = hit.normal;
            Vector3 targetPosition = hit.point;
            playerManager.isGrounded = true;
            this.targetPosition.y = targetPosition.y;

            if (playerManager.isInAir)
            {
                if (inAirTimer > 0.5f)
                {
                    Debug.Log("You were in the air for " + inAirTimer);
                    playerAnimatorManager.PlayTargetAnimation("Land", true);
                    inAirTimer = 0;
                }
                else
                {
                    playerAnimatorManager.PlayTargetAnimation("Empty", false);
                    inAirTimer = 0;
                }
                playerManager.isInAir = false;
            }
        }
        else
        {
            if (playerManager.isGrounded)
            {
                playerManager.isGrounded = false;
            }

            if (!playerManager.isInAir)
            {
                if (!playerManager.isInteracting)
                {
                    playerAnimatorManager.PlayTargetAnimation("Falling", true);
                }

                Vector3 vel = rigidbody.velocity;
                vel.Normalize();
                rigidbody.velocity = vel * (movementSpeed / 2);
                playerManager.isInAir = true;
            }
        }

        if (playerManager.isGrounded)
        {
            if (playerManager.isInteracting || inputHandler.moveAmount > 0)
            {
                myTransform.position = Vector3.Lerp(myTransform.position, this.targetPosition, Time.deltaTime);
            }
            else
            {
                myTransform.position = this.targetPosition;
            }
        }
    } 

    public void HandleJumping()
    {
        if (playerManager.isInteracting)
            return;

        if (inputHandler.jumpInput)
        {
            //if player is moving, perform a jump with animations
            if (inputHandler.moveAmount > 0)
            {
                moveDirection = cameraObject.forward * inputHandler.vertical;
                moveDirection += cameraObject.right * inputHandler.horizontal;
                playerAnimatorManager.PlayTargetAnimation("Jump", false);
                moveDirection.y = 0;
                Quaternion jumpRotation = Quaternion.LookRotation(moveDirection);
                myTransform.rotation = jumpRotation;
                rigidbody.AddForce(Vector3.up*jumpForce,ForceMode.Impulse);
            }
        }
        //add stationary jump
    }

    public void HandleMovement(float deltaTime)
    {
        if (inputHandler.rollFlag)
            return;

        if (playerManager.isInteracting)
            return;

        CalculateMoveDirection();

        //handles animations based on movement
        if (inputHandler.lockOnFlag && !inputHandler.sprintFlag)
        {
            playerAnimatorManager.UpdateAnimatorValues(inputHandler.vertical, inputHandler.horizontal, playerManager.isSprinting);
        }
        else
        {
            playerAnimatorManager.UpdateAnimatorValues(inputHandler.moveAmount, 0, playerManager.isSprinting);
        }

        if (playerAnimatorManager.canRotate)
        {
            HandleRotation(deltaTime);
        }
    }

    #endregion


}

