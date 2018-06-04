using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CustomPathFormation : Formation {

    [System.Serializable]
    public enum Orientation {
        None = 0,
        LookAtPosition = 1,
        LookAtTarget = 2,
        ToPath = 3
    }

    public enum MirrorType{
        None = 0,
        xAxis = 1,              //x-axis symmetry
        yAxis = 2,              //y-axis symmetry
        Both = 3                //Symmetrical on both axis
    }

    public int leaderIndx = 0;
    [Header("Path info")]
    public Vector3[] wayPoints;
    public bool closedPath = false;                      
    public MirrorType pathMirrorType = MirrorType.None;
    public bool loop = false;
    public PathType pathType = PathType.CatmullRom;
    public bool downward = true;
    [Header("Target stuff")]
    public Orientation pathOrientation;
    public Transform lookAtTarget = null;
    public float lookAhead = 0.001f;
    public Vector3 lookAtPosition = Vector3.zero;

    //private Vector3[] truePath;
    //private Vector3[] mirroredPath;

    // Use this for initialization
    public new void Start () {
        base.Start();

        if (pathOrientation == Orientation.LookAtTarget && lookAtTarget == null) {
            lookAtTarget = GameObject.Find("Player").transform;
        }

        //SetZToZero();
    }

    public void SetZToZero() {
        if(wayPoints.Length <= 0) {
            return;
        }

        for(int indx = 0; indx < wayPoints.Length; indx++) {
            wayPoints[indx].z = 0;
        }
    }

    public override void SummonFormation(Vector3 center) {
        if (wayPoints.Length <= 0) {
            return;
        }

        Vector3[] truePath = new Vector3[wayPoints.Length];
        Vector3[] mirroredPath = null;

        if(pathMirrorType != MirrorType.None) {
            mirroredPath = new Vector3[wayPoints.Length];
        }

        //Translate all the path upward
        for (int indx = 0; indx < wayPoints.Length; indx++) {
            //Set the true path
            truePath[indx] = new Vector3(wayPoints[indx].x, wayPoints[indx].y + center.y, 0);

            switch (pathMirrorType) {
                //Also set the mirror path
                case MirrorType.xAxis:
                mirroredPath[indx] = truePath[indx];
                mirroredPath[indx].y *= -1;
                break;

                case MirrorType.yAxis:
                mirroredPath[indx] = truePath[indx];
                mirroredPath[indx].x *= -1;
                break;

                case MirrorType.Both:
                mirroredPath[indx] = truePath[indx];
                mirroredPath[indx] *= -1;
                break;
            }
        }

        StartCoroutine(SpawnCoroutine(truePath, center));

        if (pathMirrorType != MirrorType.None) {
            StartCoroutine(SpawnCoroutine(mirroredPath, center));
        }
    }

    public IEnumerator SpawnCoroutine(Vector3[] path, Vector3 center) {
        //We need to ensure that the spawn order of the leader is reasonable
        if (mainObject == null) {
            leaderIndx = -1;
        } else {
            if (minorObjects.Count <= 0 || numOfMinor == 0) {
                leaderIndx = 0;
            } else {
                //Clamp the order
                leaderIndx = Mathf.Clamp(leaderIndx, 0, minorObjects.Count);
            }
        }

        int spawnedMinion = 0;

        for(int indx = 0; indx < numOfMinor; indx++) {
            if (spawnedMinion == leaderIndx) {
                //Spawn the object first
                Transform spawnedMain = Instantiate(mainObject, path[0], Quaternion.identity);
                objectsMasterScript.AddChild(spawnedMain);
                //Add the object to the spawned list
                spawnList.Add(spawnedMain);

                Tween tween1;
                switch (pathOrientation) {
                    case Orientation.ToPath:
                    tween1 = spawnedMain.DOPath(path, getToPlaceTime, pathType, PathMode.TopDown2D)
                    .SetOptions(closedPath)
                    .SetEase(Ease.Linear)
                    .SetLookAt(lookAhead, Vector3.forward, Vector3.right)
                    ;
                    if (loop == true) {
                        //Loop to inifinity and beyond!
                        tween1.SetLoops(-1, LoopType.Yoyo);
                    }
                    break;

                    case Orientation.LookAtPosition:
                    tween1 = spawnedMain.DOPath(path, getToPlaceTime, pathType, PathMode.TopDown2D)
                        .SetOptions(closedPath)
                        .SetEase(Ease.Linear)
                        .SetLookAt(lookAtPosition)
                        ;
                    if (loop == true) {
                        //Loop to inifinity and beyond!
                        tween1.SetLoops(-1, LoopType.Yoyo);
                    }
                    break;

                    case Orientation.LookAtTarget:
                    tween1 = spawnedMain.DOPath(path, getToPlaceTime, pathType, PathMode.TopDown2D)
                        .SetOptions(closedPath)
                        .SetEase(Ease.Linear)
                        .SetLookAt(lookAtTarget)
                        ;
                    if (loop == true) {
                        //Loop to inifinity and beyond!
                        tween1.SetLoops(-1, LoopType.Yoyo);
                    }
                    break;

                    case Orientation.None:
                    Vector3 targetRotation = spawnedMain.eulerAngles;
                    targetRotation.z = 0;
                    if (downward == true) {
                        targetRotation.z = 180;
                    }

                    tween1 = spawnedMain.DOPath(path, getToPlaceTime, pathType, PathMode.TopDown2D)
                        .SetOptions(closedPath)
                        .SetEase(Ease.Linear)
                        ;
                    if (loop == true) {
                        //Loop to inifinity and beyond!
                        tween1.SetLoops(-1, LoopType.Yoyo);
                    }

                    //Rotate afterward  -   Add a delay before rotation to override the look rotation of the tween
                    StartCoroutine(RotateObjectWithDelay(spawnedMain, targetRotation, 0.00001f));
                    break;
                }

                spawnedMinion++;
                yield return new WaitForSeconds(waitBetweenSpawn);
            }

            Transform spawnedObject = Instantiate(minorObjects[spawnIndx], path[0], Quaternion.identity);
            objectsMasterScript.AddChild(spawnedObject);

            Tween tween2;
            switch (pathOrientation) {
                case Orientation.ToPath:
                tween2 = spawnedObject.DOPath(path, getToPlaceTime, pathType, PathMode.TopDown2D)
                .SetOptions(closedPath)
                .SetEase(Ease.Linear)
                .SetLookAt(lookAhead, Vector3.forward, Vector3.right)
                ;
                if (loop == true) {
                    //Loop to inifinity and beyond!
                    tween2.SetLoops(-1, LoopType.Yoyo);
                }
                break;

                case Orientation.LookAtPosition:
                tween2 = spawnedObject.DOPath(path, getToPlaceTime, pathType, PathMode.TopDown2D)
                    .SetOptions(closedPath)
                    .SetEase(Ease.Linear)
                    .SetLookAt(lookAtPosition)
                    ;
                if (loop == true) {
                    //Loop to inifinity and beyond!
                    tween2.SetLoops(-1, LoopType.Yoyo);
                }
                break;

                case Orientation.LookAtTarget:
                tween2 = spawnedObject.DOPath(path, getToPlaceTime, pathType, PathMode.TopDown2D)
                    .SetOptions(closedPath)
                    .SetEase(Ease.Linear)
                    .SetLookAt(lookAtTarget)
                    ;
                if (loop == true) {
                    //Loop to inifinity and beyond!
                    tween2.SetLoops(-1, LoopType.Yoyo);
                }
                break;

                case Orientation.None:
                Vector3 targetRotation = spawnedObject.eulerAngles;
                targetRotation.z = 0;
                if (downward == true) {
                    targetRotation.z = 180;
                }

                tween2 = spawnedObject.DOPath(path, getToPlaceTime, pathType, PathMode.TopDown2D)
                    .SetOptions(closedPath)
                    .SetEase(Ease.Linear)
                    ;
                if (loop == true) {
                    //Loop to inifinity and beyond!
                    tween2.SetLoops(-1, LoopType.Yoyo);
                }

                //Rotate afterward  -   Add a delay before rotation to override the look rotation of the tween
                StartCoroutine(RotateObjectWithDelay(spawnedObject, targetRotation, 0.00001f));
                break;
            }

            //Increase the number of minions spawned
            spawnedMinion++;
            //Add the object to the spawned list
            spawnList.Add(spawnedObject);

            if (distributionType == DistributionType.Evenly) {
                ChangeIndex(0);     //Increase index
            }

            yield return new WaitForSeconds(waitBetweenSpawn);
        }

        if (spawnedMinion == leaderIndx) {
            //Is it time to spawn the leader?
            Transform spawnedMain = Instantiate(mainObject, path[0], Quaternion.identity);
            objectsMasterScript.AddChild(spawnedMain);
            //Add the object to the spawned list
            spawnList.Add(spawnedMain);

            Tween tween1;
            switch (pathOrientation) {
                case Orientation.ToPath:
                tween1 = spawnedMain.DOPath(path, getToPlaceTime, pathType, PathMode.TopDown2D)
                .SetOptions(closedPath)
                .SetEase(Ease.Linear)
                .SetLookAt(lookAhead, Vector3.forward, Vector3.right)
                ;
                if (loop == true) {
                    //Loop to inifinity and beyond!
                    tween1.SetLoops(-1, LoopType.Yoyo);
                }
                break;

                case Orientation.LookAtPosition:
                tween1 = spawnedMain.DOPath(path, getToPlaceTime, pathType, PathMode.TopDown2D)
                    .SetOptions(closedPath)
                    .SetEase(Ease.Linear)
                    .SetLookAt(lookAtPosition)
                    ;
                if (loop == true) {
                    //Loop to inifinity and beyond!
                    tween1.SetLoops(-1, LoopType.Yoyo);
                }
                break;

                case Orientation.LookAtTarget:
                tween1 = spawnedMain.DOPath(path, getToPlaceTime, pathType, PathMode.TopDown2D)
                    .SetOptions(closedPath)
                    .SetEase(Ease.Linear)
                    .SetLookAt(lookAtTarget)
                    ;
                if (loop == true) {
                    //Loop to inifinity and beyond!
                    tween1.SetLoops(-1, LoopType.Yoyo);
                }
                break;

                case Orientation.None:
                Vector3 targetRotation = spawnedMain.eulerAngles;
                targetRotation.z = 0;
                if (downward == true) {
                    targetRotation.z = 180;
                }

                tween1 = spawnedMain.DOPath(path, getToPlaceTime, pathType, PathMode.TopDown2D)
                    .SetOptions(closedPath)
                    .SetEase(Ease.Linear)
                    ;
                if (loop == true) {
                    //Loop to inifinity and beyond!
                    tween1.SetLoops(-1, LoopType.Yoyo);
                }

                //Rotate afterward  -   Add a delay before rotation to override the look rotation of the tween
                StartCoroutine(RotateObjectWithDelay(spawnedMain, targetRotation, 0.00001f));
                break;
            }
            yield return new WaitForSeconds(waitBetweenSpawn);
        }

        //Afterward, launch all the object
        LaunchObjects();
    }

    public IEnumerator RotateObjectWithDelay(Transform target, Vector3 targetRotation, float delay = 0) {
        yield return new WaitForSeconds(delay);

        target.eulerAngles = targetRotation;
    }

    public override void StartObject(Transform curObject) {
        if (curObject == null) {
            return;
        }

        GeneralObject generalScript = curObject.GetComponent<GeneralObject>();
        if (generalScript != null) {
            generalScript.isStarted = true;
            generalScript.canAttack = true;
        }
    }
}
