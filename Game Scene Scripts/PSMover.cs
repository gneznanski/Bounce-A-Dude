using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Not used, this was for moving the PS around the new game border, might use later
public class PSMover : MonoBehaviour {
	public GameObject[] waypoints;
	private int curPoint;

	//This script was for a potential PSeffect during the gameover display, not used
	void Start () {
		gameObject.transform.position = waypoints [0].transform.position;
		curPoint = 1;
	}
	
	// Update is called once per frame
	void Update () {
		if(gameObject.transform.position != waypoints[curPoint].transform.position){
			gameObject.transform.position = Vector3.MoveTowards (gameObject.transform.position, waypoints [curPoint].transform.position, .4f);
		}else{
			curPoint++;
			if(curPoint == waypoints.Length){
				curPoint = 0;
			}
		}
	}
}
