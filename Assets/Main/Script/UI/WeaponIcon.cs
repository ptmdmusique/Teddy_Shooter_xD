using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponIcon : MonoBehaviour {

    public Sprite[] spriteList;
    public int curIndx = 0;

    private Image iconHolder;
    private Animator myAnim;

    // Use this for initialization
    void Awake () {
        myAnim = GetComponent<Animator>();
        iconHolder = GetComponent<Image>();

        if (iconHolder == null) {
            iconHolder = transform.Find("Icon Child").GetComponent<Image>();
        }
	}
	
    public void NextImage() {
        curIndx++;
        if (curIndx >= spriteList.Length) {
            curIndx = 0;
        }
        myAnim.SetTrigger("Change");
    }

    public void ChangeSprite() {
        iconHolder.sprite = spriteList[curIndx];
    }
}
