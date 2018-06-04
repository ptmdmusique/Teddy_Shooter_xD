using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumericStatDisplayer : MonoBehaviour {

    public Text targetText;
    public float displayedValue;

    public float pointAnimDurationSec = 1f;         //Animation duration
    public float pointAnimTimer = 0f;               //Lerp's start point
    
    float savedDisplayedScore = 0;                  //Saved value for the start point of the lerp

    public void AddPoints(float points) {
        // A
        // what if you get more points before last points finished animating ?
        // start the animation again but from the score that was already being shown
        // --> no sudden jump in score animation
        savedDisplayedScore = displayedValue;
        // B
        // the player instantly has these points so nothng gets 
        // messed up if e.g. level ends before score animation finishes
        displayedValue += points;
        // Lerp gets a new end point
        pointAnimTimer = 0f;
    }

    void Update() {
        pointAnimTimer += Time.deltaTime;
        float prcComplete = pointAnimTimer / pointAnimDurationSec;
        // don't modify the start and end values here 
        // prcComplete will grow linearly but if you change the start/end points
        // it will add a cumulating error
        if (targetText != null) { 
            targetText.text = Mathf.Lerp(savedDisplayedScore, displayedValue, prcComplete).ToString("F0");
        }
    }

    private void Start() {
        if (targetText == null) {
            targetText = transform.Find("Stat").GetComponent<Text>();
        }
    }
}
