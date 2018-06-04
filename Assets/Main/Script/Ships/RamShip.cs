using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RamShip : Spaceship {

    [Header("Ram Stuff")]
    public float chargeTime;
    public float chargeMultiplier;
    public Coroutine chargeCoroutine;
    public Transform player;
    public float xRange = -1;                  //Tolerance Range to start charging

    private bool isCharging = false;

    new private void Start() {
        base.Start();    
        if (player == null) {
            player = GlobalState.FindPlayer();
        }

        if (xRange == -1){
            xRange = myCollider.bounds.extents.x;   //Get half of the collider
        }
    }

    new public void Update() {
        base.Update();

        canAttack = CheckAttackCondition();
        if (canAttack == true && TargetInFront() == true && isCharging == false) {
            StartCharge();
        }
    }

    public IEnumerator Charge() {
        myRb.velocity = Vector3.zero;

        yield return new WaitForSeconds(chargeTime);

        ChargeAttack();
    }

    public void ChargeAttack() {
        myRb.velocity = initialDir * mySpeed * chargeMultiplier;
    }

    public bool StartCharge() {
        if (chargeCoroutine == null) {
            isCharging = true;
            chargeCoroutine = StartCoroutine(Charge());
            return true;
        }
        return false;
    }

    public bool StopCharge() {
        if (chargeCoroutine != null) {
            //Do something to go back to normal 
            StopCoroutine(chargeCoroutine);
            chargeCoroutine = null;
            return true;
        }
        return false;
    }

    public bool TargetInFront() {
        //Cast a ray, if the ray hit the player then we are facing the player
        bool found = false;
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, xRange, transform.TransformDirection(0, 1, 0));
        foreach(RaycastHit2D curHit in hits) {
            GeneralObject hitScript = curHit.transform.GetComponent<GeneralObject>();
            if (hitScript != null && hitScript.myAllianceState != myAllianceState && hitScript.myTag.Contains("Projectile") == false && hitScript.myTag.Contains("Collectible") == false) {
                found = true;
                break;
            }
        }

        //Check whether the player is in front using the local forward direction
        return found;
    }

}
