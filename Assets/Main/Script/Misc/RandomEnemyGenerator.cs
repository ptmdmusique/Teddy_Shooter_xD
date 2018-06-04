using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEnemyGenerator : MonoBehaviour {

    Debugging masterDebugger;
    public List<Transform> enemyList;

    [Header("Spawn Info")]
    public float waveWaitTime;
    public float enemyWaitTime;
    public int numberOfWave = -1;
    public int spawnIndx = 0;
    public int enemyPerWave = 10;
    public bool isRandom = false;

    //Game Master and other misc
    private Coroutine spawnCoroutine;
    private GameMaster masterScript;
    private Vector2 xBoundary;
    private Vector2 yBoundary;
    private int curWave = 0;
    //public Transform enemyCollector;
    public Collector enemyCollectorScript;
    //ADD CODE TO SUPPORT SPAWNING FROM OTHER SIDES AND DIRECTION!

    private void Start() {
        masterDebugger = GetComponent<Debugging>();
        enemyList = masterDebugger.toSpawnObjectList;        //Get enemy list

        masterScript = GetComponent<GameMaster>();
        //Get the Masters!
        Invoke("GetTheMaster", 1);

        //Start spawning
        Invoke("StartSpawn", 2);
    }

    IEnumerator SpawnCoroutine() {
        curWave++;
        for (int indx = 0; indx < enemyPerWave; indx++) {
            if (isRandom == true) {
                spawnIndx = Random.Range(0, enemyList.Count);
            }

            //ADD CODE TO SUPPORT SPAWNING FROM OTHER SIDES AND DIRECTION!
            Vector3 spawnPos = transform.position;
            spawnPos.x = Random.Range(xBoundary.x, xBoundary.y);
            Transform newEnemy = Instantiate(enemyList[spawnIndx], enemyCollectorScript.transform);
            newEnemy.position = spawnPos;
            GeneralObject newEnemyScript = newEnemy.GetComponent<GeneralObject>();
            if (newEnemy != null && newEnemyScript != null) {
                enemyCollectorScript.AddChild(newEnemy);
            }

            yield return new WaitForSeconds(enemyWaitTime);
        }
        yield return waveWaitTime;

        if (numberOfWave == -1) {       //Infinitely spawning
            curWave = 0;
            spawnCoroutine = StartCoroutine(SpawnCoroutine());
        } else {
            if (curWave < numberOfWave) {
                spawnCoroutine = StartCoroutine(SpawnCoroutine());
            } else {
                spawnCoroutine = null;
            }
        }
    }

    void StartSpawn() {
        if (spawnCoroutine == null) {
            spawnCoroutine = StartCoroutine(SpawnCoroutine());
        }
    }

    void GetTheMaster() {
        //Get the other Masters!
        enemyCollectorScript = masterScript.objectsMasterScript;

        //Get the important info
            //Get the boundaries
        xBoundary = masterScript.xBoundary;
        yBoundary = masterScript.yBoundary;
    }
}
