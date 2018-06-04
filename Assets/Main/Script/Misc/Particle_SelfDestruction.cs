using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle_SelfDestruction : MonoBehaviour {

    private ParticleSystem myPS;

	void Start () {
        //The particle system will destroy itself after its duration
        myPS = GetComponent<ParticleSystem>();
        if (myPS != null) {
            Destroy(gameObject, myPS.main.duration);
        }
	}
	
	
}
