using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableGameObjectAfterSeconds : MonoBehaviour
{
    [SerializeField] private int secondsToTurnOff=5;//The amount of time before the object turns off
    private float timeStamp;//The point at which the object was turned on

    private void OnEnable()
    {
        timeStamp = Time.time + secondsToTurnOff;
    }

    private void FixedUpdate()
    {
        DisableAfterSeconds();
    }

    private void DisableAfterSeconds()
    {
        if (timeStamp <= Time.time)
        {
            gameObject.SetActive(false);
        }
    }
}
