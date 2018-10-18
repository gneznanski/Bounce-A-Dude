using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonAudioController : MonoBehaviour {
	public Controller controlScript;
	private AudioSource source;
	public AudioClip cannonIntroSound;
	public bool unMute, unPause, userPause;
	private float pauseTimer;

	void Start () {
		controlScript = GameObject.Find("Controller").GetComponent<Controller>();
		source = GetComponent<AudioSource> ();
		source.volume = .9f;
		unMute = false;
		unPause = false;
		userPause = false;
	}

	void Update () {
		if(Time.timeScale == 0){
			source.Pause ();
			if(!controlScript.canAudioPause){
				unMute = true;
				unPause = true;
				pauseTimer = 0;
			}else{
				userPause = true;
			}
		}else if(Time.timeScale != 0){
			if (!controlScript.duckSequenceDone && unMute) {  //Handle waiting for row clear song to end
				if (unPause) {
					source.UnPause ();
					source.mute = true;
					unPause = false;
				}
				pauseTimer += Time.deltaTime;
				if (pauseTimer >= 1.79f) {
					source.mute = false;
					unMute = false;
				}
			}

			if(!controlScript.duckSequenceDone && userPause){ //Restart the music if there are still ducks to shoot
				source.UnPause ();
				userPause = false;
			}
		}
		if(SmScript.gameManager.showingAd){ //Stop music playing before an ad
			if(source.isPlaying){
				source.Stop ();
			}
		}
	}

	public AudioSource getSource(){
		return source;
	}
}
