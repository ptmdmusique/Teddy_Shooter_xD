using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToMouse : MonoBehaviour {

	public bool useCustomCamera = false;
    public Camera mainCamera;

    private void Start() {
        if(useCustomCamera == false) {
            mainCamera = Camera.main;
        } 
    }

    // Update is called once per frame
    void Update () {
        //Grab the current mouse position on the screen
        Vector3 mouseOnScreen = GlobalState.GetMousePosition(mainCamera);

        //Rotates toward the mouse
        transform.eulerAngles = new Vector3(transform.eulerAngles.x,
            transform.eulerAngles.y, 
            Mathf.Atan2((mouseOnScreen.y - transform.position.y), (mouseOnScreen.x - transform.position.x)) * Mathf.Rad2Deg - 90);

    }
}
