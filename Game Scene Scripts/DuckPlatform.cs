using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuckPlatform : MonoBehaviour {
	private float forceX, forceY;
	
	void Start () {
		forceX = 1.2f; //Bounce forces
		forceY = 1.2f;
	}

	//Trigger zone for 1-way platform performance even with multiple ducks at once
	private void OnTriggerEnter2D(Collider2D col){
		if (col.gameObject.tag == "Dude")
		{
			if(col.gameObject.transform.position.y - .05f > transform.position.y){
				col.gameObject.GetComponent<RubberDuckMovement> ().playSound ();
				Rigidbody2D rigidB = col.attachedRigidbody;
				rigidB.velocity = new Vector2(rigidB.velocity.x/2, 0);
				float newPosition = .1f; //Position the duck outside the trigger zone
				if(col.gameObject.GetComponent<Bouncer>().inBigMode()){
					newPosition = .2f;
				}else{
					newPosition = .1f;
				}
				col.gameObject.transform.position = new Vector2 (col.gameObject.transform.position.x, gameObject.transform.position.y + newPosition);

				//Add bounce force to the duck, attempt to bounce in an appropriate direciton
				if (rigidB.velocity.x > Vector2.zero.x)
				{
					rigidB.AddForce(new Vector2(forceX, forceY), ForceMode2D.Impulse);
				}else if(rigidB.velocity.x < Vector2.zero.x)
				{
					rigidB.AddForce(new Vector2(-forceX, forceY), ForceMode2D.Impulse);
				}else{
					if(col.transform.position.x <= 0){
						rigidB.AddForce(new Vector2(forceX, forceY), ForceMode2D.Impulse);
					}else{
						rigidB.AddForce(new Vector2(-forceX, forceY), ForceMode2D.Impulse);
					}
				}
			}
		}
		if(col.gameObject.tag == "PowerUp"){
			Physics2D.IgnoreCollision (GetComponent<BoxCollider2D> (), col.GetComponent<Collider2D>());
		}
	}
}
