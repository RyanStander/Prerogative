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
    private CameraHandler cameraHandler;
    private UIManager uiManager;

    private Vector2 movementInput;
    private Vector2 cameraInput;

    //Combat inputs
    public bool rollInput,lightAttackInput,heavyAttackInput, jumpInput,interactInput,lockOnInput;
    public float lockOnTargetInput;

    //Menu Inputs
    public bool menuInput,inventoryInput,equipmentInput;

    //Quickslot inputs (item Selection)
    public bool dPadUp,dPadDown,dPadRight,dPadLeft;

    //use flags to know when its already in the process
    //combat flags
    public bool rollFlag, sprintFlag,comboFlag,lockOnFlag;
    public float rollInputTimer;

    //menu flags
    public bool menuFlag, inventoryFlag, equipmentFlag;

    //lock on cooldown
    private float lockOnSwapStamp, lockOnSwapCooldown=1;

    private void Awake()
    {
        playerCombatManager = GetComponent<PlayerCombatManager>();
        playerInventory = GetComponent<PlayerInventory>();
        playerManager = GetComponent<PlayerManager>();
        uiManager = FindObjectOfType<UIManager>();
        cameraHandler = FindObjectOfType<CameraHandler>();
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

            inputActions.PlayerMovement.LockOnTarget.performed += lockOnTargetInputActions => lockOnTargetInput = lockOnTargetInputActions.ReadValue<float>();

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
        //Combat inputs
        HandleMoveInput(delta);
        HandleRollInput(delta);
        HandleAttackInput(delta); 
        HandleLockOnInput();

        //Menu inputs
        HandleQuickslotInput();
        HandleMenuInput();
        HandleInventoryInput();
        HandleEquipmentInput();
    }

    public void HandleMoveInput(float delta)
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
        inputActions.PlayerActions.LockOn.performed += i => lockOnInput = true;

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

    private void HandleLockOnInput()
    {
        if (lockOnInput&&!lockOnFlag)
        {
            lockOnInput = false;
            cameraHandler.HandleLockOn();

            if (cameraHandler.nearestLockOnTarget!=null)
            {
                cameraHandler.currentLockOnTarget = cameraHandler.nearestLockOnTarget;
                lockOnFlag = true;
            }
        }
        else if (lockOnInput && lockOnFlag)
        {
            lockOnInput = false;
            lockOnFlag = false;
            cameraHandler.ClearLockOnTarget();
        }
        //Move to the next left target
        if (lockOnFlag && lockOnTargetInput<0)
        {
            cameraHandler.HandleLockOn();
            if (cameraHandler.leftLockTarget!=null)
            {
                if (lockOnSwapStamp<=Time.time)
                {
                    lockOnSwapStamp = Time.time + lockOnSwapCooldown;
                    cameraHandler.currentLockOnTarget = cameraHandler.leftLockTarget;
                }
            }
        }//Move to the next right target
        else if (lockOnFlag&&lockOnTargetInput>0)
        {
            cameraHandler.HandleLockOn();
            if (cameraHandler.rightLockTarget != null)
            {
                if (lockOnSwapStamp <= Time.time)
                {
                    lockOnSwapStamp = Time.time + lockOnSwapCooldown;
                    cameraHandler.currentLockOnTarget = cameraHandler.rightLockTarget;
                }
            }
        }
        cameraHandler.SetCameraHeight();
    }

    #region Menu Inputs
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
    #endregion


}
