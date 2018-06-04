using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GeneralControl : MonoBehaviour {

    public bool canBeUsed = true;

    private GameMaster gmScript;

    private void Awake() {
        gmScript = GetComponent<GameMaster>();    
    }

    private void Update() {
        if (canBeUsed == false) {
            return;
        }

        if (Input.GetKeyDown(KeyCode.P) == true) {
            if (Time.timeScale == 0) {
                gmScript.ChangeNotificationText("Unpaused!", 1f);
                Time.timeScale = 1;
            } else {
                gmScript.ChangeNotificationText("Paused!", 1f);
                Time.timeScale = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.R) == true) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (Input.GetKeyDown(KeyCode.Escape) == true) {
            Application.Quit();
        }
    }
}
