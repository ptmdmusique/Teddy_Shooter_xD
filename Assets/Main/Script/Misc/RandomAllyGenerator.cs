using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAllyGenerator : MonoBehaviour {

    Debugging masterDebugger;
    public List<Transform> allyList;

    [Header("Spawn Info")]
    public float waveWaitTime;
    public float allyWaitTime;
    public int numberOfWave = -1;
    public int spawnIndx = 0;
    public int allyPerWave = 10;
    public bool isRandom = false;

    //Game Master and other misc
    private Coroutine spawnCoroutine;
    private GameMaster masterScript;
    private Vector2 xBoundary;
    private Vector2 yBoundary;
    private int curWave = 0;
    public Collector allyColectorScript;
    //ADD CODE TO SUPPORT SPAWNING FROM OTHER SIDES AND DIRECTION!

    private void Start() {
        masterDebugger = GetComponent<Debugging>();
        allyList = masterDebugger.toSpawnObjectList;        //Get ally list

        masterScript = GetComponent<GameMaster>();
        //Get the Masters!
        Invoke("GetTheMaster", 1);

        //Start spawning
        Invoke("StartSpawn", 2);
    }

    IEnumerator SpawnCoroutine() {
        curWave++;
        for (int indx = 0; indx < allyPerWave; indx++) {
            if (isRandom == true) {
                spawnIndx = Random.Range(0, allyList.Count);
            }

            //ADD CODE TO SUPPORT SPAWNING FROM OTHER SIDES AND DIRECTION!
            Vector3 spawnPos = transform.position;
            spawnPos.y = yBoundary.x - 5;
            spawnPos.x = Random.Range(xBoundary.x, xBoundary.y);
            Transform newAlly = Instantiate(allyList[spawnIndx], allyColectorScript.transform);
            newAlly.position = spawnPos;
            GeneralObject newEnemyScript = newAlly.GetComponent<GeneralObject>();
            if (newAlly != null && newEnemyScript != null) {
                allyColectorScript.AddChild(newAlly);
            }

            yield return new WaitForSeconds(allyWaitTime);
        }
        yield return waveWaitTime;

        if (numberOfWave == -1) {       //Infinitely spawning
            curWave = 0;
            spawnCoroutine = StartCoroutine(SpawnCoroutine());
        }
        else {
            if (curWave < numberOfWave) {
                spawnCoroutine = StartCoroutine(SpawnCoroutine());
            }
            else {
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
        allyColectorScript = masterScript.alliesMasterScript;

        //Get the important info
        //Get the boundaries
        xBoundary = masterScript.xBoundary;
        yBoundary = masterScript.yBoundary;
    }
}
