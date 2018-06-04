using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotationOverTime : MonoBehaviour {

    public float tumble;

    void Start() {
        GetComponent<Rigidbody2D>().angularVelocity = Random.Range(0.5f, 1) * tumble;
    }
}
