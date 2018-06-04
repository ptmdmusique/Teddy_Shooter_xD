using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour {

    public Transform target;
    public bool useOffset = false;
    public float smoothFactor = 0;

    private Vector3 offset = Vector3.zero;

    private void Start() {
        CalculateOffset();
    }

    void CalculateOffset() {
        if (useOffset == true) {
            offset = transform.position - target.position;
        }
        else {
            ResetOffset();
        }
    }
    void ResetOffset() {
        offset = Vector3.zero;
    }

    private void FixedUpdate() {
        if (smoothFactor == 0) {
            transform.position = offset + target.position;
        }
        else {
            transform.position = Vector3.Lerp(transform.position, target.position, smoothFactor * Time.deltaTime);  
        }
    }
}
