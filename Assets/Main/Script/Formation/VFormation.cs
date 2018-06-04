using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class VFormation : Formation {
    //ADD MORE CODE TO SUPPORT SPAWNING FROM THE LEFT AND RIGHT

    public float minXDistance = 2;
    public float minYDistance = 2;
    public Vector2 captainOffset;
    public bool downward = true;
    public bool useSpinning = false;                        //Do we use the spinning animation?
    public bool sideWay = false;                            //Do we go from the side?
    //Do objects on the same height spawn at the same time?
    public bool sameHeightSpawnTogether = true;

    public override void SummonFormation(Vector3 center) {
        if (summonCoroutine == null) { 
            //Only summon when there is no other coroutine
            summonCoroutine = StartCoroutine(StartSummon(center));
        }
    }

    IEnumerator StartSummon(Vector3 center){
        //Facing which way?
        Vector3 targetRotation;
        targetRotation.z = downward == true ? 180 : 0;
        Sequence tweenSeq = DOTween.Sequence();
        int direction = 1;
        if (downward == false) {
            direction = -1;
        }

        if (mainObject != null) {
            //Spawn from the top and then move back
            Vector3 spawnPos = center;
            if (downward == true) {
                spawnPos.y = yBoundary.y;
            } else {
                spawnPos.y = yBoundary.x;
            }

            Transform newObject = Instantiate(mainObject, objectsMasterScript.transform);
            newObject.position = spawnPos;
            objectsMasterScript.AddChild(newObject);
            targetRotation.x = newObject.eulerAngles.x;
            targetRotation.y = newObject.eulerAngles.y;
            //Stop auto start
            StopAutoStart(newObject);
            //Add to spawn list
            spawnList.Add(newObject);

            Vector3 gotoPos = center;
            gotoPos.x += captainOffset.x;
            gotoPos.y += captainOffset.y * direction;

            //Now move down/up
            newObject.DOMove(gotoPos, getToPlaceTime).SetEase(Ease.InOutQuad);
            //And we rotate :D
            newObject.eulerAngles = targetRotation;

            yield return new WaitForSeconds(waitBetweenSpawn);
        }

        if (minorObjects.Count == 0) {
            yield break;
        }

        int objectCount = 0;
        

        for (int indx = 1; indx <= numOfMinor / 2; indx++) { 

            //Left wing
            Vector3 leftWing = center;
            leftWing.x -= minXDistance * indx;
            leftWing.y += minYDistance * direction * indx;
            Vector3 leftSpawn = leftWing;
            if (sideWay == true) { 
                leftSpawn.x = xBoundary.x;
            } else {
                leftSpawn.y = yBoundary.y;
            }

            //Spawn at the boundary first
            Transform newObject = Instantiate(minorObjects[spawnIndx], objectsMasterScript.transform);
            newObject.position = leftSpawn;
            objectsMasterScript.AddChild(newObject);
            objectCount++;
            targetRotation.x = newObject.eulerAngles.x;
            targetRotation.y = newObject.eulerAngles.y;
            //Stop auto start
            StopAutoStart(newObject);
            //Add to spawnList
            spawnList.Add(newObject);

            //Then move to the correct place and rotate :D
            if (useSpinning == true) { 
                tweenSeq.Append(newObject.DOMove(leftWing, getToPlaceTime)).Join(newObject.DORotate(targetRotation, getToPlaceTime)).SetEase(Ease.InOutQuad);
            } else {
                //Rotate immediately first
                newObject.eulerAngles = targetRotation;
                tweenSeq.Append(newObject.DOMove(leftWing, getToPlaceTime));
            }

            //Do we have any other object to spawn?
            if (objectCount > numOfMinor) {
                break;
            }

            //Don't wait if we choose to spawn objects who have the same height together
            if (sameHeightSpawnTogether == false) {
                yield return new WaitForSeconds(waitBetweenSpawn);
            }

            //Right wing
            Vector3 rightWing = center;
            rightWing.x += minXDistance * indx;
            rightWing.y += minYDistance * direction * indx;
            Vector3 rightSpawn = rightWing;
            if (sideWay == true) {
                rightSpawn.x = xBoundary.y;
            } else {
                rightSpawn.y = yBoundary.y;
            }
            
            //Spawn at the boundary first
            newObject = Instantiate(minorObjects[spawnIndx], objectsMasterScript.transform);
            newObject.position = rightSpawn;
            objectsMasterScript.AddChild(newObject);
            objectCount++;
            targetRotation.x = newObject.eulerAngles.x;
            targetRotation.y = newObject.eulerAngles.y;
            //Stop auto start
            StopAutoStart(newObject);
            //Add to spawnList
            spawnList.Add(newObject);

            //Then move to the correct place and rotate :D
            if (useSpinning == true) { 
                tweenSeq.Append(newObject.DOMove(rightWing, getToPlaceTime)).Join(newObject.DORotate(targetRotation, getToPlaceTime)).SetEase(Ease.InOutQuad);
            } else {
                //Rotate immediately first
                newObject.eulerAngles = targetRotation;
                tweenSeq.Append(newObject.DOMove(rightWing, getToPlaceTime));
            }

            //Do we have any other object to spawn?
            if (objectCount > numOfMinor) {
                break;
            }

            if (distributionType == DistributionType.Evenly) {
                ChangeIndex(0);     //Increase index
            }

            yield return new WaitForSeconds(waitBetweenSpawn);
        }

        //Reset the index back to normal
        spawnIndx = initialIndx;

        if (stayInFormation == true) {
            yield return new WaitForSeconds(getToPlaceTime);
        }

        //Launch objects
        LaunchObjects();

        summonCoroutine = null;
    }
}
