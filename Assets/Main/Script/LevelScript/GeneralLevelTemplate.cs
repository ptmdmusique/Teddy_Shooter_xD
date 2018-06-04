using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralLevelTemplate : MonoBehaviour {

    [System.Serializable]
    public class ObjectList {
        public Transform myObject;

        [Header("Object basic info")]
        public float mySpeed = -1;
        public float myDamage = -1;
        public Vector2 initialDir = Vector2.zero;
        public float myValue = -1;
        public float maxHealth = -1;
        public float curHealth = -1;
    }
    [Header("Enemy list")]
    public List<ObjectList> enemyList;
    public List<ObjectList> bossList;
    public List<int> checkPointScore;                           //Score to reach the next check point
    public List<NotificationString> checkPointString;           //String to display when we reach the checkpoint
    public NotificationString levelStartString;

    
    [Header("Formation")]
    public List<Formation> formationList;
    public List<int> formationSpawnChance;
    public float formationWaitTime = 0.5f;
    protected int formationLowIndx = -1;
    protected int formationHighIndx = -1;
    protected Coroutine formationCoroutine;

    protected int waveLowIndx = 0;                                   //Lowest index of the spawn-able list
    protected int waveHighIndx = 0;                                  //Highest index of the spawn-able list
    protected int checkPoint = 0;                                   //What checkpoint are we currently at?
    protected bool reachCheckPoint = false;
    protected GameMaster gmScript;
    protected PlayerShip playerShip;
    protected bool faded = false;
    protected Collector objectMasterScript;

    [Header("Wave info")]
    public List<int> enemySpawnChance = new List<int>();
    public float enemyWaitTime = 0.5f;
    public float waveWaitTime = 2;
    public int enemyPerWave = 20;
    public float maxAttempt = 5;                                  //Only keep trying to spawn something within the attempts allowed
    protected Transform curBoss = null;
    protected Coroutine waveCoroutine;

    protected Vector2 xBoundary;
    protected Vector2 yBoundary;
    protected Vector2 xScreen;
    protected Vector2 yScreen;
    protected float screenWidth;
    protected float screenHeight;

    // Use this for initialization
    protected void Awake() {
        if (enemySpawnChance.Count == 0) {
            enemySpawnChance = new List<int>(enemyList.Count);
            for (int indx = 0; indx < enemyList.Count; indx++) {
                enemySpawnChance.Add(100);
            }
        }

        if (checkPointScore.Count == 0) {
            checkPointScore = new List<int>(enemyList.Count);
            checkPointScore.Add(200);
            for (int indx = 0; indx < enemyList.Count; indx++) {
                checkPointScore.Add((int)(checkPointScore[indx - 1] * 1.5f));
            }
        }
    }

    public virtual void Start() {
        playerShip = GameObject.Find("Player").GetComponent<PlayerShip>();
        gmScript = GameObject.Find("Game Master").GetComponent<GameMaster>();
        objectMasterScript = gmScript.objectsMasterScript;

        xScreen = gmScript.xScreen;
        yScreen = gmScript.yScreen;
        xBoundary = gmScript.xBoundary;
        yBoundary = gmScript.yBoundary;

        gmScript.ChangeNotificationText(levelStartString);
        gmScript.StartLevel();

        screenWidth = GlobalState.GetCameraWidth(gmScript.mainCamera);
        screenHeight = GlobalState.GetCameraHeight(gmScript.mainCamera);
        //StartCoroutine(SpawnWave(3));
    }

    public virtual bool SpawnEnemy(int indx) {
        int chance = Random.Range(0, 101);
        if (chance < enemySpawnChance[indx]) {
            //Spawn the enemy
            Transform newEnemy = Instantiate(enemyList[indx].myObject, objectMasterScript.transform);

            //Put the enemy into the right place
            Vector3 newPos = new Vector3(Random.Range(xScreen.x, xScreen.y) * 0.9f, yBoundary.y, 0);
            newEnemy.position = newPos;

            //Put the enemy into the right parent
            objectMasterScript.AddChild(newEnemy);            

            GeneralObject newEnemyScript = newEnemy.GetComponent<GeneralObject>();
            if (newEnemyScript != null) {
                newEnemyScript.myAllianceState = AllianceState.Enemy;
                if (newEnemyScript.autoLaunch == false) {
                    StartCoroutine(StartObject(newEnemyScript));
                }

                //Adjust the enemy's stat
                if(enemyList[indx].myDamage != -1) {
                    newEnemyScript.myDamage = enemyList[indx].myDamage;
                }
                if (enemyList[indx].curHealth != -1) {
                    newEnemyScript.curHealth = enemyList[indx].curHealth;
                }
                if (enemyList[indx].maxHealth != -1) {
                    newEnemyScript.maxHealth = enemyList[indx].maxHealth;
                }
                if (enemyList[indx].myValue != -1) {
                    newEnemyScript.myValue = enemyList[indx].myValue;
                }
                if (enemyList[indx].mySpeed != -1) {
                    newEnemyScript.mySpeed = enemyList[indx].mySpeed;
                }
            }

            return true;
        }

        return false;
    }

    public virtual IEnumerator StartObject(GeneralObject script) {
        yield return new WaitForSeconds(0.03f);
        script.StartObject(script.initialDir, script.mySpeed);
    }

    public virtual void SpawnBoss(int indx, Vector3 fromWhere) {
        //Spawn the boss
        curBoss = Instantiate(bossList[indx].myObject, fromWhere, Quaternion.identity);

        //Put into manager
        objectMasterScript.AddChild(curBoss);

        GeneralObject bossScript = curBoss.Find("Main").GetComponent<GeneralObject>();
        if (bossScript != null) {
            //Launch the boss
            bossScript.StartObject(bossScript.initialDir, bossScript.mySpeed);

            //Adjust the enemy's stat
            if (bossList[indx].myDamage != -1) {
                bossScript.myDamage = bossList[indx].myDamage;
            }
            if (bossList[indx].curHealth != -1) {
                bossScript.curHealth = bossList[indx].curHealth;
            }
            if (bossList[indx].maxHealth != -1) {
                bossScript.maxHealth = bossList[indx].maxHealth;
            }
            if (bossList[indx].myValue != -1) {
                bossScript.myValue = bossList[indx].myValue;
            }
            if (bossList[indx].mySpeed != -1) {
                bossScript.mySpeed = bossList[indx].mySpeed;
            }
        }
    }

    public virtual IEnumerator SpawnWave(float delay = 0) {

        yield return new WaitForSeconds(delay);

        if (curBoss == null) {
            //Only spawn when the boss is not around
            for (int indx = 0; indx < Random.Range(enemyPerWave * 0.8f, enemyPerWave + 1); indx++) {
                int spawnIndx = Random.Range(waveLowIndx, waveHighIndx + 1);
                int attempt = 1;
                while (SpawnEnemy(spawnIndx) == false && attempt++ <= maxAttempt) {
                    //Must spawn something
                    spawnIndx = Random.Range(waveLowIndx, waveHighIndx + 1);
                }

                yield return new WaitForSeconds(Random.Range(enemyWaitTime * 0.75f, enemyWaitTime));
            }

            yield return new WaitForSeconds(Random.Range(waveWaitTime * 0.75f, waveWaitTime));
        }

        waveCoroutine = StartCoroutine(SpawnWave());
    }

    public virtual IEnumerator SpawnFormation(float delay = 0) {

        yield return new WaitForSeconds(delay);

        if (curBoss == null && formationLowIndx > -1) {
            int spawnIndx = Random.Range(formationLowIndx, formationHighIndx + 1);
            int spawnChance = Random.Range(0, 101);

            if(spawnChance <= formationSpawnChance[spawnIndx]) {
                //Spawn within the permitted areas
                Vector2 spawnPos = new Vector2(Random.Range(xScreen.y - screenWidth * formationList[spawnIndx].xBound.y, xScreen.y - screenWidth * formationList[spawnIndx].xBound.x), 
                                                Random.Range(yScreen.y - screenHeight * formationList[spawnIndx].yBound.y, yScreen.y - screenHeight * formationList[spawnIndx].yBound.x));
                formationList[spawnIndx].SummonFormation(spawnPos);
            }
        }

        yield return new WaitForSeconds(Random.Range(formationWaitTime * 0.75f, formationWaitTime));
        formationCoroutine = StartCoroutine(SpawnFormation());
    }
}
