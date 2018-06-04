using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLevel1 : GeneralLevelTemplate {

    public override void Start() {
        base.Start();
        waveCoroutine = StartCoroutine(SpawnWave(3));
    }

    // Update is called once per frame
    void Update () {
        switch (checkPoint) {
            case 0:
            if (playerShip.curScore >= checkPointScore[checkPoint]) {
                //Show the notification text
                gmScript.ChangeNotificationText(checkPointString[checkPoint]);
                checkPoint++;

                //Change the index accordingly
                waveLowIndx = 0;
                waveHighIndx = 1;

                //Change the probability to spawn
                enemySpawnChance[0] = 75;
                enemySpawnChance[1] = 50;
            }
            break;

            case 1:
            if (playerShip.curScore >= checkPointScore[checkPoint]) {
                gmScript.ChangeNotificationText(checkPointString[checkPoint]);
                checkPoint++;
                waveHighIndx = 2;
                
                enemySpawnChance[0] = 50;
                enemySpawnChance[1] = 20;
                enemySpawnChance[2] = 30;

                formationCoroutine = StartCoroutine(SpawnFormation(0.5f));
                formationLowIndx = 0;
                formationHighIndx = 1;
            }
            break;

            case 2:
            if (playerShip.curScore >= checkPointScore[checkPoint]) {
                gmScript.ChangeNotificationText(checkPointString[checkPoint]);
                
                checkPoint++;
                waveHighIndx = 3;
                
                enemySpawnChance[1] = 30;
                enemySpawnChance[3] = 40;

                formationLowIndx = 0;
                formationHighIndx = 2;
            }
            break;

            case 3:
            if (playerShip.curScore >= checkPointScore[checkPoint]) {
                gmScript.ChangeNotificationText(checkPointString[checkPoint]);
                checkPoint++;
                formationLowIndx = 0;
                formationHighIndx = 4;
            }
            break;

            case 4:
            if (playerShip.curScore >= checkPointScore[checkPoint]) {
                gmScript.ChangeNotificationText(checkPointString[checkPoint]);

                //Stop the old coroutine to avoid infinite loop when the chances are 0
                if(waveCoroutine != null) { 
                    StopCoroutine(waveCoroutine);
                }

                if(formationCoroutine != null) {
                    StopCoroutine(formationCoroutine);
                }

                checkPoint++;
                //Spawn the boss first since the condition of the wave spawning depends on the boss
                SpawnBoss(0, new Vector3(0, yBoundary.y, 0));
                

                //Set all the chance to 0, 
                enemySpawnChance[0] = 0;
                enemySpawnChance[1] = 0;
                enemySpawnChance[2] = 0;
                enemySpawnChance[3] = 0;
            }
            break;

            case 5:
            if(curBoss == null) {
                gmScript.ChangeNotificationText(checkPointString[checkPoint]);
                checkPoint++;
                gmScript.EndLevel();

                //enemySpawnChance[0] = 50;
                //enemySpawnChance[1] = 75;
                //enemySpawnChance[2] = 80;
                //enemySpawnChance[3] = 75;
                //waveCoroutine = StartCoroutine(SpawnWave());
            }
            break;
        }

        if(playerShip == null && faded == false) {
            StopCoroutine(SpawnWave());
            gmScript.ChangeNotificationText("You dieded!", 1.5f);
            gmScript.EndLevel();
            faded = true;
        }
    }
}
