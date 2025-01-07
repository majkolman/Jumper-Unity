using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    //declare your particle system here
    public ParticleSystem spiral;
    void OnTriggerEnter(Collider other)
    {
        //Debug.Log(this.gameObject.transform.position);
        // sets the spawnpoint to the checkpoint's position
        other.gameObject.GetComponent<Respawn>().spawnPoint = this.gameObject.transform.position;
        //Debug.Log(other.gameObject.GetComponent<Respawn>().spawnPoint);
        spiral.Play();
    }
}
