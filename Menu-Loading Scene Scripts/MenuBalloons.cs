using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBalloons : MonoBehaviour {
	public GameObject balloon1, balloon2, balloon3, balloon4, balloon5, balloon6, target1, target2, target3, target4;
	private float timePassed;

	// Use this for initialization
	void Start () {
		//numBalloons = 30;
	}
	
	// Update is called once per frame
	void Update () {
		timePassed += Time.deltaTime;
		if(timePassed >= .1){
			float startingY = Random.Range(-6f, 6f);
			GameObject newBalloon = Instantiate(getPrefab(), new Vector2(-10, startingY), Quaternion.identity);
			newBalloon.GetComponent<MenuBalloonMovement> ().target = getTarget();
			timePassed = 0;
		}
	}

	GameObject getTarget(){
		int targetNum = Random.Range (1, 5);
		if(targetNum == 1){
			return target1;
		}else if(targetNum == 2){
			return target2;
		}else if(targetNum == 3){
			return target3;
		}else{
			return target4;
		}
	}

	GameObject getPrefab(){
		int prefabNum = Random.Range (1, 7);
		if(prefabNum == 1){
			return balloon1;
		}else if(prefabNum == 2){
			return balloon2;
		}else if(prefabNum == 3){
			return balloon3;
		}else if (prefabNum == 4){
			return balloon4;
		}else if (prefabNum == 5){
			return balloon5;
		}else{
			return balloon6;
		}
	}
}
