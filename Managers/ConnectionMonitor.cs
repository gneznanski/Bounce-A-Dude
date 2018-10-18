using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionMonitor : MonoBehaviour {
	public MenuControl mScript;
	private WWW www;
	private const string urlSafe = "https://www.google.com/";
	public bool connected, paused, conCheckDone;
	public float waitTimer, startTime, pauseTime;
	private const float timeToWait = 115f;//115

	// Use this for initialization
	void Start () {
		paused = false;
		pauseTime = 0f;
		startTime = Time.realtimeSinceStartup;
		checkConnection();
	}
	
	// Update is called once per frame
	void Update () {
		if (!paused) {
			waitTimer = Time.realtimeSinceStartup - startTime;
			if (waitTimer >= timeToWait) {
				checkConnection();
				startTime = Time.realtimeSinceStartup;
			}
		}
	}

	//Pause the connection timer while user is not playing the game, resume when they return
	private void OnApplicationPause(bool paused){
		if(paused){
			this.paused = true;
			pauseTime = Time.realtimeSinceStartup;
		}else{
			this.paused = false;
			startTime += (Time.realtimeSinceStartup - pauseTime);
		}
	}

	//Check connection helper method
	public void checkConnection(){
		StartCoroutine (checkConnectionCoroutine ());
	}
	//Check if the player is connected or not
	IEnumerator checkConnectionCoroutine(){
		conCheckDone = false;
		www = new WWW (urlSafe);
		yield return www;
		if(www.isDone && www.bytesDownloaded > 0){
			connected = true;
		}
		if(www.isDone && www.bytesDownloaded == 0){
			connected = false;
		}
		conCheckDone = true;
	}

	//Stop coroutine checking connection
	public void stopCheckCoroutine(){
		StopAllCoroutines ();
	}
}
