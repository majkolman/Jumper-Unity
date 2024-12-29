using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatePlayerObject : MonoBehaviour
{
    public Transform orientation;
    public ChangeCam cameraScript;

    void Awake()
    {
        orientation = GameObject.Find("Orientation").transform;
        cameraScript = GameObject.Find("CameraMonitor").GetComponent<ChangeCam>();
    }
    void Update()
    {
        transform.rotation = orientation.rotation;
    }
}
