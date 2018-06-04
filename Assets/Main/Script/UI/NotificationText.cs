using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationText : MonoBehaviour {

    public enum TextState {
        Warning = 0,
        CheckpointReach = 1,
        EndLevel = 2,
        ObjectUnlock = 3
    }

    private Text myText;
    private Animator myAnim;
    private Coroutine changeStateCoroutine = null;
    //public float changeBackTime = 1f;

    // Use this for initialization
    void Awake () {
		if (myText == null) {
            myText = GetComponent<Text>();
        }

        if (myAnim == null) {
            myAnim = GetComponent<Animator>();
        }
	}
	
    public void ChangeState(string newText, float waitTime, TextState newState) {
        if (changeStateCoroutine == null) {
            changeStateCoroutine = StartCoroutine(ChangeStateCoroutine(newText, waitTime, newState));
        }

    }

    public IEnumerator ChangeStateCoroutine(string newText, float waitTime, TextState newState) {
        //Change to new state
        myAnim.SetInteger("State", (int) newState);
        myText.text = newText;
        //Wait
        yield return new WaitForSeconds(waitTime);
        //Set state back to default
        myAnim.SetInteger("State", -1);
        //Set coroutine back to default
        changeStateCoroutine = null;
    }
}
