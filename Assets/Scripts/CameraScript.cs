using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    public float smoothingCamera = 0.5f;
    [Range(0.0f, 10.0f)]
    public float rotationSpeedCamera = 2.0f;

    void LateUpdate()
    {
        if(Input.GetKey(KeyCode.Q))
            transform.Rotate(0, -rotationSpeedCamera, 0);
        if(Input.GetKey(KeyCode.E))
            transform.Rotate(0, rotationSpeedCamera, 0);
    }

}