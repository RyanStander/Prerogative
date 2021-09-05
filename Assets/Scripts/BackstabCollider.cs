using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class BackstabCollider : MonoBehaviour
{
    public Transform backstabberStandPoint;

    private float timeElapsed, lerpDuration = 0.5f;
    private bool currentlyMovingToPosition = false;
    private Vector3 startPoint;

    public void LerpToPoint(Transform transformToMove)
    {
        startPoint = transformToMove.position;
        currentlyMovingToPosition = true;
        timeElapsed = 0;
        while (currentlyMovingToPosition)
        {
            if (timeElapsed < lerpDuration)
            {
                transformToMove.position = Vector3.Lerp(startPoint, backstabberStandPoint.position, timeElapsed / lerpDuration);

                timeElapsed += Time.deltaTime;
            }
            else
            {
                currentlyMovingToPosition = false;
            }
        }
    }
}
