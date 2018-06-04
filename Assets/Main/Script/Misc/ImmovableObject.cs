using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImmovableObject : MonoBehaviour {

    Vector3 initialPosition;
	void Awake () {
        initialPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = initialPosition;
	}
}
