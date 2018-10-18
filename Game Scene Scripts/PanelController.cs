using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelController : MonoBehaviour {
	public Sprite bones, bruce, ninja, ducks;
	public void togglePanel(){
		gameObject.SetActive (false);
	}
}
