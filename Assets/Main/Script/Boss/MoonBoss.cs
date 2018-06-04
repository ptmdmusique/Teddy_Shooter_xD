using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoonBoss : GeneralObject {

    [Header("Minion")]
    public List<Transform> minionType;
    public int maxMinion = 1;
    public float minionSpeed = 10;
    public List<Transform> minionList = new List<Transform>();
    public List<Transform> backupList = new List<Transform>();
    public float rebuildWait = 0.5f;
    public float scaleTime = 0.5f;

    [Header("Waves")]
    public List<Transform> waveObject;
    public int objectPerWave = 20;
    public int numberOfWave = 5;
    public float waveWaitTime = 2;
    public float objectWaitTime = 0.3f;
    
    [Header("Basic info")]
    public Transform target;
    public float flashTime = 1;
    public float waveWarningTime = 2;
    private Transform spriteHolder;
    private Vector2 xBoundary;
    private Vector2 yBoundary;
    private Vector2 xScreen;
    private Vector2 yScreen;
    private GameMaster gameMasterScript;
    private Collector objectMasterScript;
    public float innerRadius = 0.5f;
    public float outerRadius = 1;
    private Coroutine rebuildCoroutine;
    private Coroutine waveCoroutine;
    private Coroutine moveDecisionCoroutine;
    private Coroutine attackDecisionCoroutine;
    private Coroutine waveDecisionCoroutine;
    private int speedMilestone = 0;             //How many milestones have we reached for speed
    private int maxMinionMilestone = 0;         //How many milestones have we reached for minions

    public override void Awake() {
        base.Awake();
        //Get the collider and radius
        spriteHolder = transform.parent.Find("Sprite Holder");

        innerRadius = GetComponent<CircleCollider2D>().radius + 1;
        if (outerRadius <= innerRadius) {
            outerRadius = innerRadius + 2f;
        }

        //Get the target
        if (target == null) {
            target = GameObject.Find("Player").transform;
        }
    }

    public void Start() {       
        GetTheMaster();
    }

    public void GetTheMaster() {
        //Change the health indicator
        myHealthIndicator = GameObject.Find("UI Canvas").GetComponent<CanvasManager>().bossHealthIndicator;
        myHealthIndicator.gameObject.SetActive(true);
        myHealthIndicator.ChangeIcon(myIcon);
        //myHealthIndicator.RestartBar(0, maxHealth);

        GameObject objectMaster = GameObject.Find("Objects Master");
        GameObject gameMaster = GameObject.Find("Game Master");
        if (gameMaster != null) {
            gameMasterScript = gameMaster.GetComponent<GameMaster>();
            if (gameMasterScript != null) {
                xBoundary = gameMasterScript.xBoundary;
                yBoundary = gameMasterScript.yBoundary;
                xScreen = gameMasterScript.xScreen * GlobalState.xScreenClamp;
                yScreen = gameMasterScript.yScreen;
            }
        }

        if (objectMaster != null) {
            objectMasterScript = objectMaster.GetComponent<Collector>();
        }
    }

    public IEnumerator RebuildMinionList(int numOfChild) {
        if (numOfChild <= 0 || minionList.Count > 0) {
            yield break ;
        }

        for (int indx = minionList.Count - 1; indx >= 0; indx--) {
            if (minionList[indx] == null) {
                minionList.RemoveAt(indx);
            }
        }

        for (int indx = 0; indx < numOfChild; indx++) {
            int randomIndx = Random.Range(0, backupList.Count);
            Transform child = backupList[randomIndx];

            float randomRad = Random.Range(innerRadius, outerRadius);
            float randomAngle = Random.Range(0.0f, Mathf.PI);

            //Calculate x and y from radius and angle
            Vector3 spawnPos = new Vector3 (randomRad * Mathf.Cos(randomAngle),
                                            randomRad * Mathf.Sin(randomAngle),
                                            0);
            //Debug.Log(randomRad + "   " + randomAngle + "    " + Vector3.Distance(spawnPos, transform.position) + "     " + randomRad * Mathf.Cos(randomAngle) + "    " + randomRad * Mathf.Sin(randomAngle));

            spawnPos += transform.position;

            //Rebuild the minions randomly
            Transform newObject = Instantiate(child, spawnPos , Quaternion.identity);
                        
            //Delay the start
            OrbitAroundObject orbitScript = newObject.GetComponent<OrbitAroundObject>();
            if (orbitScript != null) {
                orbitScript.startDelay = scaleTime * 2;
            }

            //Scale with animation
            Vector3 initialScale = child.localScale;
            newObject.DOScale(Vector3.zero, 0.0001f);
            newObject.DOScale(initialScale, scaleTime).SetEase(Ease.OutElastic);

            newObject.gameObject.SetActive(true);
           
            //Set parent
            newObject.parent = spriteHolder;
            minionList.Add(newObject);
            yield return new WaitForSeconds(rebuildWait);
        }
        rebuildCoroutine = null;
    }

    public void LaunchMinion(Vector3 toWhere) {
        if (minionList.Count <= 0 || rebuildCoroutine != null || toWhere == Vector3.zero) {
            return;
        }

        foreach(Transform child in minionList) {
            //Launch the minion
            child.GetComponent<Rigidbody2D>().velocity = (toWhere - child.position).normalized * minionSpeed;

            OrbitAroundObject orbitScript = child.GetComponent<OrbitAroundObject>();
            if (orbitScript != null) {
                orbitScript.enabled = false;    //Stop it from orbiting around
            }

            //Put it directly into object master's list
            child.parent = null;
            objectMasterScript.AddChild(child);
        }

        minionList.Clear();
    }

    IEnumerator SpawnAsteroid() {
        for (int wave = 0; wave < Random.Range(1, numberOfWave + 1); wave++) {
            for (int indx = 0; indx < Random.Range(objectPerWave / 2, objectPerWave + 1); indx++) {
                Vector3 spawnPos = new Vector3(Random.Range(xScreen.x, xScreen.y), yBoundary.y, 0);
                int spawnIndx = Random.Range(0, waveObject.Count);
                Transform newObject = Instantiate(waveObject[spawnIndx], spawnPos, Quaternion.identity);
                GeneralObject generalScript = newObject.GetComponent<GeneralObject>();
                if (generalScript != null) {
                    StartCoroutine(StartAsteroid(generalScript));
                }

                objectMasterScript.AddChild(newObject);

                yield return new WaitForSeconds(objectWaitTime);
            }
            yield return new WaitForSeconds(waveWaitTime);
        }

        waveCoroutine = null;
    }

    IEnumerator StartAsteroid(GeneralObject script) {
        yield return new WaitForSeconds(0.05f);
        script.StartObject(script.initialDir, script.mySpeed * 5);
    }

    public void MoveToTarget(Vector3 target) {
        transform.DOMove(target, Vector3.Distance(target, transform.position) / mySpeed).SetEase(Ease.InOutQuad);
    }

    IEnumerator DecideToMove() {
        float chance = Random.Range(0.0f, 1f);
        if (chance >= 75) { 
            //No moving
            yield return new WaitForSeconds(1);
        } else {
            //Do move
            //Randomly choose a spot
            Vector3 target = new Vector3(Random.Range(xScreen.x, xScreen.y), transform.position.y, 0);

            //Then move to that spot
            MoveToTarget(target);

            yield return new WaitForSeconds(Vector3.Distance(target, transform.position) / mySpeed);
        }

        moveDecisionCoroutine = StartCoroutine(DecideToMove());
    }
    
    IEnumerator DecideToLaunch() {
        int chance = Random.Range(0, 100);

        if (chance <= 80 && rebuildCoroutine == null && minionList.Count <= 0) {
            rebuildCoroutine = StartCoroutine(RebuildMinionList(maxMinion));
        } else if (chance >= 50) {
            yield return new WaitForSeconds(1.5f);
        } else if (minionList.Count > 0 && target != null && rebuildCoroutine == null){
            //Flash and then attack
            AttackFlash();
            yield return new WaitForSeconds(flashTime);

            //Launch
            if (target != null) { 
                LaunchMinion(target.position);
            }
        }

        attackDecisionCoroutine = StartCoroutine(DecideToLaunch());
    }

    IEnumerator DecideToSpawnWave() {
        int chance = Random.Range(0, 100);

        if (curHealth <= 0.40f * maxHealth && chance <= 60 && waveCoroutine == null) {
            WaveWarningFlash();
            yield return new WaitForSeconds(waveWarningTime);   //Wait a little bit
            waveCoroutine = StartCoroutine(SpawnAsteroid());    //Spawn
            yield return new WaitForSeconds((objectPerWave * objectWaitTime + waveWaitTime) * numberOfWave);    //Wait until the wave is finish
        } else {
            yield return new WaitForSeconds(0.5f);
        }

        waveDecisionCoroutine = StartCoroutine(DecideToSpawnWave());
    }

    public void AttackFlash() {
        spriteHolder.GetComponent<Animator>().SetTrigger("Hurt");
    }

    public void WaveWarningFlash() {
        gameMasterScript.ChangeNotificationText("Asteroid Wave(s) Incoming!", waveWarningTime, 0);
    } 

    public void HealthCheck() {
        if ((curHealth <= 0.75f * maxHealth && speedMilestone == 0) || 
            (curHealth <= 0.6f * maxHealth && speedMilestone == 1) || 
            (curHealth <= 0.5f * maxHealth && speedMilestone == 2) || 
            (curHealth <= 0.25f * maxHealth && speedMilestone == 3) ||
            (curHealth <= 0.10f * maxHealth && speedMilestone == 4)
            ) {
            IncreaseSpeed(1.5f);
        }

        if ((curHealth <= 0.90f * maxHealth && maxMinionMilestone == 0) ||
            (curHealth <= 0.75f * maxHealth && maxMinionMilestone == 1) ||
            (curHealth <= 0.50f * maxHealth && maxMinionMilestone == 2) ||
            (curHealth <= 0.25f * maxHealth && maxMinionMilestone == 3) ||
            (curHealth <= 0.15f * maxHealth && maxMinionMilestone == 4) ||
            (curHealth <= 0.10f * maxHealth && maxMinionMilestone == 5)) {

            if (maxMinionMilestone == 3) {
                minionSpeed *= 1.5f;           //Increase minion speed after a while
            }

            IncreaseMaxMinion(1);
        }

        myHealthIndicator.SetValue(curHealth, maxHealth);
    }

    public void IncreaseSpeed(float percent) { 
        mySpeed *= percent;
        speedMilestone++;
    }

    public void IncreaseMaxMinion(int number) {
        maxMinion += number;
        maxMinionMilestone++;
    }

    private void FixedUpdate() {
        HealthCheck();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        GeneralObject otherScript = collision.GetComponent<GeneralObject>();
        if (otherScript != null) {
            if (otherScript.maxHealth <= -1) {
                return;
            }

            if (otherScript.myAllianceState != AllianceState.Neutral) {
                if (otherScript.myAllianceState != myAllianceState) {
                    if (otherScript.isInvincible == false || throughInvicible == true) {
                        //If the object can be damaged then substract the health
                        otherScript.TakeDamage(myDamage);
                    }
                }
            }
        }

        myHealthIndicator.StartShaking(0.5f);
    }

    public override void StartObject(Vector2 direction, float launchSpeed) {
        //Since there is a delay before we get the master, we need to check if we have get the masters or not
        float yDestination = yScreen.y;
        if(yDestination == 0) {
            yDestination = GlobalState.GetCameraYBound(mainCamera).y * GlobalState.yScreenClamp;
        } else {
            yDestination *= GlobalState.yScreenClamp;
        }

        //Move from top of the screen
        transform.parent.transform.DOMoveY(yDestination * 0.9f, 3).SetEase(Ease.Flash);
        StartCoroutine(InvisibleForSomeTime(3));                        //Be invicible for some time
        Invoke("StartAttack", 3);                                       //Then attack
        
    }

    public override void StartAttack() {
        moveDecisionCoroutine = StartCoroutine(DecideToMove());
        attackDecisionCoroutine = StartCoroutine(DecideToLaunch());
        waveDecisionCoroutine = StartCoroutine(DecideToSpawnWave());
    }
}
        