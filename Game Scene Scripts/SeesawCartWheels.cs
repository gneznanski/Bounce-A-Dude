using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeesawCartWheels : MonoBehaviour {
	public Sprite cart1, cart2, cart3, cart4, fillRed, fillBlue, fillGreen, fillNone;
	public GameObject imagePanel;
	public bool ok2Roll;
	private Sprite[] cartArray;
	private GameObject controlEmpty, fillL, fillM, fillR;
	private TeeterBase teeterScript;
	private GameObject controller;
	private int index, count;
	private float prevValue;

	//Roll the cart wheels in the correct direction of movement and speed that the user inputs
	void Start () {
		controlEmpty = GameObject.Find ("Control Empty 1");
		teeterScript = GameObject.Find ("Teeter(Player)").GetComponent<TeeterBase> ();
		controller = GameObject.Find ("Controller");
		fillL = GameObject.Find ("TopFillL");
		fillM = GameObject.Find ("TopFillM");
		fillR = GameObject.Find ("TopFillR");
		cartArray = new Sprite[4];
		index = 0;
		count = 1;

		cartArray[0] = cart1;
		cartArray[1] = cart2;
		cartArray[2] = cart3;
		cartArray[3] = cart4;
	}
	
	// Update is called once per frame
	void Update () {
		if (teeterScript != null) {
			teeterScript.baseImageHeight = imagePanel.transform.position.y;
			teeterScript.checkScrollBounds();
		}else{
			teeterScript = GameObject.Find ("Teeter(Player)").GetComponent<TeeterBase> ();
		}

		//Setting the fill or no fill holes in the cart top
		if(controller.GetComponent<Controller>().fallingDude.GetComponent<Bouncer>().inBigMode()){
			fillPowerUpHolders (2, true);
		}else{
			fillPowerUpHolders (2, false);
		}
		if(controller.GetComponent<BalloonMovement>().bombIsSet()){
			fillPowerUpHolders (1, true);
		}else{
			fillPowerUpHolders (1, false);
		}
		if(controller.GetComponent<Controller>().fallingDude.GetComponent<Bouncer>().getGreenPowerUp()){
			fillPowerUpHolders (3, true);
		}else{
			fillPowerUpHolders (3, false);
		}
	}

	//Change image depending on which way cart is moving, looks like wheels are moving
	public void rollWheels(float value){
		if (ok2Roll) {
			if (Mathf.Abs (value - prevValue) >= .002f) { //Ignore tiny value changes
				//Determine how fast the wheels should move depending on the change in value from previous value
				if (Mathf.Abs (value - prevValue) > .008f) { //Fast speed wheels
					if ((count % 1) == 0) {
						if (value > prevValue) { //Moving left
							index--;
							if (index < 0) {
								index = 3;
							}
						} else { //Moving right
							index++;
							if (index > 3) {
								index = 0;
							}
						}
						gameObject.GetComponent<Image> ().sprite = cartArray [index];
						prevValue = value;
					}
				} else if (Mathf.Abs (value - prevValue) >= .005f) { //Medium speed wheels
					if ((count % 2) == 0) {
						if (value > prevValue) { //Moving left
							index--;
							if (index < 0) {
								index = 3;
							}
						} else { //Moving right
							index++;
							if (index > 3) {
								index = 0;
							}
						}
						gameObject.GetComponent<Image> ().sprite = cartArray [index];
						prevValue = value;
					}
				} else {  //Slow speed wheels
					if ((count % 3) == 0) {
						if (value > prevValue) { //Moving left
							index--;
							if (index < 0) {
								index = 3;
							}
						} else { //Moving right
							index++;
							if (index > 3) {
								index = 0;
							}
						}
						gameObject.GetComponent<Image> ().sprite = cartArray [index];
						prevValue = value;
					}
				}
				count++;
			}
		}
	}

	//Display or hide the powerup indicators on the cart
	private void fillPowerUpHolders(int type, bool toggle){
		if(type == 1){
			if (toggle) {
				fillL.GetComponent<Image>().sprite = fillRed;
			}else{
				fillL.GetComponent<Image>().sprite = fillNone;
			}
		}else if(type == 2){
			if (toggle) {
				fillM.GetComponent<Image>().sprite = fillBlue;
			}else{
				fillM.GetComponent<Image>().sprite = fillNone;
			}
		}else if(type == 3){
			if (toggle) {
				fillR.GetComponent<Image>().sprite = fillGreen;
			}else{
				fillR.GetComponent<Image>().sprite = fillNone;
			}
		}else{ //Might need later
		}
	}
}
