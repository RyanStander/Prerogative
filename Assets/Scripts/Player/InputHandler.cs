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
    public bool menuInput,inventoryInput,equipmentInput;

    //Quickslot inputs (item Selection)
    public bool dPadUp,dPadDown,dPadRight,dPadLeft;

    //use flags to know when its already in the process
    //combat flags
    public bool rollFlag, sprintFlag,comboFlag;
    public float rollInputTimer;

    //menu flags
    public bool menuFlag, inventoryFlag, equipmentFlag;

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

            InputsInitialize();


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
        
        //Menu inputs
        HandleMenuInput();
        HandleInventoryInput();
        HandleEquipmentInput();
    }

    public void MoveInput(float delta)
    {
        horizontal = movementInput.x;
        vertical = movementInput.y;
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
        mouseX = cameraInput.x;
        mouseY = cameraInput.y;
    }

    private void InputsInitialize()
    {
        //combat inputs
        inputActions.PlayerActions.LightAttack.performed += i => lightAttackInput = true;
        inputActions.PlayerActions.HeavyAttack.performed += i => heavyAttackInput = true;
        inputActions.PlayerActions.Jump.performed += i => jumpInput = true;
        inputActions.PlayerActions.Interact.performed += i => interactInput = true;

        //quickslot inputs
        inputActions.QuickSlots.DPadRight.performed += i => dPadRight = true;
        inputActions.QuickSlots.DPadLeft.performed += i => dPadLeft = true;

        //menu inputs
        inputActions.UIInputs.Menu.performed += i => menuInput = true;
        inputActions.UIInputs.Inventory.performed += i => inventoryInput = true;
        inputActions.UIInputs.Equipment.performed += i => equipmentInput = true;
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
        if (dPadRight)
        {
            playerInventory.ChangeRightWeapon();
        }
        else if (dPadLeft)
        {
            playerInventory.ChangeLeftWeapon();
        }
    }

    private void HandleMenuInput() 
    {
        if (menuInput)
        {
            if (inventoryFlag || equipmentFlag)
            {
                uiManager.CloseAllSecondaryWindows();
                uiManager.CloseSelectWindow();
                uiManager.OpenHUDWindow();
                SetAllSecondaryMenuFlagsToFalse();
                menuFlag = false;
            }
            else
            {
                menuFlag = !menuFlag;
                if (menuFlag)
                {
                    uiManager.OpenSelectWindow();
                    uiManager.CloseAllSecondaryWindows();
                    SetAllSecondaryMenuFlagsToFalse();
                    uiManager.CloseHUDWindow();
                }
                else
                {
                    uiManager.CloseSelectWindow();
                    uiManager.OpenHUDWindow();
                }
            }
        }
    }

    private void HandleInventoryInput()
    {
        if (inventoryInput)
        {
            inventoryFlag = !inventoryFlag;
            if (inventoryFlag)
            {
                uiManager.OpenWeaponInventoryWindow();
                uiManager.CloseSelectWindow();
                uiManager.CloseHUDWindow();
                menuFlag = false;
            }
            else
            {
                uiManager.CloseWeaponInventoryWindow();
                uiManager.CloseSelectWindow();
                uiManager.OpenHUDWindow();
            }
        }
    }

    private void HandleEquipmentInput()
    {
        if (equipmentInput)
        {
            equipmentFlag = !equipmentFlag;
            if (equipmentFlag)
            {
                uiManager.OpenEquipmentWindow();
                uiManager.CloseSelectWindow();
                uiManager.CloseHUDWindow();
                menuFlag = false;
            }
            else
            {
                uiManager.CloseEquipmentWindow();
                uiManager.CloseSelectWindow();
                uiManager.OpenHUDWindow();
            }
        }
    }



    //Used when closing all secondary menus without using the direct inputs to avoid buggs
    private void SetAllSecondaryMenuFlagsToFalse()
    {
        inventoryFlag = false;
        equipmentFlag = false;
    }
}
