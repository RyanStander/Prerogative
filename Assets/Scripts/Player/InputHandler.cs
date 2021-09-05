using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InputHandler : MonoBehaviour
{
    [HideInInspector] public float horizontal, vertical, moveAmount, mouseX, mouseY;

    private PlayerControls inputActions;
    private PlayerCombatManager playerCombatManager;
    private PlayerInventory playerInventory;
    private PlayerManager playerManager;
    private WeaponSlotManager weaponSlotManager;
    private CameraHandler cameraHandler;
    private UIManager uiManager;
    private AnimatorManager animatorManager;

    private Vector2 movementInput;
    private Vector2 cameraInput;

    //Combat inputs
    [HideInInspector] public bool rollInput,primaryAttackInput,primaryHoldAttackInput, jumpInput,interactInput,twoHandInput,lockOnInput;
    [HideInInspector] public float lockOnTargetInput;

    //Menu Inputs
    [HideInInspector] public bool menuInput,inventoryInput,equipmentInput;

    //Quickslot inputs (item Selection)
    [HideInInspector] public bool dPadUp,dPadDown,dPadRight,dPadLeft;

    //use flags to know when its already in the process
    //combat flags
    [HideInInspector] public bool rollFlag, sprintFlag,comboFlag,lockOnFlag,twoHandFlag;
    [HideInInspector] public float rollInputTimer;

    //menu flags
    [HideInInspector] public bool menuFlag, inventoryFlag, equipmentFlag;

    //lock on cooldown
    [HideInInspector] private float lockOnSwapStamp, lockOnSwapCooldown=1;

    //backstabbing raycast (raycasts a line out to check if it hits any backstab colliders
    [Tooltip("This should be placed a bit in front of the player's chest.")] public Transform criticalAttackRayCastStartPoint;

    private void Awake()
    {
        playerInventory = GetComponent<PlayerInventory>();
        playerManager = GetComponent<PlayerManager>();

        weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
        playerCombatManager = GetComponentInChildren<PlayerCombatManager>();
        animatorManager = GetComponentInChildren<AnimatorManager>();

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
        HandleTwoHandInput();
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
        inputActions.PlayerActions.PrimaryAttack.performed += i => primaryAttackInput = true;
        inputActions.PlayerActions.PrimaryHoldAttack.performed += i => primaryHoldAttackInput = true;
        inputActions.PlayerActions.Jump.performed += i => jumpInput = true;
        inputActions.PlayerActions.Interact.performed += i => interactInput = true;
        inputActions.PlayerActions.LockOn.performed += i => lockOnInput = true;
        inputActions.PlayerActions.TwoHandSwap.performed += i => twoHandInput = true;

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
        if (primaryAttackInput)
        {
            playerCombatManager.HandlePrimaryAttackAction();
        }

        if (primaryHoldAttackInput)
        {
            playerCombatManager.HandlePrimaryHeldAttackAction();
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

    private void HandleTwoHandInput()
    {
        if (twoHandInput)
        {
            twoHandInput = false;
            twoHandFlag = !twoHandFlag;

            if (twoHandFlag)
            {
                //Enable two handing
                weaponSlotManager.LoadWeaponOnSlot(playerInventory.rightWeapon, false);
            }
            else
            {
                //Disable two handing
                weaponSlotManager.LoadWeaponOnSlot(playerInventory.rightWeapon, false);
                weaponSlotManager.LoadWeaponOnSlot(playerInventory.leftWeapon, true);
            }
        }
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
