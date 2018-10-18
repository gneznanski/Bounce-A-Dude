using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleLoaderBG : MonoBehaviour {
	Vector2 screenVector;

	//Make the bg image slightly bigger than the screen, show a little less of the image edges. Might need?
	int wOffset = 0; //50
	int hOffset = 0; //15

	void Awake(){
		gameObject.GetComponent<RectTransform> ().sizeDelta = new Vector2(Screen.width + wOffset, Screen.height + hOffset);
	}

	//Resize the loader background image to fit the size of the device screen
	void Update(){
		screenVector = new Vector2(Screen.width+wOffset, Screen.height+hOffset);
		if(gameObject.GetComponent<RectTransform> ().sizeDelta != screenVector){
			gameObject.GetComponent<RectTransform> ().sizeDelta = screenVector;
		}
	}
}
