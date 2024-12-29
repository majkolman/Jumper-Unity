using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Mousemovement : MonoBehaviour
{
    public float sensitivity;

    public Transform orientation;
    public Transform camHolder;
    public float TransitionSpeed = 0.5f;
    public float StartFov = 90f;
    public float WallRunFov = 100f;
    public float WallRunTilt = 5f;
    public ChangeCam cameraScript;

    float xRotation = 0f;
    float yRotation = 0f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (cameraScript.CamMode == 0)
        {
            float mouseX = Input.GetAxis("Mouse X") * sensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            yRotation += mouseX;

            camHolder.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
            orientation.rotation = Quaternion.Euler(0f, yRotation, 0f);
        }
    }

    public void DoTilt(float endValue)
    {
        transform.DOLocalRotate(new Vector3(0, 0, endValue), TransitionSpeed);
    }

    public void DoFov(float endValue)
    {
        GetComponent<Camera>().DOFieldOfView(endValue, TransitionSpeed);
    }
}
