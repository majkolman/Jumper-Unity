using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCam : MonoBehaviour
{
    public GameObject thirdCam;
    public Cinemachine.CinemachineFreeLook thirdCamScript;

    //0-->First Person
    //1-->Third Person
    public int CamMode = 1;

    void Awake()
    {
        thirdCam = GameObject.Find("ThirdCam");
        thirdCamScript = thirdCam.GetComponent<Cinemachine.CinemachineFreeLook>();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (CamMode == 1)
            {
                CamMode = 0;
            }
            else
            {
                CamMode += 1;
            }
            StartCoroutine(CamChange());
        }
    }

    IEnumerator CamChange()
    {
        yield return new WaitForSeconds(0.01f);
        if (CamMode == 0)
        {
            thirdCamScript.Priority = 1;
            Invoke(nameof(DisableCam), 2f);
            
        }
        if (CamMode == 1)
        {
            thirdCam.SetActive(true);
            Invoke(nameof(SetPriority), 0.05f);
        }
    }

    void DisableCam()
    {
        thirdCam.SetActive(false);
    }

    void SetPriority()
    {
        thirdCamScript.Priority = 11;
    }

}
