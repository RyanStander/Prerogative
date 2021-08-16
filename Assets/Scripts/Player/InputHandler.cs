using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InputHandler : MonoBehaviour
{
    public float horizontal, vertical, moveAmount, mouseX, mouseY;

    private PlayerControls inputActions;
    private PlayerCombatManager playerCombatManager;
    private PlayerInventory playerInventory;
    private PlayerManager playerManager;
    private UIManager uiManager;

    private Vector2 movementInput;
    private Vector2 cameraInput;

    //Combat inputs
    public bool rollInput,lightAttackInput,heavyAttackInput, jumpInput,interactInput;

    //Menu Inputs
    public bool menuInput,inventoryInput;

    //Quickslot inputs (item Selection)
    public bool dPadUp,dPadDown,dPadRight,dPadLeft;

    //Know when its already in the process
    public bool rollFlag, sprintFlag,comboFlag;
    public float rollInputTimer;

    private void Awake()
    {
        playerCombatManager = GetComponent<PlayerCombatManager>();
        playerInventory = GetComponent<PlayerInventory>();
        playerManager = GetComponent<PlayerManager>();
        uiManager = FindObjectOfType<UIManager>();
    }

    private void OnEnable()
    {
        if (inputActions == null)
        {
            //if no input actions set, create one
            inputActions = new PlayerControls();
            //makes it so that input actions for movement/camera checked by movement  inputs and camera inputs
            inputActions.PlayerMovement.Movement.performed += movementInputActions => movementInput = movementInputActions.ReadValue<Vector2>();
            inputActions.PlayerMovement.Camera.performed += cameraInputActions => cameraInput = cameraInputActions.ReadValue<Vector2>();
            
            //attacking input actions
            inputActions.PlayerActions.LightAttack.performed += i => lightAttackInput = true;
            inputActions.PlayerActions.HeavyAttack.performed += i => heavyAttackInput = true;

            //jumping input action
            inputActions.PlayerActions.Jump.performed += i => jumpInput = true;
        }

        inputActions.Enable();
    }
    private void OnDisable()
    {
        inputActions.Disable();
    }

    public void TickInput(float delta)
    {
        MoveInput(delta);
        HandleRollInput(delta);
        HandleAttackInput(delta);
        HandleQuickslotInput();
        HandleInteractingButtonInput();
        HandleMenuInput();
        HandleInventoryInput();
    }

    public void MoveInput(float delta)
    {
        horizontal = movementInput.x;
        vertical = movementInput.y;
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
        mouseX = cameraInput.x;
        mouseY = cameraInput.y;
    }

    private void HandleRollInput(float delta)
    {
        rollInput = inputActions.PlayerActions.Roll.phase == UnityEngine.InputSystem.InputActionPhase.Started;
        sprintFlag = rollInput;
        if (rollInput)
        {
            rollInputTimer += delta;
        }
        else
        {
            if (rollInputTimer > 0 && rollInputTimer < 0.5f)
            {
                sprintFlag = false;
                rollFlag = true;
            }

            rollInputTimer = 0;
        }
    }

    private void HandleAttackInput(float delta)
    {
        if (lightAttackInput)
        {
            if (playerManager.canDoCombo)
            {
                comboFlag = true;
                playerCombatManager.HandleWeaponCombo(playerInventory.rightWeapon);
                comboFlag = false;
            }
            else
            {
                if (playerManager.isInteracting)
                    return;

                if (playerManager.canDoCombo)
                    return;
                playerCombatManager.HandleLightAttack(playerInventory.rightWeapon);
            }
        }

        if (heavyAttackInput)
        {
            if (playerManager.canDoCombo)
            {
                comboFlag = true;
                playerCombatManager.HandleWeaponCombo(playerInventory.rightWeapon);
                comboFlag = false;
            }
            else
            {
                if (playerManager.isInteracting)
                    return;

                if (playerManager.canDoCombo)
                    return;
                playerCombatManager.HandleHeavyAttack(playerInventory.rightWeapon);
            }

        }
    }

    private void HandleQuickslotInput()
    {
        inputActions.QuickSlots.DPadRight.performed += i => dPadRight = true;
        inputActions.QuickSlots.DPadLeft.performed += i => dPadLeft = true;
        if (dPadRight)
        {
            playerInventory.ChangeRightWeapon();
        }
        else if (dPadLeft)
        {
            playerInventory.ChangeLeftWeapon();
        }
    }

    private void HandleInteractingButtonInput()
    {
        inputActions.PlayerActions.Interact.performed += i => interactInput = true;
    }

    private void HandleMenuInput()
    {
        inputActions.UIInputs.Menu.performed += i => menuInput = true;

        if (menuInput)
        {
            uiManager.ToggleSelectWindow();
        }
    }

    private void HandleInventoryInput()
    {
        inputActions.UIInputs.Inventory.performed += i => inventoryInput = true;

        if (inventoryInput)
        {
            uiManager.ToggleInventory();
        }
    }
}
