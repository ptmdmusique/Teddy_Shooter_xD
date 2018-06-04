using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomCollectibleGenerator : MonoBehaviour {

    Debugging masterDebugger;
    GameMaster gmScript;
    Collector collectibleMaster;
    Vector2 xBoundary;
    Vector2 yBoundary;
    Vector2 xScreen;
    Vector2 yScreen;
    Coroutine spawnCoroutine;

    public float waveWait = 0;
    public int numOfSpawn = 5;
    public float spawnObjDelay = 3;
    public float spawnDelay = 3;
    public List<Debugging.CollectibleObj> collectibleList;

    private void Start() {
        masterDebugger = GetComponent<Debugging>();
        collectibleList = masterDebugger.collectibleList;        //Get enemy list

        gmScript = GetComponent<GameMaster>();
        //Get the Masters!
        GetTheMaster();
        //Invoke("GetTheMaster", 1);

        StartSpawn();
        //Start spawning
        //Invoke("StartSpawn", 2);
    }

    IEnumerator SpawnCoroutine() {
        for (int indx = 0; indx < numOfSpawn; indx++) {
            int spawnIndx = Random.Range(0, collectibleList.Count);
            int spawnChance = Random.Range(0, 100);

            //If we get the chance within the spawnChance range, then spawn it
            if (spawnChance <= collectibleList[spawnIndx].spawnChance) {
                
                //Instantiate
                Transform newCollectible = Instantiate(collectibleList[spawnIndx].myCollectible, 
                                                        collectibleMaster.transform);
                //Put the object into the master
                GeneralObject newColScript = newCollectible.GetComponent<GeneralObject>();
                if (newCollectible != null && newColScript != null) {
                    collectibleMaster.AddChild(newCollectible);

                    //Calculate the position
                    Vector3 spawnPos = transform.position;
                    spawnPos.x = Random.Range(xScreen.x, xScreen.y);

                    //Should the collectible be ally or enemy?
                    int state = Random.Range(0, 2);
                    if (state == 0) {
                        newColScript.myAllianceState = AllianceState.Ally;
                        spawnPos.y = yBoundary.y - 0.5f;
                    } else {
                        newColScript.myAllianceState = AllianceState.Enemy;
                        spawnPos.y = yBoundary.x + 0.5f;
                    }

                    newCollectible.position = spawnPos;
                }
            }

            yield return new WaitForSeconds(spawnObjDelay);
        }

        yield return new WaitForSeconds(waveWait);

        spawnCoroutine = null;
        StartSpawn();
    }

    void StartSpawn() {
        if (spawnCoroutine == null) {
            spawnCoroutine = StartCoroutine(SpawnCoroutine());
        }
    }

    void GetTheMaster() {
        //Get the other Masters!
        collectibleMaster = gmScript.collectiblesMasterScript;

        xScreen = gmScript.xScreen;
        yScreen = gmScript.yScreen;
        xBoundary = gmScript.xBoundary;
        yBoundary = gmScript.yBoundary;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
