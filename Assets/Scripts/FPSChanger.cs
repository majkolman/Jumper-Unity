using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSChanger : MonoBehaviour
{
    public int targetFPS = 60;
    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFPS;
    }
    
    void Update()
    {
        if(Application.targetFrameRate != targetFPS)
            Application.targetFrameRate = targetFPS;
    }
}
