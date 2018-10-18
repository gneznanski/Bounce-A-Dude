using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBalloonAnimate : MonoBehaviour {
	private bool adding;
	private int rotate, count;

	//Give the balloons some rotation to make them more interesting in the menu
	void Start () {
		//random adding
		if(Random.Range(0, 2) == 0){
			adding = false;
		}else{
			adding = true;
		}
		//random rotate
		rotate = Random.Range(-10, 11);
		gameObject.transform.rotation = Quaternion.Euler (0, 0, rotate);
		count = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (count % 3 == 0) {
			gameObject.transform.rotation = Quaternion.Euler (0, 0, rotate);
			if (adding) {
				rotate++;
				if (rotate >= 10) {
					adding = false;
				}
			} else {
				rotate--;
				if (rotate <= -10) {
					adding = true;
				}
			}
		}
		count++;
	}
}
