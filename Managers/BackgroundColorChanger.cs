using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundColorChanger : MonoBehaviour {
	public static BackgroundColorChanger bgcc;
	public Color curBorderColor, curWallColor, curGroundColor;
	public Color[] borderColors, wallColors, groundColors;
	public bool waitingForController;
	public int select;
	private Controller controlScript;
	private SpriteRenderer[] renders;
	private const int numRenders = 11;
	private int prevBorderNum, prevWallNum, prevGroundNum, curIndex;

	//Script to change all of the colors in the game scene
	void Awake(){
		bgcc = this;
	}

	// Use this for initialization
	void Start () {
		controlScript = gameObject.GetComponent<Controller> ();
		prevBorderNum = 4;
		prevWallNum = 4;
		prevGroundNum = 4;
		curIndex = 0;

		//Get references to the renderers
		renders = new SpriteRenderer[numRenders];
		renders [0] = GameObject.Find ("Back Wall").GetComponent<SpriteRenderer> (); //Inner wall
		renders [1] = GameObject.Find ("Inside Ground Above Control").GetComponent<SpriteRenderer> ();
		renders [2] = GameObject.Find ("Background Inside (2)").GetComponent<SpriteRenderer> (); //Inner Border(was green)
		renders [3] = GameObject.Find ("Left Gap Cover").GetComponent<SpriteRenderer> ();
		renders [4] = GameObject.Find ("Right Gap Cover").GetComponent<SpriteRenderer> ();
	}

	void Update(){
		if(waitingForController){ //Wait until the controller determines the prefab, then set the appropriate renderers
			if (controlScript.attachedDudeName.Equals("Duck1")) { //Set the color to render for the platform and covers
				renders [5] = GameObject.Find ("Top Left Duck Platform Cover(White)").GetComponent<SpriteRenderer> ();
				renders [6] = GameObject.Find ("Top Right Duck Platform Cover(White)").GetComponent<SpriteRenderer> ();
				renders [7] = GameObject.Find ("Bottom Left Duck Platform Cover(White)").GetComponent<SpriteRenderer> ();
				renders [8] = GameObject.Find ("Bottom Right Duck Platform Cover(White)").GetComponent<SpriteRenderer> ();
				renders [9] = GameObject.Find ("Left Cannon Cover(White)").GetComponent<SpriteRenderer> ();
				renders [10] = GameObject.Find ("Right Cannon Cover(White)").GetComponent<SpriteRenderer> ();
			}else{
				renders [5] = GameObject.Find ("Top Left Platform Cover(White)").GetComponent<SpriteRenderer> ();
				renders [6] = GameObject.Find ("Top Right Platform Cover(White)").GetComponent<SpriteRenderer> ();
				renders [7] = GameObject.Find ("Bottom Left Platform Cover(White)").GetComponent<SpriteRenderer> ();
				renders [8] = GameObject.Find ("Bottom Right Platform Cover(White)").GetComponent<SpriteRenderer> ();
			}

			//Setup initial colors
			renders [0].color = curWallColor;
			renders [1].color = curGroundColor;
			for(int i = 2; i < numRenders; i++){
				if (renders [i] != null) {
					renders [i].color = curBorderColor;
				}
			}
			waitingForController = false;
		}
	}

	//Pick a random color change
	public void changeBackgroundColorRandom(){
		pickColor ();
		renders [0].color = curWallColor;
		renders [1].color = curGroundColor;
		for(int i = 2; i < numRenders; i++){
			if (renders [i] != null) {
				renders [i].color = curBorderColor;
			}
		}
	}

	//Change colors in order
	public void changeBackgroundColor(){
		if (curIndex < 10) {
			curBorderColor = borderColors [curIndex];
			curGroundColor = groundColors [curIndex];
			curWallColor = wallColors [curIndex];

			renders [0].color = curWallColor;
			renders [1].color = curGroundColor;
			for (int i = 2; i < numRenders; i++) {
				if (renders [i] != null) {
					renders [i].color = curBorderColor;
				}
			}
			curIndex++;
		}else{
			changeBackgroundColorRandom ();
		}
	}

	//Pick random colors to change the scene after speed score reached
	private void pickColor(){
		int rand;

		do{
			rand = Random.Range (0, 15);
		}while(rand == prevBorderNum);
		prevBorderNum = rand;
		curBorderColor = borderColors[rand];

		do{
			rand = Random.Range (0, 15);
		}while(rand == prevWallNum);
		prevWallNum = rand;
		curWallColor = wallColors [rand];

		do{
			rand = Random.Range (0, 15);
		}while(rand == prevGroundNum);
		prevGroundNum = rand;
		curGroundColor = groundColors[rand];
	}

	//Set the colors
	public void changeColor(){
		if (select < borderColors.Length) {
			curBorderColor = borderColors [select];
			curGroundColor = groundColors [select];
			curWallColor = wallColors [select];

			renders [0].color = curWallColor;
			renders [1].color = curGroundColor;
			for (int i = 2; i < numRenders; i++) {
				if (renders [i] != null) {
					renders [i].color = curBorderColor;
				}
			}
		}else{
			changeBackgroundColorRandom ();
		}
	}
}
