using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerShip : Spaceship {

    [Header("Master Stuff")]
    public Vector2 xScreen;
    public Vector2 yScreen;
    public Transform gameMaster;
    public GameMaster gMScript;

    [Header("Health and other Stats")]
    public float curScore = 0;
    private NumericStatDisplayer scoreStat;
    private float curRage = 0;
    private float maxRage = 100;

    [Header("Weapon Info")]
    //Weapon type: Single - Multi - Ulti
    private List<NormalGun> curSingle = new List<NormalGun>();
    public int activeSingle = 1;
    public int maxActiveSingle = 3;
    private NormalGun curMulti;
    private NormalGun curUlti;

    [Header("UI")]
    public StatusIndicator myRageIndicator;
    public StatusIndicator myUltimateIndicator;
    public WeaponIcon[] weaponIcons;

    //Child animator
    private Animator childAnim;

    new public void Start() {
        base.Start();

        //Find the Game Master
        gameMaster = GameObject.Find("Game Master").transform;
        gMScript = gameMaster.GetComponent<GameMaster>();

        GetBorder();
        //Add all weapon
        AddAllWeapon();
        
        canAttack = true;

        //Get the score
        if (scoreStat == null) {
            scoreStat = GameObject.Find("Score Parent").GetComponent<NumericStatDisplayer>();
        }
        if (scoreStat != null && scoreStat.targetText == null) {
            scoreStat.targetText = GameObject.Find("Score Parent").transform.Find("Stat").GetComponent<Text>();
        }

        //Get the animator of the child
        if (childAnim == null) {
            childAnim = transform.Find("TeddyShip").GetComponent<Animator>();
        }

        weaponIcons = new WeaponIcon[3];
        Transform weaponParent = GameObject.Find("Weapon Icons").transform;
        weaponIcons[0] = weaponParent.Find("Single Gun").GetComponent<WeaponIcon>();
        weaponIcons[1] = weaponParent.Find("Multi Gun").GetComponent<WeaponIcon>();
        weaponIcons[2] = weaponParent.Find("Ultimate").GetComponent<WeaponIcon>();

        //Change the default sprite base on the weapons
        weaponIcons[0].curIndx = curSingle[0].curIndx;
        weaponIcons[0].ChangeSprite();
        weaponIcons[1].curIndx = curMulti.curIndx;
        weaponIcons[1].ChangeSprite();
        weaponIcons[2].curIndx = 0;

    }

    void GetBorder() {
        xScreen = gMScript.xScreen * GlobalState.xScreenClamp;
        yScreen = gMScript.yScreen;
    }

	// Use this for initialization
    void FixedUpdate() {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, moveVertical, 0.0f);
        myRb.velocity = movement * mySpeed;

        myRb.position = new Vector3
        (
            Mathf.Clamp(myRb.position.x, xScreen.x, xScreen.y),
            Mathf.Clamp(myRb.position.y, yScreen.x, yScreen.y),
            0.0f
        );

        
    }

    new private void Update() {
        base.Update();

        InputController();

        //Change the status indicator if necessary
        myHealthIndicator.SetValue(curHealth, maxHealth);
        myRageIndicator.SetValue(curRage, maxRage);
    }

    void Fire() {
        if (Input.GetButton("SingleShot") == true) {
            switch (activeSingle)
            {
                case 1:
                    UseGun(curSingle[0]);
                    break;
                case 2:
                    UseGun(curSingle[1]);
                    UseGun(curSingle[2]);   
                    break;
                default:
                    for (int indx = 0; indx < activeSingle; indx++)
                    {
                        UseGun(curSingle[indx]);
                    }
                    break;
            }
            
        }
        if (Input.GetButton("MultiShot") == true) {
            UseGun(curMulti);
        }
        if (Input.GetButton("Ulti") == true) {
            UseGun(curUlti);
        }

    }

    void ChangeBullet() {
        if (Input.GetButtonDown("ChangeSingle") == true) {
            foreach(NormalGun singleGun in curSingle) {
                singleGun.NextBullet();
            }
            weaponIcons[0].NextImage();
        }
        if (Input.GetButtonDown("ChangeMulti") == true) {
            curMulti.NextBullet();
            weaponIcons[1].NextImage();
        }
    }

    //Controlling the user input except the movement
    void InputController() {
        ChangeBullet();
        Fire();
    }

    public override void AddActiveWeapon(int indx) {
        NormalGun weaponScript = weaponList[indx].GetComponent<NormalGun>();
        activeWeaponList.Add(weaponList[indx]);
        weaponScript.canFire = true;
        if (weaponScript.myTag.Contains("Ulti")) {
            curUlti = weaponScript;
        } else if (weaponScript.myTag.Contains("Single")){
            curSingle.Add(weaponScript);
        } else if (weaponScript.myTag.Contains("Multi")) {
            curMulti = weaponScript;
        }
    }

    //Guns
    public override void AddAllWeapon() {
        for (int indx = 0; indx < weaponList.Count; indx++) {
            AddActiveWeapon(indx);
        }
    }
    public override void InitializeWeaponList() {
        //Use this to initialize weapon list AND alliance state
        foreach (Transform child in transform) {
            GeneralObject childScript = child.GetComponent<GeneralObject>();
            if (childScript != null) {
                childScript.myAllianceState = myAllianceState;
                NormalGun childWeaponScript = child.GetComponent<NormalGun>();
                if (childWeaponScript != null) {
                    childWeaponScript.fromPlayer = true;
                    if (childScript.GetComponent<UltimateWeapon>() != null) {
                        ultimateList.Add(child);
                        child.GetComponent<NormalGun>().canFire = false;                    
                    }
                    else {
                        weaponList.Add(child);
                        child.GetComponent<NormalGun>().canFire = false;
                    }
                }
            }
        }

    }
    public override void UseGun(NormalGun curGun) {      
        if (canAttack == true && curGun != null && curGun.fireCoroutine == null) {
            float rageCost = curGun.bulletList[curGun.curIndx].rageCost;
            if (curRage >= rageCost) {
                curRage = ClampRage(curRage - rageCost);
                curGun.fireCoroutine = StartCoroutine(curGun.Fire());
            }
        }
    }
    public void ChangeActiveSingle() {
        activeSingle++;
        if (activeSingle > maxActiveSingle) {
            activeSingle = 1;
        }
    }

    //Rage
    public float ClampRage(float value) {
        return Mathf.Clamp(value, 0, maxRage);
    }
    public void AddRage(float value) {
        curRage = ClampRage(value + curRage);
    }

    //Score
    public void AddScore(float value) {
        curScore += value;
        scoreStat.AddPoints(value);
    }

    //Misc stuff
    public override void OnTriggerEnter2D(Collider2D collision) {
        GeneralObject colScript = collision.GetComponent<GeneralObject>();
        if (colScript != null) {
            if (colScript.myAllianceState != myAllianceState) {
                if (colScript.isInvincible == false || throughInvicible == true) {
                    //If the object can be damaged then substract the health
                    colScript.TakeDamage(myDamage);
                    //If make contact with enemy, then trigger the red flash animation
                    if (colScript.myTag.Contains("Collectible") == false) { 
                        childAnim.SetTrigger("IsHurt");
                    }
                    myHealthIndicator.StartShaking(0.5f);
                }
            }
        }
    }


}
