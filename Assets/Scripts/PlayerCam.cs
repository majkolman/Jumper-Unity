using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Mousemovement : MonoBehaviour
{
    public float sensitivity = 2f;

    public Transform orientation;
    public Transform camHolder;
    public float WallRunTransitionSpeed = 0.5f;
    public float StartFov = 90f;
    public float WallRunFov = 110f;
    public float WallRunTilt = 5f;
    public float CameraShakeAmount = 2f;
    public float CameraShakeDuration = 0.1f;
    public float SlideFov = 110f;
    public float SlideTransitionSpeed = 1f;
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

    public void DoTilt(float endValue, float duration)
    {
        transform.DOLocalRotate(new Vector3(0, 0, endValue), duration);
    }

    public void DoFov(float endValue, float duration)
    {
        GetComponent<Camera>().DOFieldOfView(endValue, duration);
    }

    public void CameraShake()
    {
        transform.DOLocalRotate(new Vector3(CameraShakeAmount, 0, 0), CameraShakeDuration).OnComplete(() => transform.DOLocalRotate(new Vector3(0, 0, 0), CameraShakeDuration));
    }
}
