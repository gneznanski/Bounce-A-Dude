using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VortexController : MonoBehaviour {
	public GameObject[] freeConnectors, movingConnectors, duckList;
	public Transform topTarget;
	public GameObject sprite;
	private Controller controlScript;
	private Vector3[] path;
	private Vector3 startLocation = new Vector3 (4.1f, 3f, 1f);
	private Vector3 connectorReset = new Vector3(0f, 1.5f, 0f);
	private Vector3 fullSize = new Vector3 (2.6f, 2.6f, 1f);
	private Vector3 resetVector = new Vector3(0, 0, 0);
	public int ducksAttached, pathIndex;
	private bool initialized, needsReset;
	private float xVel, yVel;
	private const float tornadoSpeed = 2.25f;
	private const float upSpeed = 1.2f;

	//Vortex script for a test powerup involving a tornado, might use later?
	void Start () {
		controlScript = GameObject.Find ("Controller").GetComponent<Controller> ();
		sprite = GameObject.Find ("Tornado Sprite");
		for(int i = 0; i < freeConnectors.Length; i++){
			freeConnectors [i] = GameObject.Find ("Connector" + (i + 1).ToString ());
			freeConnectors [i].transform.localPosition = resetVector;
		}
		path = new Vector3[2];
		path[0] = new Vector3 (-3.3f, 3f, 1f);
		path[1] = new Vector3 (3.3f, 3f, 1f);
		//gameObject.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		if(controlScript.powerUpActive2){
			if (!initialized) {
				initialized = true;
				transform.position = startLocation;
				ducksAttached = 0;
				pathIndex = 0;
				needsReset = true;
				sprite.transform.localScale = new Vector3 (0f, 0f, 1f);
			}
			if(sprite.transform.localScale.x < 2.6f){
				sprite.transform.localScale += new Vector3 (.05f, .05f, 1f);
			}else{
				sprite.transform.localScale = fullSize;
			}
			if(transform.position != path[pathIndex]){
				transform.position = Vector3.MoveTowards (transform.position, path[pathIndex], Time.deltaTime * tornadoSpeed);
			}else{
				pathIndex++;
				if(pathIndex >= path.Length){
					pathIndex = 0;
				}
			}
			if(ducksAttached != 0){
				for(int i = 0; i < ducksAttached; i++){
					if (movingConnectors [i] != null) {
						if (movingConnectors [i].transform.localPosition.y != topTarget.localPosition.y) {
							movingConnectors [i].transform.localPosition = Vector3.MoveTowards (movingConnectors [i].transform.localPosition, topTarget.localPosition, Time.deltaTime * upSpeed);
						} else {
							duckList [i].GetComponent<SpringJoint2D> ().enabled = false;
							duckList [i].GetComponent<RubberDuckMovement> ().attachVortex = false;

							Rigidbody2D dudeRB = duckList [i].GetComponent<Rigidbody2D> ();
							xVel = dudeRB.velocity.x;
							yVel = dudeRB.velocity.y;
							if (dudeRB.velocity.x < -1) {
								xVel = -1;
							}else if(dudeRB.velocity.x > 1){
								xVel = 1;
							}
							if (dudeRB.velocity.y <= -1) {
								yVel = -1;
							}else if(dudeRB.velocity.y > 5){
								yVel = 5;
							}
							dudeRB.velocity = new Vector2 (xVel, yVel);

							duckList [i].layer = 8;
							movingConnectors [i] = null;
						}
					}
//					if (movingConnectors [i] != null) {
//						if (movingConnectors [i].transform.localPosition.y != topTarget.localPosition.y) {
//							movingConnectors [i].transform.position = new Vector3(movingConnectors [i].transform.position.x, duckList [i].transform.position.y, 0);
//						}else {
//							duckList [i].GetComponent<SpringJoint2D> ().enabled = false;
//							duckList [i].layer = 8;
//							movingConnectors [i] = null;
//						}
//					}
				}
			}
		}else{
			//reset
			if (needsReset) {
				needsReset = false;
				for (int i = 0; i < duckList.Length; i++) {
					if (duckList [i] != null) {
						duckList [i].GetComponent<RubberDuckMovement> ().alreadyVortex = false;
					}
				}
				initialized = false;
				pathIndex = 0;
				for (int i = 0; i < freeConnectors.Length; i++) {
					if (movingConnectors [i] != null) {
						movingConnectors [i] = null;
					}
					if (duckList [i] != null) {
						duckList [i].GetComponent<SpringJoint2D> ().enabled = false;
						duckList [i].GetComponent<RubberDuckMovement> ().attachVortex = false;
						duckList [i].GetComponent<RubberDuckMovement> ().alreadyVortex = false;
						duckList [i].layer = 8;
						duckList [i] = null;
					}
					freeConnectors [i].transform.localPosition = connectorReset;
				}
				//turn off
				//gameObject.SetActive (false);
			}

			//shrink   //////maybe have it come out of the cannon hole?
			if(sprite.transform.localScale.x > 0){
				sprite.transform.localScale -= new Vector3 (.05f, .05f, 1f);
			}else{
				if (gameObject.activeInHierarchy) {
					sprite.transform.localScale = fullSize;
					gameObject.SetActive (false);
				}
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D col){
		if (col.gameObject.tag == "Dude") {
			RubberDuckMovement duck = col.gameObject.GetComponent<RubberDuckMovement> ();
			if (!duck.attachVortex && !duck.alreadyVortex) { 
				if (ducksAttached < 11) {
					//attach
					duck.attachVortex = true;
					duck.alreadyVortex = true;
					col.gameObject.GetComponent<SpringJoint2D> ().enabled = true;
					duck.tornadoReset ();
					if (duck.hitGround) {
						controlScript.ducksDead--;
					}

					//set duck index
					duckList [ducksAttached] = col.gameObject;
					movingConnectors [ducksAttached] = freeConnectors [ducksAttached];
					movingConnectors [ducksAttached].transform.position = new Vector3(movingConnectors[ducksAttached].transform.position.x, duck.gameObject.transform.position.y, 0);
					col.gameObject.GetComponent<SpringJoint2D> ().connectedBody = movingConnectors [ducksAttached].GetComponent<Rigidbody2D> ();
					ducksAttached++;
				}
			}
		}
	}
}
