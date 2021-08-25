using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasLookAtCamera : MonoBehaviour
{
    [SerializeField] private Canvas canvas;

    private void Update()
    {
        if (canvas!=null)
        {
            canvas.transform.LookAt(Camera.main.transform);
        }
    }
}
