using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AllianceState {
    Ally = 0,
    Enemy = 1,
    Neutral = 2
};

public enum ShotType {
    Straight = 0,
    Spread = 1,
    Path = 2
}

public enum SpreadType {
    pattern = 0,
    random = 1
}

public enum WeaponType {
    singleShot = 0,
    multiShot = 1
}

public enum EvasionLevel {
    NoEvasion = 0,
    Easy = 1,
    Medium = 2,
    Hard = 3
}

public enum SpecialTarget {
    None = 0,
    Mouse = 1,
    Player = 2
}

public enum DistributionType {
    SingleType = 0,         //Only use a single type of enemy
    Evenly = 1,             //One type to another
}

public enum FormationSpawnType {
    Custom = 0,
    TopDown = 1,
    LeftRight = 2,
    OneSide = 3
}

[System.Serializable]
public class NotificationString {
    public string myString = "";
    public float waitTime = 0;
    public NotificationText.TextState textState = 0;
}

public class GlobalState : MonoBehaviour {

    //Map value from one range to another
    static public float Map(float in_min, float in_max, float out_min, float out_max, float x) {
        return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }

    static public float xScreenClamp = 0.8f;
    static public float yScreenClamp = 0.8f;

    //Find the player
    static public Transform FindPlayer() {
        GameObject player = GameObject.Find("Player");
        if (player != null) {
            return player.transform;
        }
        return null;
    }

    static public Vector3 GetMousePosition(Camera targetCamera) {
        Vector3 mouseOnScreen = targetCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
            Input.mousePosition.y,
            Input.mousePosition.z - targetCamera.transform.position.z));
        return mouseOnScreen;
    }

    static public float GetCameraHeight(Camera targetCamera) {
        //Ortho size = height / 2
        return targetCamera.orthographicSize * 2;
    }

    static public float GetCameraWidth(Camera targetCamera) {
        //Aspect ratio = width / height
        float prefWidth = 1680;
        float prefHeight = 2000;
        return targetCamera.orthographicSize * 2.0f * prefWidth / prefHeight;
    }
     
    static public Vector2 GetCameraXBound(Camera targetCamera) {
        return new Vector2(targetCamera.transform.position.x - GetCameraWidth(targetCamera) / 2, targetCamera.transform.position.x + GetCameraWidth(targetCamera) / 2);
    }

    static public Vector2 GetCameraYBound(Camera targetCamera) {
        return new Vector2(targetCamera.transform.position.y - GetCameraHeight(targetCamera) / 2, targetCamera.transform.position.x + GetCameraHeight(targetCamera) / 2);
    }
}
