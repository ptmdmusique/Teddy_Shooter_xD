using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spaceship : GeneralObject {

    [Header("Ship Info")]
    public float startDelay = 0;
    //Weapon things
    public List<Transform> weaponList = new List<Transform>();          //Hold normal weapons
    public List<Transform> ultimateList = new List<Transform>();        //Hold ultimates
    public List<Transform> activeWeaponList = new List<Transform>();    //Hold active weapons                                     //Hold the index of current ultimate 
    public bool autoFire = false;
    public bool useAllWeapon = false;
    
    //Evasion
    public EvasionLevel myEvasionLevel = EvasionLevel.NoEvasion;
    //public float tilt;
    //public float dodge;
    //public float smoothing;
    //public Vector2 startWait;
    //public Vector2 maneuverTime;
    //public Vector2 maneuverWait;
    //public IEnumerator evadeCoroutine;
    
    // Use this for initialization
    override public void Awake () {
        base.Awake();
        if (weaponList.Count == 0) {
            InitializeWeaponList();
        }

        //First gun is always active by default      
        if (weaponList.Count > 0 && activeWeaponList.Count <= 0 && name != "Player") {
            if (useAllWeapon == true) {
                AddAllWeapon();         //Add all weapon to active list if the ship can use all weapon at the same time
            }
            else { 
                AddActiveWeapon(0);
            }
        }
    }

    public void Start() {
        if (autoLaunch == true) {
            StartCoroutine(StartObjectCoroutine());
        }
    }

    new void Update() {
        base.Update();

        canAttack = CheckAttackCondition();
        if (autoFire == true && canAttack == true) {
            if (useAllWeapon == true) { 
                UseAllActiveGun();
            } else {
                UseGun(activeWeaponList[0].GetComponent<NormalGun>());
            }
        }
    }

    //Weapon stuff
    public virtual void UseGun(NormalGun curGun) {
        if (canAttack == true && curGun != null && curGun.fireCoroutine == null) {
            curGun.fireCoroutine = StartCoroutine(curGun.Fire());
        }
    }

    public void UseAllActiveGun() {
        foreach (Transform child in activeWeaponList) {
            UseGun(child.GetComponent<NormalGun>());
        }
    }

    public virtual void InitializeWeaponList() {
        //Use this to initialize weapon list AND alliance state
        foreach (Transform child in transform) {
            GeneralObject childScript = child.GetComponent<GeneralObject>();
            if (childScript != null) {
                childScript.myAllianceState = myAllianceState;
                if (childScript.myTag.Contains("Weapon") == true) {
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

	public virtual void AddActiveWeapon(int indx) {
        NormalGun weaponScript = weaponList[indx].GetComponent<NormalGun>();
        activeWeaponList.Add(weaponList[indx]);
        weaponScript.canFire = true;
    }

    public virtual void AddAllWeapon() {
        for (int indx = 0; indx < weaponList.Count; indx++) {
            AddActiveWeapon(indx);
        }
    }

    public void RemoveActiveWeapon(int indx) {
        NormalGun weaponScript = weaponList[indx].GetComponent<NormalGun>();
        activeWeaponList.Remove(weaponList[indx]);
        weaponScript.canFire = false;
    }

    public void CleanActiveWeaponList() {
        activeWeaponList.RemoveAll(activeWeaponList => activeWeaponList.childCount > 0);
    }

    //Can we attack?
    public virtual bool CheckAttackCondition() {
        if (OnScreen() == true && isStarted == true) {
            return true;
        }
        return false;
    }  

    //Start the ship
    public override void StartObject(Vector2 direction, float launchSpeed) {
        
        Vector3 velocity = new Vector2(0, 0);
        if (direction != new Vector2(0, 0)) {
            velocity = direction * launchSpeed;
            //Go up as it is the default direction of ally ship
        } else {
            if (myAllianceState == AllianceState.Ally) {
                direction = Vector2.up;
                velocity = direction * launchSpeed;
            }
            else {                
                direction = Vector2.down;
                velocity = direction * launchSpeed;
            }
        }

        if (autoLaunch == true) {
            //Rotate to direction base on the initial direction (velocity is created from initial direction)
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y,
                Mathf.Atan2(velocity.y / launchSpeed, velocity.x / launchSpeed) * Mathf.Rad2Deg - 90 + transform.eulerAngles.z);
            //Dang Quaternion...
        } else {
            if (myAllianceState == AllianceState.Enemy) {
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 180);
            }
        }

        if (initialDir == Vector2.zero) {
            initialDir = direction;
        }
        myRb.velocity = velocity;

        isStarted = true;
    }

    IEnumerator StartObjectCoroutine() {
        yield return new WaitForSeconds(startDelay);
        StartObject(initialDir, mySpeed);
    }

    public virtual void OnTriggerEnter2D(Collider2D collision) {
        GeneralObject colScript = collision.GetComponent<GeneralObject>();
        if (colScript != null) {
            if (colScript.myAllianceState != myAllianceState) {
                if (colScript.isInvincible == false || throughInvicible == true) {
                    //If the object can be damaged then substract the health
                    colScript.TakeDamage(myDamage);
                    
                }
            }
        }
    }

    //public virtual IEnumerator LowLevelEvasion() {
    //    yield return new WaitForSeconds(Random.Range(startWait.x, startWait.y));
    //    while (true) {
    //        targetManeuver = Random.Range(1, dodge) * -Mathf.Sign(transform.position.x);
    //        yield return new WaitForSeconds(Random.Range(maneuverTime.x, maneuverTime.y));
    //        targetManeuver = 0;
    //        yield return new WaitForSeconds(Random.Range(maneuverWait.x, maneuverWait.y));
    //    }
    //}
}
