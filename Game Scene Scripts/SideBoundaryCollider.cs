using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SideBoundaryCollider : MonoBehaviour {
	private Text fpsText;
	private int lowest, counter, fps;

	//Repurposed script to implement an fps tracker during performance testing and optimization
	void Start () {
		fpsText = GetComponent<Text> ();
		lowest = int.MaxValue;
		counter = 0;
	}
	
	// Update is called once per frame
	void Update () {
		fps = (int)(1f / Time.unscaledDeltaTime);
		if(fps < lowest){
			lowest = fps;
		}
		//fpsText.text = fps.ToString () + " / " + lowest.ToString();
		counter++;
		if(counter >= 40){
			fpsText.text = lowest.ToString();
			counter = 0;
			lowest = int.MaxValue;
		}
	}
}
