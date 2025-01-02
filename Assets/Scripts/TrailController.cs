using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailController : MonoBehaviour
{
    private Playermovement playermovement;
    private ParticleSystem particleSystem;
    void Start()
    {
        playermovement = GameObject.Find("Player").GetComponent<Playermovement>();
        particleSystem = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if(playermovement.isGrounded && playermovement.currentSpeed > 1)
        {
            particleSystem.Play();
        }
        else
        {
            particleSystem.Stop();
        }
    }
}
