using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMusicPlayer : MonoBehaviour {
	private Controller controlScript;
	private AudioSource source;
	private float startTimer, waitToStart, songTime;
	private bool startMusic, pausing;

	// Use this for initialization
	void Start () {
		source = GetComponent<AudioSource> ();
		controlScript = GameObject.Find ("Controller").GetComponent<Controller> ();
		startTimer = 0;
		songTime = 0;
		waitToStart = 6;
		pausing = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(controlScript == null){
			controlScript = GameObject.Find ("Controller").GetComponent<Controller> ();
		}
		if (!startMusic) {
			startTimer += Time.deltaTime;
		}else{
			if(!pausing){
				songTime += Time.deltaTime;
				if(songTime >= source.clip.length){
					startMusic = false;
				}
			}
		}

		if(startTimer >= waitToStart){
			startMusic = true;
			startTimer = 0;
			waitToStart = 3;
			playMusic ();
		}

		if(controlScript.canAudioPause){
			source.Pause ();
			pausing = true;
		}else{
			if(pausing){
				source.Play ();
				pausing = false;
			}
		}

		if(controlScript.gameOver){
			fadeAudio ();
		}
	}

	void playMusic(){
		source.Play ();
		songTime = 0;
	}

	void fadeAudio(){
		while(source.volume > 0){
			source.volume -= .001f;
		}
	}
}
