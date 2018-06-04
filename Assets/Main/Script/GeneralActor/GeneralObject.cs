using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralObject : MonoBehaviour {

    [Header("Master Stuff")]
    protected Collector vfxMasterScript;

    public string myTag;
    public bool isInvincible = false;
    public bool throughInvicible = false;
    public AllianceState myAllianceState;
    public List<Transform> myExplosion;
    public Sprite myIcon;

    [Header("Info")]
    public float maxHealth = -1;
    public float curHealth;
    public StatusIndicator myHealthIndicator;
    public float myDamage;
    public float myValue = 10;
    public Transform destroyParent = null;
    public Color defaultColor = Color.white;
    public bool autoLaunch = true;
    //Speed things
    public float mySpeed = 5;
    public Vector2 initialDir = new Vector2(0, 0);
    public bool canAttack = false;
    public bool isStarted = false;

    //Components
    protected Rigidbody2D myRb;
    protected Collider2D myCollider;
    protected Animator myAnimator;
    protected SpriteRenderer mySR;
    protected Camera mainCamera;

	// Use this for initialization
	virtual public void Awake () {
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        myRb = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<Collider2D>();
        myAnimator = GetComponent<Animator>();

        mySR = GetComponent<SpriteRenderer>();
        ChangeColor(defaultColor);

        curHealth = maxHealth;

        //Get the MASTA!
        vfxMasterScript = GameObject.Find("VFXs Master").GetComponent<Collector>();
    }
	
    //Clamp the health to [0, maxHealth]
	float ClampHealth(float parm) {
        return Mathf.Clamp(parm, 0, maxHealth);
    }

    //Set CurHealth to parm
    void SetCurHealth(float parm) {
        curHealth = ClampHealth(parm);
    }

    //Add health (use negative to substract health)
    public void AddHealth(float parm) {
        curHealth = ClampHealth(curHealth + parm); 
    }

    virtual public void Die() {
        CreateExplosion(transform.position);
        
        //Check the health for the indicator purpose
        //CheckHealthCondition();
        if(myTag.Contains("Boss") && myHealthIndicator != null) {
            myHealthIndicator.gameObject.SetActive(false);
        }

        if (myAllianceState == AllianceState.Enemy) {
            GameObject player = GameObject.Find("Player");
            PlayerShip playerScript = null;
            if (player != null) { 
                playerScript = player.GetComponent<PlayerShip>();
            }
            if (playerScript != null) {
                playerScript.AddScore(myValue);
            }
        }
        if (destroyParent == null) { 
            Destroy(gameObject);
        } else {
            Destroy(destroyParent.gameObject);
        }
    }

    public virtual void CreateExplosion(Vector3 position) {
        if (myExplosion.Count > 0) {
            foreach (Transform explosion in myExplosion) {
                if (explosion == null) {
                    return;
                }
                Transform curExplosion = Instantiate(explosion, position, Quaternion.identity);
                if (vfxMasterScript != null) {
                    vfxMasterScript.AddChild(curExplosion);
                }
            }
        }
    }

    virtual public void CheckHealthCondition() {
        if (curHealth <= 0) {
            Die();
        }
    }
    
    virtual public void TakeDamage(float damage) {
        AddHealth(-damage);
    }

    protected void Update() {
        if (maxHealth > -1) {
            CheckHealthCondition();
        }

        //If the object is not visible then ... be invicible!
        if (OnScreen() == false) {
            isInvincible = true;
        } else {
            isInvincible = false;
        }
    }

    //Are we inside the view?
    protected bool OnScreen() {
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(transform.position);
        bool onScreen = screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
        return onScreen;
    }

    public void ChangeColor(Color color) {
        if (mySR != null) {
            mySR.color = color;
        } else {
            defaultColor = color;
        }
    }

    //Use this to start flying
    public virtual void StartObject(Vector2 direction, float launchSpeed) {
    }

    //Be invicible in a certain time
    public IEnumerator InvisibleForSomeTime(float time) {
        isInvincible = true;

        yield return new WaitForSeconds(time);

        isInvincible = false;
    }

    public virtual void OnImpact()
    {
        
    }

    public virtual void StartAttack() {}
}

