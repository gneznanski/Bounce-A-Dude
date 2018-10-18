using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextMovement : MonoBehaviour {
	public bool moving, changeColor;
	private GameObject moveTarget;
	private Color textColor;
	private int speed = 35;
	private float changeSpeed;
	private int type;

	//Move the Floating Text toward a target, color part is not used, moving is? Can't remember at this point.
	void Start () {
		moveTarget = GameObject.Find ("Floating Text Center Target");
	}
	
	// Update is called once per frame
	void Update () {
		if(moveTarget == null){
			moveTarget = GameObject.Find ("Floating Text Center Target");
		}
		if(moving){
			transform.position = Vector2.MoveTowards (transform.position, Camera.main.WorldToScreenPoint(moveTarget.transform.position), speed * Time.deltaTime);
		}
		if(changeColor){
			if(type == 2){ //Big mode
				textColor = Color.Lerp (textColor, Color.green, Time.deltaTime / changeSpeed); //blue to green
			}else if(type == 1){ //Bomb
				textColor = Color.Lerp (textColor, Color.magenta, Time.deltaTime / changeSpeed); //red to purple
			}else{
				//might need later
			}

//			int rand = Random.Range (1, 4);
//			if (rand == 1) {
//				textColor = Color.Lerp (textColor, Color.white, Time.deltaTime / changeSpeed);
//			}else if(rand == 2){
//				textColor = Color.Lerp (textColor, Color.blue, Time.deltaTime / changeSpeed);
//			}else if(rand == 3){
//				textColor = Color.Lerp (textColor, Color.red, Time.deltaTime / changeSpeed);
//			}else{
//				textColor = Color.Lerp (textColor, Color.magenta, Time.deltaTime / changeSpeed);
//			}
			gameObject.GetComponentInChildren<Text>().color = textColor; 
		}
	}

	public void changeColors(Color color, float changeSpeed, int type){
		textColor = color;
		this.changeSpeed = changeSpeed;
		changeColor = true;
		this.type = type;
	}
}
