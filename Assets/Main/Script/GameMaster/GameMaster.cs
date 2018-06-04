using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;

public class GameMaster : MonoBehaviour {

    public Transform myBorder;
    public BoxCollider2D borderCollider;
    public Vector2 xBoundary;
    public Vector2 yBoundary;
    public Vector2 xScreen;                 //X border of the screen
    public Vector2 yScreen;                 //Y border of the screen

    [Header("Collectors")]
    private Transform collectorMaster;
    public Transform objectsMaster;
    public Collector objectsMasterScript;
    public Transform collectiblesMaster;
    public Collector collectiblesMasterScript;
    public Transform alliesMaster;
    public Collector alliesMasterScript;

    [Header("UI")]
    private NotificationText notificationText;
    private Animator introOutroAnim;
    public Camera mainCamera;

    // Use this for initialization
    void Awake () {
        //Find the border
        myBorder = GameObject.Find("Border").transform;
        
        //Get the masters
        collectorMaster = GameObject.Find("Collector").transform;
        objectsMaster = collectorMaster.Find("Objects Master").transform;
        alliesMaster = collectorMaster.Find("Allies Master").transform;
        collectiblesMaster = collectorMaster.Find("Collectibles Master").transform;

        objectsMasterScript = objectsMaster.GetComponent<Collector>();
        alliesMasterScript = alliesMaster.GetComponent<Collector>();
        collectiblesMasterScript = collectiblesMaster.GetComponent<Collector>();

        notificationText = GameObject.Find("Notification Text").GetComponent<NotificationText>();
        introOutroAnim = GameObject.Find("Intro Outro").GetComponent<Animator>();

        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        xScreen = GlobalState.GetCameraXBound(mainCamera);
        yScreen = GlobalState.GetCameraYBound(mainCamera);

        if (myBorder != null) {
            borderCollider = myBorder.GetComponent<BoxCollider2D>();
            if (borderCollider != null) {
                //Adjust the collider size as 1.5 size of the screen size (* 2 because size = 2 * xScreen)
                borderCollider.size = new Vector2(xScreen.y * 1.7f * 2, yScreen.y * 1.3f * 2);
                xBoundary.x = myBorder.transform.position.x - borderCollider.size.x / 2 + 1;
                xBoundary.y = myBorder.transform.position.x + borderCollider.size.x / 2 - 1;

                yBoundary.x = myBorder.transform.position.y - borderCollider.size.y / 2 + 1;
                yBoundary.y = myBorder.transform.position.y + borderCollider.size.y / 2 - 1;
            }
        }
    }

    private void Start() {
        
    }

    public void ChangeNotificationText(string newText, float waitTime, NotificationText.TextState option = 0) {
        notificationText.ChangeState(newText, waitTime, option);
    }

    public void ChangeNotificationText(NotificationString text) {
        notificationText.ChangeState(text.myString, text.waitTime, text.textState);
    }
	
    //Start level with some animation
    public void StartLevel(int option = 0) {
        if (introOutroAnim == null) {
            return;
        }
        introOutroAnim.SetInteger("Type", option);
        introOutroAnim.SetTrigger("NextState");
    }

    //End level with some animation
    public void EndLevel(int option = 0) {
        if (introOutroAnim == null) {
            return;
        }
        introOutroAnim.SetInteger("Type", option);
        introOutroAnim.SetTrigger("NextState");
    }
}
