using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectShake : MonoBehaviour {

    public bool isShaking = false;
    public float time = 0.5f;
    public float tumble = 0.1f;
    public bool useTarget = false;          //Do we use any target or we need to shake the object itself?
    public Transform target = null;
    private Vector2 initialPos;

    private void Awake() {
        if (target == null) {
            initialPos = transform.position;
        } else {
            initialPos = target.position;
        }
    }

    // Update is called once per frame
    void Update () {
        if (isShaking == false) {
            return;
        }

        if (isShaking == true) {
            Vector3 newPos = initialPos + Random.insideUnitCircle * tumble;
            if (useTarget == false) { 
                transform.position = newPos;
            } else {
                if (target != null) { 
                    target.position = newPos;
                }
            }
        }
    }

    public void StartShaking() {
        isShaking = true;
        Invoke("ReturnBack", time);
    }

    public void StartShaking(float time) {
        isShaking = true;
        Invoke("ReturnBack", time);
    }

    public void ReturnBack() {
        isShaking = false;
        if (useTarget == false) { 
            transform.position = initialPos;
        } else {
            if (target != null) {
                target.position = initialPos;
            }
        }
    }
}
