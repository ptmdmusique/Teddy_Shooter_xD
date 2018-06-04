using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalGun : GeneralObject {

    [Header("Master Stuff")]
    public Transform bulletMaster;
    public Collector bulletMasterScript;

    [Header("Bullet Info")]
    public int curIndx = 0;
    public bool fromPlayer = false;

    [System.Serializable]
    public class Bullet {
        public float fireRate = 5;          //Bullet per second: 1/fireRate = reloadTime
        public int bulletPerWave = 3;
        public float waveWaitTime = 3;
        public float waitAfterWave = 0.5f;
        public int numberOfWave = 1;
        public int bulletBeforeWait = -1;        
        public bool useCustomColor = false;
        public Color bulletOverlapColor = Color.white;
        public float shotForce = 20;
        public float damage = 10;
        public float rageCost = 10;
        public float rageReward = -1;
        public float lifeTime = -1;
        public float startDelay = 0;
        public ForceMode2D forceMode = ForceMode2D.Force;
        public float speed = -1;

        public Transform bullet;
        public Projectile bulletScript;

        void Start() {
            bulletScript = bullet.GetComponent<Projectile>();
        }
    }
    [Header("Bullet List")]
    public List<Bullet> bulletList;

    [Header("Gun Info")]
    public ShotType curShotType;
    public SpreadType curSpreadType;
    public WeaponType myWeaponType;
    public Transform gunNozzle;
    public Transform aimPoint;
    public Transform gunLight;
    public bool autoFire = false;
    public bool fireFromStart = false;
    public bool canFire = true;
    public Coroutine fireCoroutine;

    [Header("For circleShot")]
    public float startAngle;
    public float endAngle;
    public float radius = -1;
    
   public void Start() {
        if (fireFromStart == true) {
            fireCoroutine = StartCoroutine(Fire());
        }

        if (gunNozzle == null) {
            gunNozzle = transform.Find("Nozzle");
        }
        
        if (aimPoint == null) {
            aimPoint = transform.Find("AimPoint");
        }

        if (gunLight == null) {
            gunLight = transform.Find("GunLight");
        }
        if (gunLight != null) { 
            gunLight.gameObject.SetActive(false);
        }

        if (radius == -1) {
            radius = Vector3.Distance(aimPoint.position, transform.position);
        }

        //Get the MASTER!
        bulletMaster = GameObject.Find("Bullets Master").transform;
        bulletMasterScript = bulletMaster.GetComponent<Collector>();
    }

    virtual public IEnumerator Fire() {
        if (canFire == true) {
            int bulletIndx = this.curIndx;
            Bullet curBullet = bulletList[bulletIndx];
            for (int wave = 0; wave < curBullet.numberOfWave; wave++) {
                if (curShotType == ShotType.Straight) {
                    //Shot in a straight line
                    myAnimator.SetFloat("FireRate", curBullet.fireRate / 5);
                    myAnimator.SetBool("Firing", true);
                    if (gunLight != null) {
                        gunLight.gameObject.SetActive(true);
                    }

                    for (int indx = 0; indx < curBullet.bulletPerWave; indx++) {
                        //Fire a chain of bullet
                        myAnimator.SetTrigger("Charge");
                        FireBullet(bulletIndx, aimPoint);
                        if (myWeaponType != WeaponType.multiShot
                                || (myWeaponType == WeaponType.multiShot && curBullet.bulletBeforeWait > 0 && (indx + 1) % curBullet.bulletBeforeWait == 0)) {
                            yield return new WaitForSeconds(Random.Range(1 / curBullet.fireRate * 0.75f, 1 / curBullet.fireRate));
                        }
                    }
                    //Set the animation back, if there is one
                    myAnimator.SetBool("Firing", false);
                    if (gunLight != null) {
                        gunLight.gameObject.SetActive(false);
                    }
                }
                else if (curShotType == ShotType.Spread) {
                    //Swap startAngle and endAngle is something mess up
                    if (startAngle > endAngle) {
                        float temp = startAngle;
                        startAngle = endAngle;
                        endAngle = temp;
                    }
                    //Recalculate the angle each time
                    float curStartAngle = startAngle + transform.eulerAngles.z;
                    float curEndAngle = endAngle + transform.eulerAngles.z;

                    //Shot
                    myAnimator.SetFloat("FireRate", curBullet.fireRate / 5);
                    myAnimator.SetBool("Firing", true);
                    if (gunLight != null) {
                        gunLight.gameObject.SetActive(true);
                    }

                    if (curBullet.bulletPerWave > 1) {
                        float angleFrag;
                        angleFrag = (float)(Mathf.Abs(curEndAngle - curStartAngle)) / (float)(curBullet.bulletPerWave - 1);
                        //Store the aim point position to set it back after firing
                        Vector3 tmpAimPos = aimPoint.position;
                        for (int indx = 0; indx < curBullet.bulletPerWave; indx++) {
                            float angle = 0;

                            if (curSpreadType == SpreadType.random) {
                                angle = Random.value * 360;
                            }
                            else {
                                angle = indx * angleFrag + curStartAngle;
                            }

                            Vector3 aimPos;
                            aimPos.x = transform.position.x + radius * Mathf.Cos(angle * Mathf.Deg2Rad);
                            aimPos.y = transform.position.y + radius * Mathf.Sin(angle * Mathf.Deg2Rad);
                            aimPos.z = 0;

                            aimPoint.position = aimPos;
                            FireBullet(bulletIndx, aimPoint);
                            //Wait for the next bullet (only wait if not multishot)
                            if (myWeaponType != WeaponType.multiShot 
                                || (myWeaponType == WeaponType.multiShot && curBullet.bulletBeforeWait > 0 && (indx + 1) % curBullet.bulletBeforeWait == 0)) { 
                                yield return new WaitForSeconds(Random.Range(1 / curBullet.fireRate * 0.75f, 1 / curBullet.fireRate));
                            }
                        }
                        aimPoint.position = tmpAimPos;  //Reverse back to the original position
                    }
                    else {
                        //If there is only 1 bullet, then just shoot straight
                        FireBullet(bulletIndx, aimPoint);
                    }

                    myAnimator.SetBool("Firing", false);
                    if (gunLight != null) {
                        gunLight.gameObject.SetActive(false);
                    }
                }

                yield return new WaitForSeconds(Random.Range(curBullet.waveWaitTime * 0.75f, curBullet.waveWaitTime));  //Control the spawn rate of firing
            }
            yield return new WaitForSeconds(Random.Range(curBullet.waitAfterWave * 0.75f, curBullet.waitAfterWave));     //Control the spawn rate of firing

            if (autoFire == true) {
                //Start the coroutine again if autofire is true
                fireCoroutine = StartCoroutine(Fire());
            }
            else {
                //Else, reset the coroutine to null after the wait time is over, so that the player won't fire rapidly
                fireCoroutine = null;
            }
        }
    }

    public void FireBullet(int bulletIndx, Transform toWhere = null) {
        Bullet bulletToShoot = bulletList[bulletIndx];
        Transform curBullet = Instantiate(bulletToShoot.bullet, gunNozzle.position, Quaternion.identity).transform;
        Projectile curBulletScript = curBullet.GetComponent<Projectile>();

        curBulletScript.fromWhere = gunNozzle.position;         //Fire from the nozzle
        curBulletScript.mySpeed = bulletToShoot.shotForce;      //Shot with some certain force
        if (bulletToShoot.damage != -1) { 
            curBulletScript.myDamage = bulletToShoot.damage;    //Set the damage as the gun's damage
        }
        curBulletScript.fromPlayer = fromPlayer;                //Is the bullet fired from the player?
        if (bulletToShoot.lifeTime != -1) { 
            curBulletScript.lifeTime = bulletToShoot.lifeTime;      //Set the life time
        }
        curBulletScript.myForceMode = bulletToShoot.forceMode;
        if (bulletToShoot.speed > 0) {
            curBulletScript.mySpeed = bulletToShoot.speed;
        }
        curBulletScript.startDelay = bulletToShoot.startDelay;  //Set the delay before we actually start

        bulletMasterScript.AddChild(curBullet);                 //Add the bullet to the bullet collector

        if (bulletToShoot.useCustomColor == true) {             //Change the color of the bullet
            curBulletScript.ChangeColor(bulletToShoot.bulletOverlapColor);
        }
        if(bulletToShoot.rageReward > -1) {
            curBulletScript.rageReward = bulletToShoot.rageReward;
        }

        //Dynamically set alliance state for the bullet
        GeneralObject bulletScript = curBullet.GetComponent<GeneralObject>();
        if (bulletScript != null) {
            bulletScript.myAllianceState = myAllianceState;
        }

        if (toWhere != null && curBulletScript.hasTarget == false) {
            curBulletScript.toWhere = toWhere.position;
            curBulletScript.hasTarget = true;
        }
    }

    public void SetIndx(int newIndx) {
        //Only change the weapon if the desinated index is valid
        if (newIndx >= 0 && newIndx < bulletList.Count) {
            curIndx = newIndx;
        }
    }

    public void NextBullet() {
        if (curIndx < bulletList.Count - 1) {
            curIndx++;
        } else {
            curIndx = 0;
        }
    }

    public void PrevBullet() {
        if (curIndx > 0) {
            curIndx--;
        }
        else {
            curIndx = bulletList.Count - 1;
        }
    }
}
