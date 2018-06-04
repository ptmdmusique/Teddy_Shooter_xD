using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : GeneralObject {

    [Header("Basic Info")]
    public float lifeTime = -1;

    [Header("Bullet")]
    public Vector3 fromWhere;
    public Vector3 toWhere;
    public bool hasTarget = false;
    public ForceMode2D myForceMode = ForceMode2D.Force;
    public bool fadeOut = false;
    public float rageReward = 1;
    public bool fromPlayer = false;
    public float startDelay = 0;
    public bool isDelay = true;

    override public void Awake() {
        base.Awake();
        
        //If the bullet has fade out option, let the animation control the Destroy()    
        if (fadeOut == false) {
            if (lifeTime >= 0) {
                Invoke("Die", lifeTime);
            }
        }   
    }
    public void Start() {
        StartObject(initialDir, mySpeed);
        Invoke("ChangeDelayState", startDelay);
    }

    public override void StartObject(Vector2 direction, float launchSpeed) {
        if (autoLaunch == true) {
            //Has target
            Vector2 velocity;
            if (hasTarget == true) {
                velocity = (toWhere - fromWhere).normalized * mySpeed;
            }
            else {

                //Go up as it is the default direction of ally bullet
                if (myAllianceState == AllianceState.Ally) {
                    if (initialDir == new Vector2(0, 0)) {
                        initialDir = Vector2.up;
                    }
                    velocity = initialDir * mySpeed;
                }
                else {
                    //Default sprite direction is up... so we need to flip :D
                    if (mySR != null) {
                        mySR.flipY = true;
                    }
                    if (initialDir == new Vector2(0, 0)) {
                        initialDir = Vector2.down;
                    }
                    velocity = initialDir * mySpeed;
                }
            }

            //Rotate. I hate quaternion...
            transform.eulerAngles = new Vector3(0, 0,
                    Mathf.Atan2(velocity.y / mySpeed, velocity.x / mySpeed) * Mathf.Rad2Deg - 90);
            Launch(velocity);
        }
    }
    virtual public void Launch(Vector2 velocity) {
        //Launch
        if (myForceMode == ForceMode2D.Impulse) {
            myRb.AddForce(velocity, myForceMode);
        }
        else {
            myRb.velocity = velocity * 1.5f;
        }
    }

    override public void Die() {
        CreateExplosion(transform.position);
        Destroy(gameObject);
    }

    public void CameraShake() {
        Transform camera = Camera.main.transform;
        CameraShake cameraShakeSript = camera.GetComponent<CameraShake>();
        cameraShakeSript.shakeDuration = 0.2f;
        cameraShakeSript.shakeAmount = 0.07f;
    }

    public override void OnImpact()
    {
        //Destroy the projectile afterwards
        CreateExplosion(transform.position);
        CameraShake();
        //CreateExplosion(transform.position);
        if (fromPlayer == true)
        {
            GameObject player = GameObject.Find("Player");
            if (player != null)
            {
                PlayerShip myPlayer = player.GetComponent<PlayerShip>();
                if (myPlayer != null)
                {
                    myPlayer.AddRage(rageReward);
                }
            }
        }
        if (maxHealth <= -1)
        {
            //Only die if there is no health
            Die();
        }
        else
        {
            //Take 25% of myDamage
            TakeDamage(myDamage * 0.25f);
            CheckHealthCondition();
        }
    }
    public virtual void OnTriggerEnter2D(Collider2D other) {
        GeneralObject otherScript = other.GetComponent<GeneralObject>();
        if (otherScript != null) {
            if (otherScript.maxHealth <= -1) {
                return;
            }

            if (otherScript.myAllianceState != AllianceState.Neutral) {
                if (otherScript.myAllianceState != myAllianceState) {
                    if (otherScript.isInvincible == false || throughInvicible == true) {
                        //If the object can be damaged then substract the health
                        otherScript.TakeDamage(myDamage);
                        OnImpact();
                    }
                }
            }
        }
    }

    public void ChangeDelayState() {
        isDelay = !isDelay;
    }
}
