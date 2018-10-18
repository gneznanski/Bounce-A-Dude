using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeCanvas : MonoBehaviour {
	void Awake(){
		DontDestroyOnLoad (this);
	}
}
