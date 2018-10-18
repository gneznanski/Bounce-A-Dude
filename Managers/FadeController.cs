using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeController : MonoBehaviour {
	private Animator anim;
	private Vector2 showFade = new Vector2 (0f, 0f);
	private Vector2 hideFade = new Vector2 (-5000f, -5000f);
	
	void Start () {
		anim = GetComponent<Animator> ();
	}

	//Fade in the menu scene
	public void fadeInMenu(){
		turnOnFadeImage ();
		anim.SetInteger ("Fading", 2);
		Invoke ("turnOffFadeImage", .6f);
	}

	//Fade out the menu scene
	public void fadeOutMenu(){
		turnOnFadeImage ();
		anim.SetInteger ("Fading", 1);
	}

	//Fade in the game scene
	public void fadeInGame(){
		turnOnFadeImage ();
		anim.SetInteger ("Fading", 2);
		Invoke ("turnOffFadeImage", .6f);
	}

	//Fade out the menu scene
	public void fadeOutGame(){
		turnOnFadeImage ();
		anim.SetInteger ("Fading", 1);
	}

	//Fade in the loading scene
	public void fadeOutLoader(){
		turnOnFadeImage ();
		anim.SetInteger ("Fading", 1);
	}

	//Move the black fade image out of camera view
	public void turnOffFadeImage(){
		transform.localPosition = hideFade;
	}

	//Move the black fade image into camera view
	public void turnOnFadeImage(){
		transform.localPosition = showFade;
	}
}
