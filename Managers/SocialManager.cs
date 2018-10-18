using UnityEngine.SocialPlatforms;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SocialManager : MonoBehaviour {
	public static SocialManager sManager;
	public string ANDROIDlbID1;
	public string IOSlbID1;
	public MenuControl mScript;
	public ConnectionMonitor cmScript;
	public bool loggingIn;
	private string leaderboardID;
	private bool gotData, shouldDisplayLB;

	void Awake(){
		sManager = this; //Singleton instance
	}
	void Start () {
		cmScript = GameObject.Find ("SceneManager").GetComponent<ConnectionMonitor> ();
		ANDROIDlbID1 = AndroidLBName;
		IOSlbID1 = IOSLBName;
	}

	//Get the top 10 scores from the leaderboard
	public void getTopScores(){
		//Test for user declines login --- arg2 is the error message
//		Social.localUser.Authenticate(
//			(bool arg1, string arg2) => {
//				if(arg1){
//					
//				}else{
//					if(arg2 != null){
//						Debug.Log(arg2);
//					}
//				}
//			})
	}

	//Log the user into GPGS or GameCenter
	public void logIn(){
		if (checkAuthentication()) {
			SmScript.gameManager.loggedIn = true;
		} else {
			loggingIn = true;
			#if UNITY_EDITOR
				//Social.localUser.Authenticate(new Action<bool> (loginCallbackAndroid));
				Social.localUser.Authenticate(new Action<bool> (loginCallbackIOS));
			#elif UNITY_ANDROID
				Social.localUser.Authenticate(new Action<bool> (loginCallbackAndroid));
			#elif UNITY_IPHONE
				Social.localUser.Authenticate(new Action<bool> (loginCallbackIOS));
			#endif
		}
	}

	//Display default GPGS leaderboard UI
	public void displayLeaderBoard(){
		if (Social.localUser.authenticated || loggingIn) {
			//Debug.Log("Showing Leaderboard UI");
			Social.ShowLeaderboardUI ();
		}else{
			#if UNITY_EDITOR //Used for testing
//				mScript.setLoginPanelCoverup(true); //Android
//				shouldDisplayLB = true;
//				mScript.setPlaySignInSound(true);
//				logIn();

				mScript.toggleLoginInfoPanel (true); //IOS
				mScript.playUISound (1);
			#elif UNITY_ANDROID
				mScript.setLoginPanelCoverup(true);
				shouldDisplayLB = true;
				mScript.setPlaySignInSound(true);
				logIn();
			#elif UNITY_IPHONE
				mScript.playUISound (1);
				mScript.toggleLoginInfoPanel (true);
				
//				mScript.setLoginPanelCoverup(true);
//				shouldDisplayLB = true;
//				mScript.setPlaySignInSound(true);
//				logIn();
			#endif
		}
	}
	
	//Report the users score to the Social leaderboard
	public void sendLBScore(long score){
		#if UNITY_ANDROID
			Social.ReportScore (score, AndroidLBName, 
				(bool success) => {
					if (success) {
						//Debug.Log ("Score reported.");
					}else{
						//Debug.Log("Score failed to report.");
					}
				});
		#elif UNITY_IPHONE
			Social.ReportScore (score, IOSLBName,
				(bool success) => {
					if (success) {
						//Debug.Log ("Score reported.");
					}else{
						//Debug.Log("Score failed to report.");
					}
				});
		#endif
	}

	//Check whether the user is logged in to a social platform or not
	public bool checkAuthentication(){
		if(Social.localUser.authenticated){
			return true;
		}else{
			SmScript.gameManager.loggedIn = false;
			return false;
		}
	}
	
	public void getHighestScore(){
		#if UNITY_ANDROID
			//leaderboardID = ANDROIDlbID1;
			leaderboardID = AndroidLBName;
		#elif UNITY_IPHONE
			//leaderboardID = IOSlbID1;
			leaderboardID = IOSLBName;
		#endif

		ILeaderboard lb = Social.CreateLeaderboard();
		lb.id = leaderboardID;
		lb.SetUserFilter(new string[]{Social.localUser.id});
		lb.LoadScores((bool success) => {
			if(success){
				if(lb.scores.Length > 0){
					Debug.Log("Got user's score: " + lb.scores.GetValue(0).ToString());
				}else{
					Debug.Log("lb.scores.Length is 0.");
				}
			}else{
				Debug.Log("Did not get the user's score.");
			}
		});
	}
	
	//Callback method for Android after logging in, called whether successful or not
	public void loginCallbackAndroid(bool result){
		loggingIn = false;
		if(result){
			checkSignInSound (result);
			SmScript.gameManager.loggedIn = true;
			//mScript.setGPGSLoginCheck (true);
			GooglePlayGames.PlayGamesPlatform.Instance.SetDefaultLeaderboardForUI (AndroidLBName);  //Set default leaderboard
			if(shouldDisplayLB){
				shouldDisplayLB = false;
				Invoke ("displayLeaderBoard", .8f);
				Invoke ("turnOffPanelCoverup", .79f);
			}
		} else {
			SmScript.gameManager.loggedIn = false;
			//mScript.setGPGSLoginCheck (false);
			checkSignInSound (result);
			turnOffPanelCoverup ();
		}
//		if (mScript != null && mScript.getLoginCoverup ().activeInHierarchy) {
//			mScript.setLoginPanelCoverup (false);
//		}
	}
	
//	//Callback method for IOS after logging in, called whether successful or not
//	public void loginCallbackIOS(bool result){
//		loggingIn = false;
//		if(result){
//			checkSignInSound (result);
//			SmScript.gameManager.loggedIn = true;
//			if(shouldDisplayLB){
//				shouldDisplayLB = false;
//				Invoke ("displayLeaderBoard", .8f);
//				Invoke ("turnOffPanelCoverup", .79f);
//			}
//		} else {
//			SmScript.gameManager.loggedIn = false;
//			turnOffPanelCoverup ();
//			if (mScript != null) {
//				mScript.playUISound (1);
//				mScript.toggleLoginInfoPanel (true);
//			}
//		}
//	}

	//Callback method for IOS after logging in, called whether successful or not - ORIGINAL VERSION
	public void loginCallbackIOS(bool result){
		loggingIn = false;
		if(result){
			SmScript.gameManager.loggedIn = true;
		} else {
			SmScript.gameManager.loggedIn = false;
		}
		checkSignInSound (result);
	}
	
	//Check if sign in sound should play or not
	private void checkSignInSound(bool result){
		if (mScript != null && mScript.getPlaySignInSound ()) {
			mScript.setPlaySignInSound (false);
			if (result) {
				mScript.playUISound (8);
			} else {
				mScript.playUISound (6);
			}
		}
	}
	
	//Turn off panel coverup (after delay if logging in before display LB, this is to prevent the user from hitting other buttons while login and display are loading)
	private void turnOffPanelCoverup(){
		if (mScript != null && mScript.getLoginCoverup ().activeInHierarchy) {
			mScript.setLoginPanelCoverup (false);
		}
	}
}
