using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WindController : MonoBehaviour
{
    // Start is called before the first frame update
    private Playermovement playermovement;
    private ParticleSystem particleSystem;
    private ParticleSystem.EmissionModule emission;
    void Start()
    {  
        playermovement = GameObject.Find("Player").GetComponent<Playermovement>();
        particleSystem = GetComponent<ParticleSystem>();
        emission = particleSystem.emission;
    }

    // Update is called once per frame
    void Update()
    {
        float Emision = playermovement.currentSpeed;
        if(Emision > 15)
        { 
            emission.rateOverTime = Math.Min(Emision, 100);
        }else
        {
            emission.rateOverTime = 0;
        }
    }
}
