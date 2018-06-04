using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collector : MonoBehaviour {

    [Header("Startup stuff")]
    public bool clearWhenStart = false;

    [Header("Info")]
    public int maxSize = 999;                   //Max size of our list
    //Destroy the entire child list
    public void DestroyAll() {
        foreach(Transform child in transform) {
            GeneralObject childScript = child.GetComponent<GeneralObject>();
            if (childScript != null) {
                childScript.Die();
            }
        }
    }
    public AllianceState myAllianceState;
    public bool setAllianceState = false;

    private void Start() {
        if (clearWhenStart == true) {
            foreach (Transform child in transform) {
                Destroy(child.gameObject);
            }
        }    
    }

    //Add new child
    public void AddChild(Transform newChild) {
        if (transform.childCount < maxSize) {
            if (newChild.parent != transform) { 
                newChild.parent = transform;
            }

            //Make sure the new enemy has the enemy tag
            GeneralObject newChildScript = newChild.GetComponent<GeneralObject>();
            if (newChildScript != null && setAllianceState == true) {
                newChildScript.myAllianceState = myAllianceState;
            }
        }
    }

    //Expand maxSize
    public void NewCapacity (int parm) {
        maxSize = parm;
    }
}
