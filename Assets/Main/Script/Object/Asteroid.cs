using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : GeneralObject {

    [Header("Sprite list")]
    public Sprite[] mySpriteList;
    //public Vector2 initialDir = new Vector2(0, 0);
    
    public override void Awake() {
        base.Awake();
        RandomSprite();

        if (autoLaunch == true) {
            StartObject(initialDir, mySpeed);
        }
    }

    void RandomSprite() {
        if (mySpriteList.Length <= 0) {
            return;
        }

        //Switch to a random sprite among our sprite list
        int ranIndx = Random.Range(0, mySpriteList.Length - 1);
        mySR.sprite = mySpriteList[ranIndx];
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        GeneralObject otherScript = collision.GetComponent<GeneralObject>();
        if (otherScript != null) {
            if (otherScript.maxHealth <= -1) {
                return;
            }

            if (otherScript.myAllianceState != AllianceState.Neutral) {
                if (otherScript.myAllianceState != myAllianceState) {
                    if (otherScript.isInvincible == false || throughInvicible == true) {
                        //If the object can be damaged then substract the health
                        otherScript.TakeDamage(myDamage);

                        //Destroy the projectile afterwards
                        Die();
                    }
                }
            }
        }
    }

    public override void StartObject(Vector2 direction, float curSpeed) {
        //Move to random direction
        Vector2 velocity;
        //Go up as it is the default direction of ally bullet
        if (myAllianceState == AllianceState.Ally) {
            if (direction == new Vector2(0, 0)) {
                direction = Vector2.up;
            }
            velocity = direction * curSpeed;
        }
        else {
            //Default sprite direction is up... so we need to flip :D
            if (mySR != null) {
                mySR.flipY = true;
            }
            if (direction == new Vector2(0, 0)) {
                direction = Vector2.down;
            }
            velocity = direction * curSpeed;
        }

        if (myRb != null) { 
            myRb.velocity = velocity;
        }
    }
}
