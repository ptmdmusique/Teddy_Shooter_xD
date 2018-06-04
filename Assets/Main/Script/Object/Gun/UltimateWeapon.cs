using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltimateWeapon : NormalGun {

    [Header("Ultimate Info")]
    public StatusIndicator myStatusIndicator;
    public float maxCharge;
    public float curCharge;
    public bool isActive = false;
    public float chargeRate = 0;

    public void UpdateStatus() {
        myStatusIndicator.SetValue(curCharge, maxCharge);
    }

    new private void Update() {
        base.Update();
        if (isActive == true) {
            //Increase the charge every frame
            curCharge = ClampCharge(curCharge + chargeRate);
            UpdateStatus();
            
            if (curCharge == maxCharge) {
                canFire = true;
            }
        }
    }

    public float ClampCharge(float value) {
        return Mathf.Clamp(value, 0, maxCharge);
    }

    public void UseUltimate() {
        if (canFire == true && curCharge == maxCharge) {
            curCharge = 0;
            UpdateStatus();
            canFire = false;

            Fire();
        }
    }
}
