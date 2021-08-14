using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerManager : MonoBehaviour
{
    private InputHandler inputHandler;
    private Animator anim;
    private CameraHandler cameraHandler;
    private PlayerLocomotion playerLocomotion;

    [Header("Player Flags")]
    public bool isInteracting, isSprinting, isInAir, isGrounded,canDoCombo;

    // Start is called before the first frame update
    void Start()
    {
        cameraHandler = FindObjectOfType<CameraHandler>();
        inputHandler = GetComponent<InputHandler>();
        anim = GetComponentInChildren<Animator>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
    }


    // Update is called once per frame
    void Update()
    {
        float delta = Time.deltaTime;
        isInteracting = anim.GetBool("isInteracting");
        canDoCombo = anim.GetBool("canDoCombo");
        anim.SetBool("isInAir", isInAir);

        inputHandler.TickInput(delta);

        //handle animation based functions for locomotion
        playerLocomotion.HandleRollingAndSprinting(delta);
        playerLocomotion.HandleJumping();

        CheckForInteractableObject();
    }

    private void FixedUpdate()
    {
        float delta = Time.fixedDeltaTime;

        //Handle movement based functions
        playerLocomotion.HandleMovement(delta);
        playerLocomotion.HandleFalling(delta, playerLocomotion.moveDirection);
        
    }

    private void LateUpdate()
    {
        //Ensures inputs only happen once per frame
        inputHandler.rollFlag = false;
        inputHandler.lightAttackInput = false;
        inputHandler.heavyAttackInput = false;
        inputHandler.jumpInput = false;
        inputHandler.interactInput = false;
        inputHandler.d_Pad_Up = false;
        inputHandler.d_Pad_Down = false;
        inputHandler.d_Pad_Right = false;
        inputHandler.d_Pad_Left = false;

        isSprinting = inputHandler.rollInput;

        if (cameraHandler != null)
        {
            float delta = Time.fixedDeltaTime;
            cameraHandler.FollowTarget(delta);
            cameraHandler.HandleCameraRotation(delta, inputHandler.mouseX, inputHandler.mouseY);
        }

        if (isInAir)
        {
            playerLocomotion.inAirTimer = playerLocomotion.inAirTimer + Time.deltaTime;
        }
    }

    public void CheckForInteractableObject()
    {
        RaycastHit hit;

        if (Physics.SphereCast(transform.position, 0.3f, transform.forward, out hit, 1f, cameraHandler.ignoreLayers))
        {
            if (hit.collider.tag == "Interactable")
            {
                Interactable interactableObject = hit.collider.GetComponent<Interactable>();

                if (interactableObject != null)
                {
                    string interactableText = interactableObject.interactableText;
                    //Set the ui text to the interactable objects text
                    //set the text pop up to true

                    if (inputHandler.interactInput)
                    {
                        hit.collider.GetComponent<Interactable>().Interact(this);
                    }
                }
            }
        }
    }
}
