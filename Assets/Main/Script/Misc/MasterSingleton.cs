using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterSingleton : MonoBehaviour {
    private static MasterSingleton masterSingleton;

    void Awake() {

        if (masterSingleton != null && masterSingleton != this) {
            Destroy(this.gameObject);
            return;
        }

        masterSingleton = this;
        DontDestroyOnLoad(this.gameObject);

    }
}
