using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadController : MonoBehaviour {
	public Slider slider;
	public Text loadText, percentText;
	private AsyncOperation op;
	private bool foundText;
	public static bool animFinished;

	void Awake(){
		if (gameObject.name == "LoadController") {
			DontDestroyOnLoad (this);
		}
	}

	// Use this for initialization 
	void Start (){
		if (gameObject.name == "LoadController") {
			GameObject.Find ("Fade Image1").GetComponent<FadeController> ().turnOffFadeImage ();
			#if UNITY_ANDROID
				GooglePlayGames.PlayGamesPlatform.Activate (); //Activate GPGS
			#endif
			StartCoroutine (loadMenu ());
			SocialManager.sManager.logIn ();
		}
	}

	void Update(){
		if (!foundText) {
			if (percentText == null) {
				percentText = GameObject.Find ("Percent Text").GetComponent<Text> ();
				foundText = true;
			}
		}
		//Show percentage of load bar completed
		if(slider.value < .9f){
			percentText.text = ((slider.value / .9)*100).ToString ("N0") + "%";
		}else{
			percentText.text = "Done!";
		}
	}

	IEnumerator loadMenu(){
		op = SceneManager.LoadSceneAsync("Menu");
		op.allowSceneActivation = false;

		while(op.progress < .9f){ //Wait for next scene to load
			yield return null;
		}
		while (!animFinished) { //Wait for loading bar animation to finish
			yield return null;
		}
		slider.value = 1f;
		//loadText.text = "Done!";

		yield return new WaitForSecondsRealtime (.27f); //Short pause
		GameObject.Find("Fade Image1").GetComponent<FadeController>().fadeOutLoader ();
		yield return new WaitForSecondsRealtime (.2f); //Pause for fade out

		op.allowSceneActivation = true; //Load next scene
	}

	public void setAnimFinished(){
		animFinished = true;
	}
}
