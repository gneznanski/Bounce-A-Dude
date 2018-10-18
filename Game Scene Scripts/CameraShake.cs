using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {
	private Animator anim;

	//Shake the camera during a bomb explosion
	void Start () {
		anim = GetComponent<Animator> ();
	}

	public void shakeScreen(){
		anim.SetBool ("Shake", true);
	}

	public void stopShake(){
		anim.SetBool ("Shake", false);
	}
}
