using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatePlayerObject : MonoBehaviour
{
    public Transform orientation;
    public ChangeCam cameraScript;

    void Update()
    {
        transform.rotation = orientation.rotation;
    }
}
