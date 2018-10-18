using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GoogleMobileAds.Api;
using System;
using System.Text.RegularExpressions;

public class SmScript : MonoBehaviour{
	public static SmScript gameManager;
	public ConnectionMonitor cmScript;
	public MenuControl mScript;
	public GameObject fadeImage;
	public int reset;
	public bool showingAd, loggedIn, adInitiallized, ready, gpgsLoginChecked, gpgsLoginDeclined;
	private InterstitialAd interstitial;
	private Animator anim;
	private Coroutine lafCoroutine;
	private GameObject activeBigModePowerUp, activeExplodeL, activeExplodeR, activeSpringPowerUp;
	private AsyncOperation adOp;
	private FadeController fadeScript;
	private bool timerStarted, retryAd, noFill;
	private int buttonType, failCounter;
	private float adTimer, startTime, pauseTime;
	private string adId;
	private const float adTimeToWait = 120f; //120
	private const int menuNum = 1;
	private const int gameNum = 2;

	private void Awake ()
	{
		//Only 1 gameManager needs to persist through scene switches
		if (gameManager == null) {
			gameManager = this;
			DontDestroyOnLoad (this);
		} else if (this != gameManager) {
			Destroy (this.gameObject);
		}
	}

	// Use this for initialization
	void Start ()
	{
		fadeScript = GameObject.Find ("Fade Image1").GetComponent<FadeController> ();
		cmScript = GetComponent<ConnectionMonitor> ();
		mScript = GameObject.Find ("Main Camera").GetComponent<MenuControl> ();
		timerStarted = false;
		adTimer = 0;

		if (SceneManager.GetActiveScene ().buildIndex == gameNum) {
			reset = 0;
		}

		#if UNITY_EDITOR
			adId = "unused";
		#elif UNITY_ANDROID
			//adId = "ca-app-pub-3940256099942544~3347511713"; //Test AdID
			adId = alkj24234; //Real AdID
			
			GooglePlayGames.PlayGamesPlatform.Activate (); //Activate GPGS
		#elif UNITY_IPHONE
			//adId = "ca-app-pub-3940256099942544~1458002511"; //Test AdID
			adId = alkj2423; //Real AdID
		#else
			adId = "unexpected_platform";
		#endif

		//Initiallize the ad service if online
		if(cmScript.connected){
			initiallizeAd ();
		}else{
			adInitiallized = false;
		}

		GameObject.Find ("LoadController").transform.SetParent(this.gameObject.transform);
		GameObject.Find ("LoadController").SetActive (false);
		ready = true;
	}

	void Update(){
		if(showingAd){
			if (SceneManager.GetActiveScene ().buildIndex == gameNum) {
				Time.timeScale = 0; //Pause the game while showing the ad
			}
		}
		if (timerStarted) {
			adTimer = Time.realtimeSinceStartup - startTime; //Keep time for how often to show an ad
		}
	}

	//Pause the ad timer while user is not playing the game, resume when they return
	void OnApplicationPause(bool paused){
		if(paused){
			pauseTime = Time.realtimeSinceStartup;
		}else{
			startTime += (Time.realtimeSinceStartup - pauseTime);
		}
	}

	//2 methods for coroutine - 1 second delay before restarting level
	public void waitToLoad ()
	{
		StartCoroutine (waitDeath ());
	}
	IEnumerator waitDeath ()
	{
		yield return new WaitForSecondsRealtime (1.0f);
		loadAfterDeath2 ();
	}

	//Start a new game
	public void startNewGame2(){
		buttonType = 3;
		GameObject.Find("Controller").GetComponent<Controller>().playNewGameSound();
		loadAfterDeath2 ();
	}


	//Reload level
	public void loadAfterDeath2 ()
	{
		PlayerPrefs.Save ();
		if (SceneManager.GetActiveScene ().buildIndex == gameNum) {
			fadeScript.fadeOutGame ();

			lafCoroutine = StartCoroutine (reloadAfterFadeAsync (gameNum));
		}
	}
	//Callback for when Play Button is clicked
	public void playButtonOnClick2(){
		PlayerPrefs.Save (); //Save the game information before loading a new scene
		buttonType = 1;
		fadeScript.fadeOutMenu ();

		if(reset == 3){ //If character was changed, reset the game(Reset will be 3) 
			removeKeys ();
			reset = 0;
		}
		StartCoroutine (loadAfterFadeAsync (gameNum)); //Start the coroutine to load the game scene
	}

	//Callback for when Menu Button is clicked
	public void menuButtonOnClick2(){
		PlayerPrefs.Save (); //Save the game information before loading a new scene
		buttonType = 2;
		if (lafCoroutine != null) {
			StopCoroutine (lafCoroutine); //If menu button is clicked during a game reload, stop the reload
		}
		fadeScript.fadeOutGame ();
		StartCoroutine (loadAfterFadeAsync (menuNum));  //Start the coroutine to load menu scene
	}

	//Initiallize Admob, request and listen for callback if cant load ad
	public void initiallizeAd(){
		MobileAds.Initialize (adId);
		adInitiallized = true;
		requestInterstitial ();
	}

	public void checkAdStatus(){
		if(!adInitiallized && cmScript.connected){
			requestInterstitial ();
		}
	}

	private void showInterstitial2(){
		if (interstitial.IsLoaded ()) {
			interstitial.Show ();
		}
	}
	
	//Check if ad should be shown
	private void checkAd2(){
		if (adInitiallized) {
			if (cmScript.connected) {
				if (adTimer == 0) {
					timerStarted = true;
					startTime = Time.realtimeSinceStartup;
					showingAd = false;
				} else if (adTimer >= adTimeToWait || retryAd) {
					showingAd = true;
					retryAd = false;
				} else {
					showingAd = false;
				}	
			} else {
				showingAd = false;
			}
		}else{
			showingAd = false;
		}
	}
	
	//Get the add from the ad service
	private void requestInterstitial()
	{
		#if UNITY_EDITOR
			string adUnitId = "unused";
		#elif UNITY_ANDROID
			//string adUnitId = "ca-app-pub-3940256099942544/1033173712"; //Test interstial ID
			string adUnitId = alkj2423; //Real interstial ID
		#elif UNITY_IPHONE
			//string adUnitId = "ca-app-pub-3940256099942544/1033173712"; //Test interstial ID
			string adUnitId = alkj2423; //Real interstial ID
		#endif

		if(interstitial != null){
			interstitial.Destroy ();
		}

		interstitial = new InterstitialAd (adUnitId);
		if (interstitial != null) {
			interstitial.OnAdLeavingApplication += this.HandleInterstitialLeavingApplication;
			interstitial.OnAdFailedToLoad += this.HandleInterstitialFailedToLoad;
			interstitial.OnAdOpening += this.HandleInterstitialOpening;
			interstitial.OnAdClosed += this.HandleInterstitialClosed;
			interstitial.OnAdLoaded += this.HandleInterstitialLoaded;

			interstitial.LoadAd (new AdRequest.Builder ().Build ());
		}
	}
	
	//Interstitial callback for how to handle leaving the app
	public void HandleInterstitialLeavingApplication (object sender, EventArgs args){
		if (interstitial != null) {
			interstitial.Destroy ();
		}
	}	

	//Interstitial callback for how to handle the ad failing to load.  ---- Unused error codes ("Internal error", "Invalid request", Default-("Unexpected error code"))
	public void HandleInterstitialFailedToLoad (object sender, AdFailedToLoadEventArgs args){
		//GameObject.Find ("Debug Text").GetComponent<Text> ().text = "Load Ad Failed " + failCounter + ": |" + args.Message + "|";
		if (cmScript.connected) { //Connected
			if (Regex.Match (args.Message, "(Request Error)").Success) { //Check for IOS restrictions
				adInitiallized = false;
				return;
			} else if (Regex.Match (args.Message, "(No fill)").Success) { //If no fill from ad service, delay the next request
				if(!noFill){
					noFill = true;
					//setRetryAd ();
				}
				Invoke ("requestInterstitial", 10);
				failCounter++;
			} else {
				requestInterstitial ();
				setRetryAd (); //Reload ad and set variables to try to show at next opportunity
			}
		}else{
			loadNextScene ();
			if (SceneManager.GetActiveScene ().buildIndex == menuNum) {
				reset = 0;
			}
		}
	}
	
	//Interstitial callback for how to handle the ad being loaded
	public void HandleInterstitialLoaded(object sender, EventArgs args){
		noFill = false; //Ad loaded, reset noFill for future ad loading
	}
	//Interstitial callback for how to handle the ad opening
	public void HandleInterstitialOpening (object sender, EventArgs args){
	}
	//Interstitial callback for how to handle closing the ad
	public void HandleInterstitialClosed (object sender, EventArgs args){
		adTimer = 0;
		requestInterstitial ();
		setRetryAd ();
		retryAd = false;
	}

	//Set variables to retry showing the ad when it is available
	private void setRetryAd(){
		//requestInterstitial ();
		retryAd = true;
		startTime = Time.realtimeSinceStartup;
		showingAd = false;
		loadNextScene ();
		if (SceneManager.GetActiveScene ().buildIndex == menuNum) {
			reset = 0;
		}
	}

	//Asynchronous loading of next scene, check adTimer to show ad or load next scene
	IEnumerator loadAfterFadeAsync(int scene){
		adOp = SceneManager.LoadSceneAsync(scene);
		adOp.allowSceneActivation = false;

		while(adOp.progress < .9f){
			yield return null;
		}
		
		yield return new WaitForSecondsRealtime (.3f);//.49
		preLoadingCleanUp ();
		if (SceneManager.GetActiveScene ().buildIndex == gameNum) {
			GameObject.Find ("Controller").GetComponent<Controller> ().stopExplodeCoroutines ();
		}
		checkAd2 ();
		if (showingAd && !noFill) {
			showInterstitial2 ();
		} else {
			loadNextScene ();
		}
	}

	//Asynchronous reloading of scene, check adTimer to show ad or load next scene
	IEnumerator reloadAfterFadeAsync(int scene){
		adOp = SceneManager.LoadSceneAsync(scene);
		adOp.allowSceneActivation = false;

		while(adOp.progress < .9f){
			yield return null;
		}
		fadeScript.fadeOutGame ();

		yield return new WaitForSecondsRealtime (.3f);//.49
		preLoadingCleanUp ();
		GameObject.Find ("Controller").GetComponent<Controller> ().stopExplodeCoroutines ();
		if (buttonType == 3) {
			checkAd2 ();
			if (showingAd && !noFill) {
				showInterstitial2 ();
			} else {
				loadNextScene ();
			}
			buttonType = 0;
		} else {
			loadNextScene ();
		}
		reset = 0;

	}

	//Reset variables and coroutines before loading new scene
	private void preLoadingCleanUp(){
		if(activeBigModePowerUp != null){
			activeBigModePowerUp.GetComponent<PowerUp> ().stopBigModeCoroutine ();
			activeBigModePowerUp = null;
		}
		if(activeSpringPowerUp != null){
			activeSpringPowerUp.GetComponent<PowerUp> ().stopGreenCoroutine ();
			activeSpringPowerUp = null;
		}
		PoolingManager.pooler.resetControllers ();
	}
	//Load the next scene and prepare some variables
	private void loadNextScene(){
		if (adOp != null) {
			adOp.allowSceneActivation = true;
			PoolingManager.pooler.checkForActivePooledObjects ();
			Time.timeScale = 1;
		}
	}

	//Remove game values from playerprefs, clean for next game
	public void removeKeys ()
	{
		PlayerPrefs.DeleteKey ("currentGame");
		PlayerPrefs.DeleteKey ("currentBalloons");
		PlayerPrefs.DeleteKey ("lives");
		PlayerPrefs.DeleteKey ("score");
		for (int i = 0; i < 10; i++) {
			PlayerPrefs.DeleteKey ("Row1Balloon" + i.ToString ());
			PlayerPrefs.DeleteKey ("Row2Balloon" + i.ToString ());
			PlayerPrefs.DeleteKey ("Row3Balloon" + i.ToString ());
		}
		PlayerPrefs.DeleteKey ("row1Pop");
		PlayerPrefs.DeleteKey ("row1Prefab");
		PlayerPrefs.DeleteKey ("row2Pop");
		PlayerPrefs.DeleteKey ("row2Prefab");
		PlayerPrefs.DeleteKey ("row3Pop");
		PlayerPrefs.DeleteKey ("row3Prefab");
	}

	public void setActivePowerUp(GameObject powerUp){
		activeBigModePowerUp = powerUp;
	}
	
	public void setActiveSpringPowerUp(GameObject springPowerUp){
		activeSpringPowerUp = springPowerUp;
	}

	public bool getShowingAd(){
		return showingAd;
	}

	public void resetShowingAd(){
		showingAd = false;
	}
}
