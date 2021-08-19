using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerManager : CharacterManager
{
    private InputHandler inputHandler;
    private Animator anim;
    private CameraHandler cameraHandler;
    private PlayerLocomotion playerLocomotion;
    
    private InteractableUI interactableUI;
    public GameObject interactableUIGameObject;
    public GameObject itemInteractableGameObject;

    [Header("Player Flags")]
    public bool isInteracting, isSprinting, isInAir, isGrounded,canDoCombo;

    // Start is called before the first frame update
    void Start()
    {
        cameraHandler = FindObjectOfType<CameraHandler>();
        inputHandler = GetComponent<InputHandler>();
        anim = GetComponentInChildren<Animator>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        interactableUI = FindObjectOfType<InteractableUI>();
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

        if (cameraHandler != null)
        {
            
        }

    }

    private void LateUpdate()
    {
        //Ensures inputs only happen once per frame

        //Combat Inputs
        inputHandler.rollFlag = false;
        inputHandler.lightAttackInput = false;
        inputHandler.heavyAttackInput = false;
        inputHandler.jumpInput = false;
        inputHandler.interactInput = false;
        inputHandler.lockOnTargetInput = 0;

        //Menu Inputs
        inputHandler.menuInput = false;
        inputHandler.inventoryInput = false;
        inputHandler.equipmentInput = false;

        //Item Selection Inputs
        inputHandler.dPadUp = false;
        inputHandler.dPadDown = false;
        inputHandler.dPadRight = false;
        inputHandler.dPadLeft = false;

        isSprinting = inputHandler.rollInput;

        if (isInAir)
        {
            playerLocomotion.inAirTimer = playerLocomotion.inAirTimer + Time.deltaTime;
        }

        if (cameraHandler != null)
        {
            float delta = Time.fixedDeltaTime;

            cameraHandler.FollowTarget(delta);

            cameraHandler.HandleCameraRotation(delta, inputHandler.mouseX, inputHandler.mouseY);
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
                    interactableUI.interactableText.text = interactableText;
                    interactableUIGameObject.SetActive(true);

                    if (inputHandler.interactInput)
                    {
                        hit.collider.GetComponent<Interactable>().Interact(this);
                    }
                }
            }
        }
        else
        {
            if (interactableUIGameObject != null)
            {
                interactableUIGameObject.SetActive(false);
            }

            if (itemInteractableGameObject!=null&& inputHandler.interactInput)
            {
                itemInteractableGameObject.SetActive(false);
            }
        }
    }
}
