using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableGameObjectAfterSeconds : MonoBehaviour
{
    [SerializeField] private int secondsToTurnOff=5;//The amount of time before the object turns off
    private float timeStamp;//The point at which the object was turned on
    private bool isTurningOff = false;//Will be set to true when the object turns off

    private void FixedUpdate()
    {
        CheckIfActive();
        DisableAfterSeconds();
    }

    private void CheckIfActive()
    {
        //if the object was just set active
        if (gameObject.activeInHierarchy && !isTurningOff)
        {
            timeStamp = Time.time + secondsToTurnOff;
            isTurningOff = true;
        }
    }

    private void DisableAfterSeconds()
    {
        if ((timeStamp <= Time.time) && isTurningOff)
        {
            isTurningOff = false;
            gameObject.SetActive(false);
        }
    }
}
