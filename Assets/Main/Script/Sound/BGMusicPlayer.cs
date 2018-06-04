using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMusicPlayer : MonoBehaviour {

    //Audio stuff
    public AudioClip[] myAudioClips;
    private AudioSource myAudioSource;
    private int curIndx = 0;
    public bool shuffle = false;
    private bool isPlaying = false;

    //Singleton stuff
    private static BGMusicPlayer musicInstance;

    void Awake() {
        //Singleton stuff
        if (musicInstance != null && musicInstance != this) {
            Destroy(this.gameObject);
            return;
        }

        musicInstance = this; 
        myAudioSource = GetComponent<AudioSource>();

    }

    // Use this for initialization
    void Start () {
        if (isPlaying == false) {
            //Haven't started yet
            if (shuffle == true) {
                ShuffleDeck();
            }

            //Play the track
            NextTrack();
        }
    }
	
	public void ShuffleDeck() {
        //Knuth unbiased algorithm
        for (int indx = 0; indx < myAudioClips.Length; indx++) {
            AudioClip temp = myAudioClips[indx];
            int indx2 = Random.Range(indx, myAudioClips.Length);
            myAudioClips[indx] = myAudioClips[indx2];
            myAudioClips[indx2] = temp;
        }
    }

    public void NextTrack() {
        myAudioSource.clip = myAudioClips[curIndx++];
        myAudioSource.Play();

        if (curIndx >= myAudioClips.Length) {
            curIndx = 0;
        }

        Invoke("NextTrack", myAudioSource.clip.length);
    }
}
