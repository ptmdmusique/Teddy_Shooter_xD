using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAnimationState : MonoBehaviour {

    public string stateName = "";
    public bool isTrigger = false;
    public bool boolValue = false;

    private Animator myAnimator;
	// Use this for initialization
	void Start () {
        myAnimator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.T) == true) {
            if (isTrigger == true) {
                myAnimator.SetTrigger(stateName);
            } else {
                myAnimator.SetBool(stateName, boolValue);
                boolValue = !boolValue;
            }
        }
	}
}
