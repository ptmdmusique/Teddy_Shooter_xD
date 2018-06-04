using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaBullet : Projectile {

    [Header("Area damage")]
    public float effectRadius = 3;
    public float impactDamagePercentage = 0.5f;
    private bool impactCall = false;

    public override void OnTriggerEnter2D(Collider2D other) {
        GeneralObject otherScript = other.GetComponent<GeneralObject>();
        if (otherScript != null) {
            if (otherScript.maxHealth <= -1) {
                return;
            }

            if (otherScript.myAllianceState != AllianceState.Neutral) {
                if (otherScript.myAllianceState != myAllianceState) {
                    if (otherScript.isInvincible == false || throughInvicible == true) {
                        AreaImpact();
                        OnImpact();
                    }
                }
            }
        }
    }

    public void AreaImpact() {
        if (impactCall == true) {
            //Already called this
            return;
        }

        impactCall = true;
        //Search for all enemy in range and then make them take damage
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, effectRadius);

        foreach (Collider2D hitObj in colliders) {
            GeneralObject hitScript = hitObj.GetComponent<GeneralObject>();
            if (hitScript != null) {
                if (hitScript.myAllianceState != AllianceState.Neutral) {
                    if (hitScript.myAllianceState != myAllianceState) {
                        if (hitScript.isInvincible == false || throughInvicible == true) {
                            if (hitScript.myTag.Contains("Projectile") == false) {
                                hitScript.TakeDamage(myDamage * impactDamagePercentage);
                                CreateExplosion(hitScript.transform.position);
                            }
                        }
                    }
                }
            }
        }
    }

    public override void Die() {
        AreaImpact();
        base.Die();
    }
}
