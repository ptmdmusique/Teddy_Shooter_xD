using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class OrbitAroundObject : MonoBehaviour {

    public Transform target;
    public float speed;

    public bool constraintX = false;
    public bool constraintY = false;
    public bool constraintZ = false;
    public float startDelay = 0;

    private Vector3 initialAngle;
    private Vector3 initialPosition;
    public bool canMove = false;
	// Use this for initialization
	void Start () {
        initialAngle = transform.eulerAngles;
        initialPosition = transform.position;
        Invoke("ChangeCanMove", startDelay);
        
    }
	
	// Update is called once per frame
	void Update () {
        OrbitAround();
	}

    void OrbitAround() {
        if (canMove == false) {
            return;
        }
        //Around the center => rotation axis is the normal vector of (target.position - transform.position) (initial)
        transform.RotateAround(target.transform.position, Vector3.Cross(target.position - initialPosition, Vector3.forward) , speed * Time.deltaTime);
        //transform.RotateAround(target.transform.position, target.position - transform.position, speed * Time.deltaTime);

        if (constraintX == false) {
            initialAngle.x = transform.eulerAngles.x;
        }

        if (constraintY == false) {
            initialAngle.y = transform.eulerAngles.y;
        }

        if (constraintZ == false) {
            initialAngle.z = transform.eulerAngles.z;
        }

        transform.eulerAngles = initialAngle;
    }
    void ChangeCanMove() {
        canMove = !canMove;
    }
}
