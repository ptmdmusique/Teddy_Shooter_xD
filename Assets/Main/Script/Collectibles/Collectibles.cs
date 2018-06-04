using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectibles : GeneralObject {

    public bool forPlayerOnly = true;
    public Vector2 defaultMoveDir = Vector2.zero;
    public float speed = 10;

	// Use this for initialization
	public override void Awake () {
        base.Awake();

        Vector2 moveVector = defaultMoveDir;
        if (moveVector == Vector2.zero) {
            //If there is no move vector specified
            if (myAllianceState == AllianceState.Ally) {
                //Move down, toward the player
                moveVector = Vector2.down;
            } else {
                //Move up
                moveVector = Vector2.up;
            }
        }
        myRb.velocity = moveVector * speed;
	}

    public void OnTriggerEnter2D(Collider2D collision) {
        //Only be collected if has the same alliance state or it is the player
        bool condition1 = forPlayerOnly == true && collision.name == "Player";
        bool condition2 = false;
        GeneralObject colScript = collision.GetComponent<GeneralObject>();
        if (colScript != null) {
            condition2 = colScript.myAllianceState == myAllianceState && forPlayerOnly == false;
        }

        if (condition1 == true || condition2 == true) { 
            OnCollected(collision.transform);
        }
    }

    public virtual void OnCollected(Transform target) {
        Destroy(gameObject);
    }
}
