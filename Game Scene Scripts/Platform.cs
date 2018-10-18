using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour {
	private GameObject dude1, dude2;
	private Controller controlScript;
    private float forceX, forceY;
	private bool sharkCollision;

	void Start () {
		controlScript = GameObject.Find ("Controller").GetComponent<Controller> ();
		if(controlScript != null){
			dude1 = controlScript.attachedDude;
			dude2 = controlScript.fallingDude;
		}

        forceX = 1.2f; // Bounce forces
        forceY = 1.2f;
	}

	void Update () {
		if (controlScript == null) {
			controlScript = GameObject.Find ("Controller").GetComponent<Controller> ();
		} else {
			if (dude1 == null || dude2 == null) {
				dude1 = controlScript.attachedDude;
				dude2 = controlScript.fallingDude;
			}
		}
		checkPosition (); //Take action depending on position of falling dude
	}

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.transform.tag == "Dude")
        {
			if (col.transform.name == "Shark1" || col.transform.name == "Shark2") {
				sharkCollision = true;
			}
			if (col.transform.position.y >= transform.position.y || sharkCollision) {
				Rigidbody2D rigidB = col.gameObject.GetComponent<Rigidbody2D> ();
				rigidB.velocity = new Vector2 (rigidB.velocity.x / 2, 0);

				//Position the dude outside of the platform
				float newPosition;
				if (col.transform.name == "Skeleton1" || col.transform.name == "Skeleton2") {
					if (col.gameObject.GetComponent<Bouncer> ().inBigMode ()) {
						newPosition = .57f;
					} else {
						newPosition = .35f;
					}
				} else if (col.transform.name == "Shark1" || col.transform.name == "Shark2") { 
					if (col.gameObject.GetComponent<Bouncer> ().inBigMode ()) {
						newPosition = .203f;
					} else {
						newPosition = .1f;
					}
					sharkCollision = false;
				} else if (col.transform.name == "Ninja1" || col.transform.name == "Ninja2") {
					if (col.gameObject.GetComponent<Bouncer> ().inBigMode ()) {
						newPosition = .35f;
					} else {
						newPosition = .22f;
					}
				} else {
					newPosition = 0;  //Put offset for new characters here 
				}
				col.transform.position = new Vector2 (col.transform.position.x, gameObject.transform.position.y + newPosition);

				//Apply the force on the dude, attempt to bounce in an appropriate direction
				if (rigidB.velocity.x > Vector2.zero.x) {
					rigidB.AddForce (new Vector2 (forceX, forceY), ForceMode2D.Impulse);
				} else if (rigidB.velocity.x < Vector2.zero.x) {
					rigidB.AddForce (new Vector2 (-forceX, forceY), ForceMode2D.Impulse);
				} else {
					if (col.transform.position.x <= 0) {
						rigidB.AddForce (new Vector2 (forceX, forceY), ForceMode2D.Impulse);
					} else {
						rigidB.AddForce (new Vector2 (-forceX, forceY), ForceMode2D.Impulse);
					}
				}
			}
        }
		if(col.transform.tag == "PowerUp"){
			Physics2D.IgnoreCollision (GetComponent<BoxCollider2D> (), col.collider);
		}
    }

    private void enableCollider()
    {
        gameObject.GetComponent<EdgeCollider2D>().enabled = true;
    }

	private void disableCollider(){
		gameObject.GetComponent<EdgeCollider2D>().enabled = false;
	}

	//Method to determine whether dude is bouncing up or starting to land, toggle colliders accordingly
	private void checkPosition(){
		if (!dude1.GetComponent<Bouncer> ().isAttached ()) {
			if (dude1.transform.position.y >= 2.8f) { //Dude has passed collider height, turn on to ensure a hit when coming down
				dude1.GetComponent<Bouncer> ().switchPlatformCollider (true);
			}
			if (!dude1.GetComponent<Bouncer> ().isPlatformColliderSwitched ()) {
				disableCollider ();
			} else {
				enableCollider ();
			}
		} else {
			if (dude2.transform.position.y >= 2.8f) { //If dude has passed collider height
				dude2.GetComponent<Bouncer> ().switchPlatformCollider (true);
			}
			if (!dude2.GetComponent<Bouncer> ().isPlatformColliderSwitched ()) { //Turn on/off to either collide with dude or let pass through, depending on direction of movement
				disableCollider ();
			} else {
				enableCollider ();
			}
		}
	}
}
