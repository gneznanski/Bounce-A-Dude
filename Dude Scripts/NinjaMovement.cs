﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Ninja movement control/interaction script
public class NinjaMovement : Bouncer {
	public AudioClip thudSound1, thudSound2, thudSound3, thudSound4, thudSound5, thudSound6;
	public bool bounce1, bounce2, movingUp, switchCollider, attachedL, attachedR;
	public float bounceVal1, bounceVal2, yPrev, xPrev;
	private GameObject dude, teeter, fText;
	private Rigidbody2D rigidB;
	private BoxCollider2D dudeTeeterCol;
	private CapsuleCollider2D dudeBalloonCol;
	private Controller controlScript;
	private Animator anim;
	private AudioSource source;
	private Vector2 zeroVector = new Vector2(0,0);
	private float topEdge, lEdge, rEdge, bEdge, lenStandXL, lenStandYL, lenStandXR, lenStandYR, localPosX, localPosY, bigModeScale;
	private int flipDir, landDir, deadDir;
	private bool bigMode, growing, shrinking, collided, greenPowerUpBool;

	//Initiallize conditions if dude is riding the seesaw
	public override void setupAttached(string name, Transform teeter){
		gameObject.name = name;
		gameObject.transform.parent = teeter;
		if(dudeTeeterCol == null){
			dudeTeeterCol = gameObject.GetComponent<BoxCollider2D>();
		}
		dudeTeeterCol.enabled = false;
		attachedL = true;
		anim.SetInteger("State", 0); //Start idle animation
	}

	public override void setupFalling(string name){
		gameObject.name = name;
		anim.SetInteger ("State", 0);
		switchCollider = true; //Start idle animation
	}

	public override void Awake()
	{
		anim = gameObject.GetComponent<Animator>(); //Get reference to the animator component, in Awake so update can begin animation
	}

	// Use this for initialization
	public override void Start () {
		controlScript = GameObject.Find("Controller").GetComponent<Controller>();
		dude = gameObject;
		rigidB = GetComponent<Rigidbody2D>();
		dudeTeeterCol = gameObject.GetComponent<BoxCollider2D>();
		dudeBalloonCol = gameObject.GetComponent<CapsuleCollider2D>();
		teeter = GameObject.Find("Teeter(Player)");
		source = GetComponent<AudioSource>();
		GetComponent<SpringJoint2D>().connectedBody = controlScript.springAnchor.GetComponent<Rigidbody2D>();
		GetComponent<SpringJoint2D> ().enabled = false;
		controlScript.springAnchor.SetActive (false);
		dudeBalloonCol.enabled = false;
		collided = false;
		bigMode = false;
		greenPowerUpBool = false;
		growing = false;
		shrinking = false;
		bounce1 = false;
		bounce2 = false;
		flipDir = 1;
		landDir = 2;
		xPrev = dude.transform.position.x; //Used to determine the direction of dude movement in the X direction
		yPrev = dude.transform.position.y; //Used to determine the direction of dude movement in the Y direction
		topEdge = 5.04f;
		lEdge = -3.78f;
		rEdge = 3.78f;
		bEdge = 1.04f; 
		lenStandXL = .691f;
		lenStandYL = .954f;
		lenStandXR = .691f;
		lenStandYR = .954f;
	}

	// Update is called once per frame 
	public override void Update () {
		if(controlScript == null){
			controlScript = GameObject.Find("Controller").GetComponent<Controller>();
		}
		if(bigMode){
			if(growing){
				if (transform.localScale.x < 1.2f) {
					transform.localScale += new Vector3 (.05f, .05f, 1);
				}else{
					transform.localScale = new Vector3 (1.2f, 1.2f, 1);
					growing = false;
				}
			}else if(shrinking){
				if (transform.localScale.x > .7f) {
					transform.localScale -= new Vector3 (.05f, .05f, 1);
				}else{
					transform.localScale = new Vector3 (.7f, .7f, 1);
					shrinking = false;
					bigMode = false;
				}
			}else{
				transform.localScale = new Vector3 (1.2f, 1.2f, 1);
			}
		}else{
			transform.localScale = new Vector3 (.7f, .7f, 1);
		}

		if (attachedL) //Dude is on left side of teeter
		{
			int rotation;
			if (bigMode) { //Dude is bigger, adjust placement on the seesaw
				localPosX = -.369f;
				localPosY = .323f;
				rotation = 11;
			} else {
				localPosX = -.403f;
				localPosY = .176f;
				rotation = 9;
			}
			dude.transform.localPosition = new Vector2(localPosX, localPosY);
			rigidB.velocity = zeroVector;
			dude.transform.rotation = Quaternion.Euler (0, 0, rotation); //Rotate dude to correctly stand on the seesaw
			anim.SetInteger("State", 0); //Idle animation
		}
		else if (attachedR) //Dude is on right side of teeter
		{
			if (bigMode) { //Dude is bigger, adjust placement on the seesaw
				localPosX = .266f;
				localPosY = .34f;
			} else {
				localPosX = .349f;
				localPosY = .182f;
			}
			dude.transform.localPosition = new Vector2(localPosX, localPosY);
			rigidB.velocity = zeroVector;
			dude.transform.rotation = Quaternion.Euler (0, 0, -7); //Rotate dude to correctly stand on the seesaw
			anim.SetInteger("State", 0); //Idle animation
		}
		else //Dude is falling
		{
			dude.transform.rotation = Quaternion.Euler (0, 0, 0); //Reset dudes rotation to zero

			//Determine if dude is moving up or down on the screen
			if(dude.transform.position.y >= yPrev){
				movingUp = true;
			}else{
				movingUp = false;
			}
			yPrev = dude.transform.position.y;

			//Determine if dude is moving left or right
			if(dude.transform.position.x < xPrev){
				flipDir = 1;
				landDir = 2;
				deadDir = 6;
			}else if(dude.transform.position.x > xPrev){
				flipDir = 4;
				landDir = 5;
				deadDir = 3;
			}
			xPrev = dude.transform.position.x;

			//Turn on colliders, or turn off capsule below platforms
			if (dude.transform.position.y > 1.6f) {
				dudeTeeterCol.enabled = true;
				dudeBalloonCol.enabled = true;
			}else{
				dudeBalloonCol.enabled = false;
			}
			if (dude.transform.position.y < 2.1f) { //Start landing animation
				if(!isMovingUp())
				{
					if (!controlScript.gameOver) {
						//anim.SetInteger ("State", 2); //Landing animation
						anim.SetInteger ("State", landDir);
					}
				}else{
					if (!controlScript.gameOver) {
						anim.SetInteger ("State", flipDir); //Left or right falling animation
					}
				}
			}else{
				if(!controlScript.checkIntro()){
					anim.SetInteger ("State", flipDir); //Left or right falling animation
				}
			}
		}
		updateVector(); //Determine bounce value
		checkBounds(); //Make sure the dude is still inside bounds of scene
	}

	//If dude hits the ground he dies
	public override void OnCollisionEnter2D(Collision2D col){
		if (col.gameObject.tag == "Ground")
		{
			anim.SetInteger ("State", deadDir); //Dead animation
			controlScript.gameOver = true; //Signal that the dude has died and game is over
			Vector2 tempVector = new Vector2 (0, 0);
			rigidB.velocity = tempVector; //Stop movement of dude on the ground
			if(gameObject.transform.parent == null){
				GetComponent<SpriteRenderer> ().sortingOrder = 11; //Draw dead dudes in front of the seesaw and rider for clarity near ground
			}
			if (!collided) {
				collided = true;
				if (!controlScript.checkDeathTextDisplay ()) {
					controlScript.updateDeathTextDisplayed ();
					fText = PoolingManager.pooler.getPooledObject (7);
					fText.SetActive (true);
					fText.GetComponent<FloatingText> ().createDeathText (transform.position, 1);
					Invoke ("resetText", 1.4f);
					//ftScript.createDeathText (transform.position, 1);
				}
			}
		}
		if(col.gameObject.tag == "Explosion" || col.gameObject.tag == "PowerUp"){
			Physics2D.IgnoreCollision (GetComponent<BoxCollider2D> (), col.collider);
			Physics2D.IgnoreCollision (GetComponent<CapsuleCollider2D> (), col.collider);
		}
		if(col.gameObject.tag == "Boundary Collider"){
			source.PlayOneShot (getThudSound(), .4f); //Play thud collision sound
			if(col.gameObject.name == "Left Inner Edge Collider"){
				if(col.contacts[0].point.x > dude.transform.position.x){
					dude.transform.position = new Vector2 (col.contacts [0].point.x + .1f, dude.transform.position.y);
				}
			}else if(col.gameObject.name == "Right Inner Edge Collider"){
				if(col.contacts[0].point.x < dude.transform.position.x){
					dude.transform.position = new Vector2 (col.contacts [0].point.x - .1f, dude.transform.position.y);
				}
			}
		}
	}

	//Apply the bounce force to the dude when bouncing off teeter
	public override void updateVector()
	{
		if (bounce1) //High bounce
		{
			dude.transform.position = new Vector2(dude.transform.position.x, 1.2f);
			//rigidB.velocity = new Vector2(rigidB.velocity.x + teeter.GetComponent<TeeterBase>().velocity/2, 0);
			rigidB.velocity = new Vector2(teeter.GetComponent<TeeterBase>().velocity/2, 0);
			rigidB.AddForce(dude.transform.up * bounceVal1, ForceMode2D.Impulse);
			bounce1 = false;
		}
		else if (bounce2) //Low bounce
		{
			dude.transform.position = new Vector2(dude.transform.position.x, 1.2f);
			//rigidB.velocity = new Vector2(rigidB.velocity.x + teeter.GetComponent<TeeterBase>().velocity/2, 0);
			rigidB.velocity = new Vector2(teeter.GetComponent<TeeterBase>().velocity/2, 0);
			rigidB.AddForce(dude.transform.up * bounceVal2, ForceMode2D.Impulse);
			bounce2 = false;
		}
	}

	//Make sure dude is within the game bounds
	public override void checkBounds()
	{
		if(dude.transform.position.y <= bEdge)
		{
			dude.transform.position = new Vector2(dude.transform.position.x, bEdge);
		}
		else if (dude.transform.position.y > topEdge)
		{
			dude.transform.position = new Vector2(dude.transform.position.x, topEdge);
		}
		else if(dude.transform.position.x > rEdge)
		{
			dude.transform.position = new Vector2(rEdge, dude.transform.position.y);
		}
		else if(dude.transform.position.x < lEdge)
		{
			dude.transform.position = new Vector2(lEdge, dude.transform.position.y);
		}
	}

	//Override Bouncer methods with character specific actions
	public override void setTeeterCol(bool active){ //Toggle box collider
		dudeTeeterCol.enabled = active;
	}
	public override void setBalloonCol(bool active){ //Toggle capsule collider
		dudeBalloonCol.enabled = active;
	}
	public override void setAttached(char side, bool active){ //Set variable for appropriate side for attached rider
		if(side == 'l'){
			attachedL = active;
		}else{
			attachedR = active;
		}
	}
	public override void setParent(Transform parent){ //Set dude parent, parent needed for correct positioning on seesaw
		dude.transform.parent = parent;
	}
	public override void setPositionOnTeeter(float sawPositionX, char side){ //Position the dude on the seesaw
		if (dude != null) {
			if (side == 'l') {
				dude.transform.position = new Vector2 (sawPositionX - lenStandXL, lenStandYL);
			} else {
				dude.transform.position = new Vector2 (sawPositionX + lenStandXR, lenStandYR);
			}
//			if(gameObject.GetComponent<SpriteRenderer>().flipX){
//				gameObject.GetComponent<SpriteRenderer> ().flipX = false;
//			}
		}
	}
	public override void setBounce(int bounceNum){ //Set high or low bounce
		if(bounceNum == 1){
			bounce1 = true;
		}else{
			bounce2 = true;
		}
	}
	public override bool isMovingUp(){ //Return whether dude is moving up or not
		return movingUp;
	}
	public void pauseAfterDeadAnimationNinja(){ //Callback after dead animation has completed, needed so dead animation is shown before pausing the game
		controlScript.animationDone = true;
	}
	public override bool isAttached(){ //Return whether dude is attached to the seesaw or falling
		if(attachedL){
			return true;
		}else if (attachedR){
			return true;
		}else {
			return false;
		}
	}
	public override void setAnim(int value){ //Set which animation should be playing
		anim.SetInteger ("State", value);
	}
	public override int getAnim(){
		return anim.GetInteger ("State");
	}
	public override void switchPlatformCollider(bool active){ //Update whether platform collider has been switched
		switchCollider = active;
	}
	public override bool isPlatformColliderSwitched(){ //Return value of switchCollider 
		return switchCollider;
	}

	public override void toggleBigMode(bool toggle){
		if(toggle){
			bigMode = true;
			growing = true;
		}else{
			shrinking = true;
		}
	}

	public override bool inBigMode(){
		return bigMode;
	}

	public override void setGreenPowerUp(bool toggle){
		greenPowerUpBool = toggle;
	}

	public override bool getGreenPowerUp(){
		return greenPowerUpBool;
	}

	private void resetText(){
		PoolingManager.pooler.resetFtext (fText);
	}

	private AudioClip getThudSound(){
		int rand = Random.Range (1, 7);
		if(rand == 1){
			return thudSound1;
		}else if(rand == 2){
			return thudSound2;
		}else if(rand == 3){
			return thudSound3;
		}else if(rand == 4){
			return thudSound4;
		}else if(rand == 5){
			return thudSound5;
		}else{
			return thudSound6;
		}
	}
}
