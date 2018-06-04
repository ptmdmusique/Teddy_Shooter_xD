using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingBullet : Projectile {

    [Header("Homing Stuff")]
    public bool staticTarget = false;   //Only change rotation if our target moves
    public Transform target;
    public float rotateSpeed = 200f;
    public SpecialTarget mySpecialTarget;
    private SpecialTarget prevSpecialTarget = SpecialTarget.None;   //Hold the info about the previous special target

    public bool TargetInFront() {
        if (target != null) {
            if ((Vector2)target.position - myRb.position == (Vector2)transform.up) {
                //Directly infront
                return true;
            }
        }
        return false;
    }

    private void FixedUpdate() {
        //Only go to target when delay state is false
        if (isDelay == true) {
            return;
        }

        CheckTarget();

        if (target == null) {
            return;
        }

        Vector2 direction = (Vector2)target.position - myRb.position;
        direction.Normalize();

        //Use cross product to determine whether we should rotate left or right
        float rotateAmount = Vector3.Cross(direction, transform.up).z;

        myRb.angularVelocity = -rotateAmount * rotateSpeed;

        Launch(transform.up * mySpeed);
    }

    public void CheckTarget() {
        if (mySpecialTarget == SpecialTarget.None) {
            return;
        }

        if (mySpecialTarget == SpecialTarget.Mouse) {
            target = transform.Find("Target");
            target.position = GlobalState.GetMousePosition(mainCamera);
            return;
        }

        if (mySpecialTarget == SpecialTarget.Player) {
            target = GlobalState.FindPlayer();
            return;
        }
    }

    public void ChangeSpecialTarget(SpecialTarget newTarget) {
        prevSpecialTarget = mySpecialTarget;
        mySpecialTarget = newTarget;
    }

    
}
