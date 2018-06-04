using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class Debugging : MonoBehaviour {

    public bool isDebugging = false;
    public List<Transform> toSpawnObjectList;
    [System.Serializable]
    public class CollectibleObj {
        public Transform myCollectible;
        public float spawnChance = 100;
    }
    public List<CollectibleObj> collectibleList;

    [Header("Test Spawn")]
    public int spawnSize = 0;
    public int enemyIndx = 0;
    public float xDistance = 3;
    public bool randomEnemy = false;
    public bool canRestart = true;
    public float customTimeScale = -1;
    public Transform bossToSpawn;

    [Header("Dotween Test")]
    private TweenParams tweenParm;
    private Sequence tweenSeq;
    private Tween myTween;
    private float multiplier = 1;
    public float testTime = 2;
    public Transform testTarget;
    public bool moveTest = false;
    public bool scaleTest = false;
    public bool combineTest = false;

    [Header("Formation Test")]
    public VFormation vFormation;
    public CustomPathFormation pathFormation;
    
    private GameMaster gmScript;
    private PlayerShip myPlayer;

    [Header("Debug stats")]
    public float addManaVal = 50;

    [Header("Mouse test")]
    public bool showMouse = false;
    public Transform mouseSprite;

	// Use this for initialization
	void Start () {
        gmScript = GetComponent<GameMaster>();
        myPlayer = GameObject.Find("Player").GetComponent<PlayerShip>();

        //Tween stuff
        tweenParm = new TweenParams().SetLoops(1, LoopType.Yoyo).SetEase(Ease.Linear).SetAutoKill(false);

        //Formation
        if (vFormation == null) { 
            vFormation = GetComponent<VFormation>();
        }
        if (pathFormation == null) { 
            pathFormation = GetComponent<CustomPathFormation>();
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (canRestart == true) {
            //Restart
            if (Input.GetKeyDown(KeyCode.R) == true) {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }

		if (isDebugging == false) {
            return;
        }

        //Show mouse position
        if (mouseSprite != null) {
            if (showMouse == true) {
                mouseSprite.gameObject.SetActive(true);
                mouseSprite.position = GlobalState.GetMousePosition(gmScript.mainCamera);
            } else {
                mouseSprite.gameObject.SetActive(false);
            }
        }

        //Spawn enemy
        if (Input.GetKeyDown(KeyCode.P) == true) {
            //Spawn a row of enemy
            for (int indx = 0; indx < spawnSize; indx++) {
                if (randomEnemy == false) {
                    Transform newEnemy = Instantiate(toSpawnObjectList[enemyIndx], gmScript.objectsMaster);
                    Vector3 newPos = transform.position;
                    newPos.x += xDistance * indx;
                    newEnemy.position = newPos;

                    gmScript.objectsMasterScript.AddChild(newEnemy);
                } else {
                    int tmpIndx = Random.Range(0, toSpawnObjectList.Count);
                    Transform newEnemy = Instantiate(toSpawnObjectList[tmpIndx], gmScript.objectsMaster);
                    Vector3 newPos = transform.position;
                    newPos.x += xDistance * indx;
                    newEnemy.position = newPos;

                    gmScript.objectsMasterScript.AddChild(newEnemy);
                }
            }
        }

        //Increase curMana
        if (Input.GetKeyDown(KeyCode.L) == true) {
            myPlayer.AddRage(addManaVal);
            myPlayer.curScore += 10000;
        }

        if (Input.GetKeyDown(KeyCode.O) == true) {
            myPlayer.ChangeActiveSingle();
        }

        if (customTimeScale > -1) {
            Time.timeScale = customTimeScale;
        }
   
        if (Input.GetMouseButtonDown(0) == true) {
            if (testTarget != null) {
                if (moveTest == true) { 
                    testTarget.DOMove(GlobalState.GetMousePosition(gmScript.mainCamera), testTime).SetAs(tweenParm);
                }
                if (scaleTest == true) {
                    testTarget.DOScale(multiplier, testTime).SetAs(tweenParm).OnComplete(DoubleMultiplier);
                    //testTarget.DOScale(transform.localScale, testTime * Time.deltaTime).SetAs(tweenParm).OnComplete(DoubleMultiplier);
                }

                if (combineTest == true) {
                    Vector3 rotateAngle = testTarget.eulerAngles;
                    if (rotateAngle.z <= 45 && rotateAngle.z >= -45) {
                        rotateAngle.z = 180;
                    } else if (rotateAngle.z > 45 && rotateAngle.z <= 135) {
                        rotateAngle.z = 270;
                    } else if (rotateAngle.z > 135 && rotateAngle.z <= 225) {
                        rotateAngle.z = 0;
                    } else {
                        rotateAngle.z = 90;
                    }
                    tweenSeq = DOTween.Sequence();
                    tweenSeq.Append(testTarget.DOMove(GlobalState.GetMousePosition(gmScript.mainCamera), testTime))
                            .Join(testTarget.DORotate(rotateAngle, testTime)).Pause()
                            //.Join(testTarget.DOLookAt(Vector3.down, testTime, AxisConstraint.X, Vector3.Cross(Vector3.up, Vector3.right)))
                            ;
                    tweenSeq.SetAs(tweenParm);
                    tweenSeq.Play();

                }
            }
        }

        if (Input.GetKeyDown(KeyCode.T) == true) {
            gmScript.ChangeNotificationText("Test", 2, 0);
        }

        //Formation Test
        if (Input.GetKeyDown(KeyCode.Comma) == true) {
            //vFormation.SummonFormation(GlobalState.GetMousePosition());
            pathFormation.SummonFormation(GlobalState.GetMousePosition(gmScript.mainCamera));
        }

        //Summon boss
        if (Input.GetKeyDown(KeyCode.Delete) == true) {
            Instantiate(bossToSpawn, GlobalState.GetMousePosition(gmScript.mainCamera), Quaternion.identity);
        }
	}

    void DoubleMultiplier() {
        multiplier *= 2;
    }
}
