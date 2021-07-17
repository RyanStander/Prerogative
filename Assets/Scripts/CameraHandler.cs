using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraHandler : MonoBehaviour
{
    [SerializeField]
    private Transform targetTransform, //The transform the camera goes to
        cameraTransform, //Transform of the camera
        cameraPivotTransform;//Transform of the camera pivot, how the camera turns on the swivel
    private Transform myTransform;//transform of the game object
    private Vector3 cameraTransformPosition, cameraFollowVelocity = Vector3.zero;//position of the camera transform
    private LayerMask ignoreLayers; //used for camera's collision with objects in the world

    public static CameraHandler singleton;

    [SerializeField] private float lookSpeed = 0.1f, followSpeed = 0.1f, pivotSpeed = 0.03f;

    private float targetPosition, defaultPosition, lookAngle, pivotAngle;
    [SerializeField] private float minimumPivot = -35, maximumPivot = 35;

    [SerializeField] private float cameraSphereRadius = 0.2f, cameraCollisionOffSet = 0.2f, minimumCollisionOffset = 0.2f;
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        singleton = this;
        myTransform = transform;
        defaultPosition = cameraTransform.localPosition.z;
        ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);
        //targetTransform = FindObjectOfType<PlayerManager>().transform;
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
            Debug.Log("collided");
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
}

