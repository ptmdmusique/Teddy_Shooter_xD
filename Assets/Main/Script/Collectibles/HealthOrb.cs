using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthOrb : Collectibles {

    public float healthValue = 50;

    public override void OnCollected(Transform target) {
        GeneralObject targetScript = target.GetComponent<GeneralObject>();
        if (targetScript != null) {
            targetScript.AddHealth(healthValue);
        }
        Destroy(gameObject);
    }
}
