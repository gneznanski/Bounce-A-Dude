using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMusicLooper : MonoBehaviour {
	public AudioClip clip1, clip2, clip3, clip4, clip5, clip6, curClip;
	public float bgMusicVolume, loopDelay;
	public int curSpeedLevel;
	private Controller controlScript;
	private AudioSource source, cannonSource;
	private float delayTime, speedUpTime, pauseTimer, curMusicSpeed;
	private bool bgMusicShouldBePlaying, pausing, playingDucks, setupDelayTime, currentlyPlaying, intro;
	private const float clipTime1 = 31.425f;
	private const float clipTime2 = 28.186f;
	private const float clipTime3 = 25.547f;
	private const float clipTime4 = 23.353f;
	private const float clipTime5 = 21.524f;
	private const float clipTime6 = 19.957f;

	//Script to loop the background music during gameplay, controls pausing for various reasons
	void Start () {
		source = GetComponent<AudioSource> ();
		source.volume = bgMusicVolume;
		controlScript = GameObject.Find ("Controller").GetComponent<Controller> ();
		if (controlScript.fallingDude != null && controlScript.fallingDude.name == "Duck2") {
			delayTime = 18f;
			playingDucks = true;
			cannonSource = GameObject.Find ("Right Cannon Target").GetComponent<AudioSource> ();
		} else {
			delayTime = 3.75f;
		}

		bgMusicShouldBePlaying = true;
		pausing = false;
		curSpeedLevel = 0;
		curClip = clip1;
		intro = true;
	}
	
	// Update is called once per frame
	void Update () {
		if(controlScript == null){
			controlScript = GameObject.Find ("Controller").GetComponent<Controller> ();
		}

		//Change the delay time for the ducks
		if(!setupDelayTime){
			if(controlScript.fallingDude != null){
				if (controlScript.fallingDude.name == "Duck2") {
					delayTime = 18f;
					playingDucks = true;
					cannonSource = GameObject.Find ("Right Cannon Target").GetComponent<AudioSource> ();
				}
				setupDelayTime = true;
				StartCoroutine(loopAudioWithDelay2 ());
			}
		}

		//Handle different pauses during the game
		if(controlScript.canAudioPause || controlScript.bgmBonusPause || controlScript.bgmGreenPause){
			source.Pause ();
			pausing = true;
		}else{
			if(pausing && !intro){
				pausing = false;
				source.Play ();
				fadeAudioIn ();
			}
		}

		//If the game is over, stop the music
		if(controlScript.gameOver){
			fadeAudioOut ();
			source.Stop ();
		}

		//If player achieved a speed up, speed up the music
		if(controlScript.speedLevel != curSpeedLevel){
			curSpeedLevel = controlScript.speedLevel;
			speedUpAudio ();
		}

		//Increment the delay timer
		if(Time.timeScale != 0){
			delayTime -= Time.deltaTime;
		}
	}

	//Main audio looping method
	IEnumerator loopAudioWithDelay2(){
		while(delayTime >= 0){
			yield return null;
		}
		if (playingDucks) {
			while (cannonSource.time != 0) { //Wait for ducks to finish
				yield return null;
			}
		}
		intro = false;
		while (bgMusicShouldBePlaying) {
			source.clip = curClip;
			source.time = 0;
			source.Play ();
			currentlyPlaying = true;
			delayTime = curClip.length;
			while (delayTime >= 0) { //Wait for clip length
				yield return null;
			}
			while (source.time <= (curClip.length - .1f)) { //Wait until song finishes playing(time could change due to pauses in the game)
				yield return null;
			}
			currentlyPlaying = false;
			yield return new WaitForSeconds (3); //Loop again after 3 seconds
		}
	}

	//Fade in the background music
	void fadeAudioOut(){
		while(source.volume > 0){
			source.volume -= .00001f;
		}
		source.volume = 0;
	}

	//Fade out the background music
	void fadeAudioIn(){
		while(source.volume < bgMusicVolume){
			source.volume += .00001f;
		}
		source.volume = bgMusicVolume;
	}

	//Change the clip to a faster bpm music clip, start playing
	void speedUpAudio(){
		if(curSpeedLevel == 0){
			curClip = clip1;
		}else if(curSpeedLevel == 1){
			curClip = clip2;
		}else if(curSpeedLevel == 2){
			curClip = clip3;
		}else if(curSpeedLevel == 3){
			curClip = clip4;
		}else if(curSpeedLevel == 4){
			curClip = clip5;
		}else{
			curClip = clip6;
		}
		speedUpTime = calculatePlayTimeClips(source.time);
		source.clip = curClip;
		source.time = speedUpTime;
		if (currentlyPlaying) { //Make sure the sound was already playing when it sped up before starting to play again
			source.Play ();
		}
	}

	//Calculate the time the old clip stopped and the new clip should start.  Also calculate the amount of time to delay before looping
	float calculatePlayTimeClips(float curTime){
		if(curSpeedLevel == 0){
			return curTime; //130bps
		}else if(curSpeedLevel == 1){
			delayTime = clipTime2 - (((clipTime1 - delayTime) / clipTime1) * clipTime2);
			return curTime * (clipTime2 / clipTime1); //145bps
		}else if(curSpeedLevel == 2){
			delayTime = clipTime3 - (((clipTime2 - delayTime) / clipTime2) * clipTime3);
			return curTime * (clipTime3 / clipTime2); //160bps
		}else if(curSpeedLevel == 3){
			delayTime = clipTime4 - (((clipTime3 - delayTime) / clipTime3) * clipTime4);
			return curTime * (clipTime4 / clipTime3); //175bps
		}else if(curSpeedLevel == 4){
			delayTime = clipTime5 - (((clipTime4 - delayTime) / clipTime4) * clipTime5);
			return curTime * (clipTime5 / clipTime4); //190bps
		}else{
			delayTime = clipTime6 - (((clipTime5 - delayTime) / clipTime5) * clipTime6);
			return curTime * (clipTime5 / clipTime4); //205bps
		}
	}
}
