using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finish : MonoBehaviour
{
    //declare your particle system here
    public ParticleSystem spiral;
    public Respawn respawnScript;
    private bool notTriggered = true;
    void OnTriggerEnter(Collider other)
    {
        if(!notTriggered) return;
        notTriggered = false;
        other.gameObject.GetComponent<Respawn>().spawnPoint = this.gameObject.transform.position;
        spiral.Play();
        respawnScript.TimerStop();
    }
}