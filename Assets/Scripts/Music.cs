using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour {

    public AudioSource music;

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(transform.gameObject);		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PlayMusic() {
        if(music.isPlaying) return;
        music.Play();
    }

    public void StopMusic() {
        music.Stop();
    }
}
