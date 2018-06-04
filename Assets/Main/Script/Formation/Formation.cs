using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[System.Serializable]
public class Formation : MonoBehaviour {

    [Header("Object list")]
    public List<Transform> minorObjects;
    public Transform mainObject;
    protected List<Transform> spawnList = new List<Transform>();
    protected Coroutine summonCoroutine;

    [Header("Object info")]
    public int numOfMinor = 0;
    public int numOfMain = 0;
    public int spawnIndx = 0;
    public float getToPlaceTime = 1;
    public float waitBetweenSpawn = 0.5f;
    public bool stayInFormation = false;
    public bool useOwnSpeed = false;
    protected float minSpeed = 0;
    protected int initialIndx = 0;

    [Header("Distribution Type")]
    public DistributionType distributionType = DistributionType.SingleType;

    [Header("Boundary")]
    public Vector2 xBound = Vector2.zero;         //The x boundary to spawn the formation within (from 0 to 1, relative to the screen width) 
    public Vector2 yBound = Vector2.zero;         //The y boundary to spawn the formation within (from 0 to 1, relative to the screen height)

    protected Collector objectsMasterScript;
    protected GameMaster gmScript;
    protected Vector2 xBoundary;
    protected Vector2 yBoundary;

    protected void Start() {
        Invoke("GetTheMaster", 0.1f);
        RebuildList();
    }

    public void RebuildList() {
        initialIndx = spawnIndx;
        FindMinSpeed();
    }

    public void FindMinSpeed() {
        //Find the minimum speed of the objects in the current list
        GeneralObject tempScript = null;
        if (mainObject != null) { 
            tempScript = mainObject.GetComponent<GeneralObject>();
        }

        if (tempScript != null) {
            minSpeed = tempScript.mySpeed;
        }

        for (int indx = 0; indx < minorObjects.Count; indx++) {
            tempScript = minorObjects[indx].GetComponent<GeneralObject>();
            if (tempScript != null && minSpeed > tempScript.mySpeed) {
                minSpeed = tempScript.mySpeed;
            }
        }
    }

    public void GetTheMaster() {
        gmScript = GameObject.Find("Game Master").GetComponent<GameMaster>();
        if (gmScript != null) {
            objectsMasterScript = gmScript.objectsMasterScript;
            xBoundary = gmScript.xBoundary;
            yBoundary = gmScript.yBoundary;
        }
    }

    public void ChangeIndex(int option = 0) {
        if (option == 0) {
            spawnIndx++;
            if (spawnIndx >= minorObjects.Count) {
                spawnIndx = 0;
            } 
        } else {
            spawnIndx--;
            if (spawnIndx < 0) {
                spawnIndx = minorObjects.Count - 1;
            }
        }
    }

    public virtual void SummonFormation(Vector3 center) { }

    public void StopAutoStart(Transform curObject) {
        GeneralObject generalScript = curObject.GetComponent<GeneralObject>();
        if (generalScript != null) {
            generalScript.autoLaunch = false;
        }
    }

    public virtual void StartObject(Transform curObject) {
        if (curObject == null) {
            return;
        }
        
        GeneralObject generalScript = curObject.GetComponent<GeneralObject>();
        if (generalScript != null) {
            if (useOwnSpeed == false) {
                generalScript.StartObject(generalScript.initialDir, minSpeed);
            } else { 
                generalScript.StartObject(generalScript.initialDir, generalScript.mySpeed);
            }
        }
    }

    public void LaunchObjects() {        
        foreach(Transform child in spawnList) {
            StartObject(child);
        }
    }
}
