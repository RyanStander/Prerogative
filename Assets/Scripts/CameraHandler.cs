using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraHandler : MonoBehaviour
{
    private InputHandler inputHandler;
    private PlayerManager playerManager;

    public Transform targetTransform, //The transform the camera goes to
        cameraTransform, //Transform of the camera
        cameraPivotTransform;//Transform of the camera pivot, how the camera turns on the swivel
    private Transform myTransform;//transform of the game object
    private Vector3 cameraTransformPosition, cameraFollowVelocity = Vector3.zero;//position of the camera transform
    public LayerMask ignoreLayers; //used for camera's collision with objects in the world
    public LayerMask environmentLayer;

    public static CameraHandler singleton;

    [SerializeField] private float lookSpeed = 0.1f, followSpeed = 0.1f, pivotSpeed = 0.03f;

    private float targetPosition, defaultPosition, lookAngle, pivotAngle;
    [SerializeField] private float minimumPivot = -35, maximumPivot = 35;

    [SerializeField] private float cameraSphereRadius = 0.2f, cameraCollisionOffSet = 0.2f, minimumCollisionOffset = 0.2f;
    [SerializeField] private float lockedPivotPosition = 2.25f, unlockedPivotPosition = 1.65f;

    public CharacterManager currentLockOnTarget;

    [SerializeField] private float maximumLockOnDistance=30;
    List<CharacterManager> availableTargets = new List<CharacterManager>();
    public CharacterManager nearestLockOnTarget,leftLockTarget,rightLockTarget;

    [Header("Debugging")] [SerializeField] private bool disableCursorLocking = false;
    private void Awake()
    {
        if (!disableCursorLocking)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        singleton = this;
        myTransform = transform;
        defaultPosition = cameraTransform.localPosition.z;
        ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10 | 1<<11 | 1<<12);

        inputHandler = FindObjectOfType<InputHandler>();
        playerManager = FindObjectOfType<PlayerManager>();
    }

    private void Start()
    {
        environmentLayer = LayerMask.NameToLayer("Environment");
    }

    public void FollowTarget(float delta)
    {
        //performs a lerp so that the camera moves smoothly to the target
        Vector3 targetPosition = Vector3.SmoothDamp
            (myTransform.position, targetTransform.position, ref cameraFollowVelocity, delta / followSpeed);
        myTransform.position = targetPosition;

        HandleCameraCollisions(delta);
    }

    public void HandleCameraRotation(float delta, float mouseXInput, float mouseYInput)
    {
        if (!inputHandler.lockOnFlag && currentLockOnTarget==null)
        {
            //limites the pivoting of the mouse
            lookAngle += (mouseXInput * lookSpeed) / delta;
            pivotAngle -= (mouseYInput * pivotSpeed) / delta;
            pivotAngle = Mathf.Clamp(pivotAngle, minimumPivot, maximumPivot);

            Vector3 rotation = Vector3.zero;
            rotation.y = lookAngle;
            Quaternion targetRotation = Quaternion.Euler(rotation);
            myTransform.rotation = targetRotation;

            rotation = Vector3.zero;
            rotation.x = pivotAngle;

            targetRotation = Quaternion.Euler(rotation);
            cameraPivotTransform.localRotation = targetRotation;
        }
        else if (currentLockOnTarget!=null)
        {
            //forcing camera to rotate towards the direction of the locked on target
            //float velocity = 0;

            Vector3 dir = currentLockOnTarget.transform.position - transform.position;
            dir.Normalize();
            dir.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = targetRotation;

            dir = currentLockOnTarget.transform.position - cameraPivotTransform.position;
            dir.Normalize();

            targetRotation = Quaternion.LookRotation(dir);
            Vector3 eulerAngle = targetRotation.eulerAngles;
            eulerAngle.y = 0;
            cameraPivotTransform.localEulerAngles = eulerAngle;
        }
        else
        {
            HandleLockOn();
            currentLockOnTarget = nearestLockOnTarget;
            if (availableTargets.Count==0)
            {
                inputHandler.lockOnFlag = false;
            }
        }
    }

    private void HandleCameraCollisions(float delta)
    {
        targetPosition = defaultPosition;
        RaycastHit hit;
        Vector3 direction = cameraTransform.position - cameraPivotTransform.position;
        direction.Normalize();
        //raycast a sphere that goes around the camera, if it intercects with any colliders, return true
        if (Physics.SphereCast(cameraPivotTransform.position, cameraSphereRadius, direction, out hit,
            Mathf.Abs(targetPosition), ignoreLayers))
        {
            //if it intersects, set target position to where it would not collide with the object
            float distance = Vector3.Distance(cameraPivotTransform.position, hit.point);
            targetPosition = -(distance - cameraCollisionOffSet);
        }
        if (Mathf.Abs(targetPosition) < minimumCollisionOffset)
        {
            targetPosition = -minimumCollisionOffset;
        }

        cameraTransformPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, delta / 0.2f);
        cameraTransform.localPosition = cameraTransformPosition;
    }

    public void HandleLockOn()
    {
        float shortestDistance = Mathf.Infinity;
        float shortestDistanceOfLeftTarget = -Mathf.Infinity;
        float shortestDistanceOfRightTarget = Mathf.Infinity;
        availableTargets = new List<CharacterManager>();

        //Creates a sphere to check fo any collisions
        Collider[] colliders = Physics.OverlapSphere(targetTransform.position, 26);

        for (int i = 0; i < colliders.Length; i++)
        {
            CharacterManager character = colliders[i].GetComponent<CharacterManager>();

            if (character != null)
            {
                //Makes sure that the target is in the camera view to avoid locking onto targets behind camera
                Vector3 lockTargetDirection = character.transform.position - targetTransform.position;
                float distanceFromTarget = Vector3.Distance(targetTransform.position, character.transform.position);
                float viewableAngle = Vector3.Angle(lockTargetDirection, cameraTransform.forward);

                RaycastHit hit;
                //Prevents locking onto self, sets within view distance and makes sure its not too far from the player
                if (character.transform.root != targetTransform.transform.root && viewableAngle > -50 && viewableAngle < 50 && distanceFromTarget <= maximumLockOnDistance)
                {
                    if (Physics.Linecast(playerManager.lockOnTransform.position, character.transform.position,out hit)) 
                    {
                        Debug.DrawLine(playerManager.lockOnTransform.position, character.transform.position);

                        if (hit.transform.gameObject.layer==environmentLayer)
                        {
                            //Cannot lock on target, object in the way
                        }
                        else
                        {
                            availableTargets.Add(character);
                        }
                    }
                }
            }
        }

        //search through available lock on targets
        for (int k = 0; k < availableTargets.Count; k++)
        {
            if (availableTargets[k]!=null)
            {
                float distanceFromTargets = Vector3.Distance(targetTransform.position, availableTargets[k].transform.position);

                //check for closest target
                if (distanceFromTargets < shortestDistance)
                {
                    shortestDistance = distanceFromTargets;
                    nearestLockOnTarget = availableTargets[k];
                }


                if (inputHandler.lockOnFlag)
                {

                    Vector3 relativeEnemyPosition = inputHandler.transform.InverseTransformPoint(availableTargets[k].transform.position);
                    var distanceFromTarget = relativeEnemyPosition.x;

                    if (relativeEnemyPosition.x <= 0.00f && distanceFromTarget > shortestDistanceOfLeftTarget && availableTargets[k] != currentLockOnTarget)
                    {
                        shortestDistanceOfLeftTarget = distanceFromTarget;
                        leftLockTarget = availableTargets[k];
                    }
                    else if (relativeEnemyPosition.x >= 0.00f && distanceFromTarget < shortestDistanceOfRightTarget && availableTargets[k] != currentLockOnTarget)
                    {
                        shortestDistanceOfRightTarget = distanceFromTarget;
                        rightLockTarget = availableTargets[k];
                    }
                }
            }
        }

    }

    public void ClearLockOnTarget()
    {
        //safety precaution, no touchy
        availableTargets.Clear();
        nearestLockOnTarget = null;
        currentLockOnTarget = null;
    }

    public void SetCameraHeight()
    {
        Vector3 velocity = Vector3.zero;
        Vector3 newLockedPosition = new Vector3(0, lockedPivotPosition);
        Vector3 newUnlockedPosition = new Vector3(0, unlockedPivotPosition);

        if (currentLockOnTarget!=null)
        {
            cameraPivotTransform.transform.localPosition = Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition, newLockedPosition, ref velocity, Time.deltaTime);
        }
        else
        {
            cameraPivotTransform.transform.localPosition = Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition, newUnlockedPosition, ref velocity, Time.deltaTime);
        }
    }
}

