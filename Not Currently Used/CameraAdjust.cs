using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraAdjust : MonoBehaviour {

	//Script to adjust camera to fit screen, not used - found a better way to do this
	void Start(){
		float targetWidth = 640f;
		float targetHeight = 480f;
		int pixelsToUnits = 100;

		float targetRatio = targetWidth / targetHeight;
		float currentRatio = (float)Screen.width/(float)Screen.height;

		if (currentRatio >= targetRatio) {
			Camera.main.orthographicSize = targetHeight / 2 / pixelsToUnits;
		} else {
			float dif = targetRatio / currentRatio;
			Camera.main.orthographicSize = targetHeight / 2 / pixelsToUnits * dif;
		}

		//GetComponent<Camera>().orthographicSize = 3;
	}
}
