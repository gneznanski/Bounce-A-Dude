using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBalloonMovement : MonoBehaviour {
	public GameObject target;
	
	//Move the menu balloon across the screen to whichever target was picked
	void Update () {
		if(target != null){
			if(transform.position != target.transform.position){
				transform.position = Vector2.MoveTowards(transform.position, target.transform.position, 3 * Time.deltaTime);
			}
			if(transform.position.x >= 10){
				Destroy (gameObject);
			}else if(transform.position.y > 6){
				Destroy (gameObject);
			}else if(transform.position.y < -6){
				Destroy (gameObject);
			}
		}
	}
}
