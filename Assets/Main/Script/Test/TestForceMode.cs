using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestForceMode : MonoBehaviour {

    Rigidbody myRb;
	// Use this for initialization
	void Start () {
        myRb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.T) == true) {
            myRb.AddForce(new Vector2(0, 1), ForceMode.Impulse);
        }
        Debug.Log(GetComponent<Rigidbody2D>().velocity);
	}
}
