using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusIndicator : MonoBehaviour {

    [SerializeField]
    private RectTransform statusRect;
    public Image statusImage;
    public SpriteRenderer statusIcon;
    private Sprite tempIcon;
    [SerializeField]
    private Text statusText;

    public Vector3 offset;
    public bool followTarget = false;
    public Transform target;
    public float lerpSpeed = 2;
    public string statusName;
    private Animator myAnim;
    [Header("Image Alpha Effect")]
    public bool mapAlpha = false;               //Do we increase object's alpha as value increase
    public bool inverseAlpha = false;           //Value increase -> alpha increase
    public float alphaMultiplier = 1;           //More intense?
    private Color targetAlpha = Color.white;
    private float targetVal = -1;

    [Header("Image Color Effect")]
    public bool mapColor = false;
    private Color targetColor = Color.white;
    public Color destColor = Color.red;
    
    [Header("Shaking")]
    public float time = 0.5f;
    public bool isShaking = false;
    public float tumble = 0.1f;
    private Vector2 initialPos;

    private void Awake() {
        Invoke("FindInitialPos", 0.1f);
        myAnim = GetComponent<Animator>();
    }

    private void Start() {
        SetTarget(target);
    }

    private void Update() {
        if (followTarget == true) {
            transform.position = offset + target.position;
        }

        //Fill the bar
        if (targetVal > 0) { 
            statusImage.fillAmount = Mathf.Lerp(statusImage.fillAmount, targetVal, Time.deltaTime * lerpSpeed);
        } else {
            statusImage.fillAmount = 0;
        }

        //Set up image color before the alpha so that the alpha value will override the color
        if (mapColor == true) {
            statusImage.color = targetColor;
        }

        //Set up alpha
        if (mapAlpha == true) {
            statusImage.color = targetAlpha;
        }        

        if (isShaking == true) {
            Vector3 newPos = initialPos + Random.insideUnitCircle * tumble;
            transform.position = newPos;
        }
    }

    public void SetValue(float curValue, float maxValue) {
        targetVal = curValue / maxValue;

        //Scale image
        //healthBarRect.localScale = new Vector3(value, healthBarRect.localScale.y, healthBarRect.localScale.z);

        //Set up alpha
        if (mapAlpha == true) {
            targetAlpha = statusImage.color;
            if (inverseAlpha == false) {
                float newValue = GlobalState.Map(0, maxValue, 0, 1, curValue * alphaMultiplier);
                targetAlpha.a = Mathf.Lerp(targetAlpha.a, newValue, Time.deltaTime * lerpSpeed * 10);
                if (curValue == 0) {
                    targetAlpha.a = 0;
                }
            } else {
                float newValue = GlobalState.Map(0, maxValue, 0, 1, (maxValue - curValue) * alphaMultiplier);
                targetAlpha.a = Mathf.Lerp(targetAlpha.a, newValue, Time.deltaTime * lerpSpeed * 10);
                if (curValue == 0) {
                    targetAlpha.a = maxValue;
                }
            }
        }

        if (mapColor == true) {
            float newValue = GlobalState.Map(0, maxValue, 0, 1, curValue);
            targetColor = Color.Lerp(targetColor, destColor * newValue, Time.deltaTime * lerpSpeed * 10);
            if (curValue == 0) {
                targetColor = destColor;
            }
        }

        if (statusText != null) { 
            statusText.text = curValue + "/" + maxValue + " " + statusName;
        }
    }

    //Set up new target
    public void SetTarget(Transform newTarget) {
        if (newTarget != null) {
            offset = transform.position - newTarget.position;
        }
    }

    public void StartShaking() {
        isShaking = true;
        Invoke("ReturnBack", time);
    }

    public void StartShaking(float time) {
        isShaking = true;
        Invoke("ReturnBack", time);
    }

    public void ReturnBack() {
        isShaking = false;
        transform.position = initialPos;
    }

    public void FindInitialPos() {
        initialPos = transform.position;
    }

    public void ChangeIcon() {
        if (tempIcon != null) {
            statusIcon.sprite = tempIcon;
        }

        tempIcon = null;
    }

    public void ChangeIcon(Sprite newIcon) {
        myAnim.SetTrigger("Change");
        tempIcon = newIcon;
        Invoke("ChangeIcon", 0.20f);
    }
    
    public void RestartBar(float curValue, float maxValue) {
        statusImage.fillAmount = 0;
        StartCoroutine(RestartCoroutine(curValue, maxValue));
    }

    public IEnumerator RestartCoroutine(float curValue, float maxValue) {
        SetValue(curValue, maxValue);
        yield return new WaitForSeconds(0.01f);

        if (statusImage.fillAmount >= 1) {
            targetVal = maxValue;
            yield return null;
        } else {
            //Move up 10% of the bar
            StartCoroutine(RestartCoroutine(curValue + maxValue / 20 , maxValue));
        }
    }
}
