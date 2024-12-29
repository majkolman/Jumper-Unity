using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform cameraPosition;

    void Awake()
    {
        cameraPosition = GameObject.Find("CameraPos").transform;
    }
    void Update()
    {
        transform.position = cameraPosition.position;
    }
}
