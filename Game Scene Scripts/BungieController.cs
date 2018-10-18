using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BungieController : MonoBehaviour {
	public GameObject dude, seesaw, dudeInAir;
	public LineRenderer line;
	public bool leavingScreen, gameOverHold;
	private Controller controlScript;
	private AudioSource source;
	private SpriteRenderer rend, ringRend, knotRend;
	private PointEffector2D pEffector;
	private BoxCollider2D boxCol;
	private Animator anim;
	private AnimatorStateInfo info;
	private Transform ring;
	private Transform ringKnot;
	//private Vector3 target1 = new Vector3(-3.5f, 3.9f, 1f);
	private Vector3 target2, resetVector;
	private int birdType, curFrame, frame;
	private float distance, height, xPrev, xVel, yVel, ringXPos, ringYPos, rEdge;
	private bool attached, reachedSeesaw, reset, soundPlayed, velSet, alreadyOnScreen, pausingAudio;
	private Vector3 startEnd = new Vector3 (4.1f, 4.3f, 1f);
	private const float speed = 3f;

	// Use this for initialization
	void Start () {
		controlScript = GameObject.Find ("Controller").GetComponent<Controller> ();
		source = GetComponent<AudioSource> ();
		source.clip = controlScript.greenIntroSound;
		source.time = 0f;
		dude = controlScript.fallingDude;
		pEffector = GetComponent<PointEffector2D> ();
		boxCol = GetComponent<BoxCollider2D> ();
		rend = GetComponent<SpriteRenderer> ();
		anim = GetComponent<Animator> ();
		ring = GameObject.Find("Ring").GetComponent<Transform> ();
		ringRend = ring.gameObject.GetComponent<SpriteRenderer> ();
		ringKnot = GameObject.Find ("Ring Knot").GetComponent<Transform> ();
		knotRend = ringKnot.gameObject.GetComponent<SpriteRenderer> ();
		resetVector = new Vector3 (0, 0, 0);
		rEdge = 3.67f;
		line.enabled = false;
		transform.position = startEnd;
		xPrev = transform.position.x;
		height = 4.17f;
		//height = 3.9f;
		gameObject.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		if(transform.position.x > rEdge){ //Keep the bird hidden behind the border walls
			rend.sortingOrder = -8;
			ringRend.sortingOrder = -9;
			knotRend.sortingOrder = -9;
		}else{ //Bird is inside the walls and should be shown
			rend.sortingOrder = 15;
			ringRend.sortingOrder = 9;
			knotRend.sortingOrder = 10;
		}

		if (controlScript.powerUpActive) {
			if (!soundPlayed) { //Play the intro music
				if (!controlScript.bgmBonusPause) {
					controlScript.pauseBGMusic ("Green", source);
					source.time = 0.1f;
					fadeInMusic ();
					soundPlayed = true;
				}
				if (!alreadyOnScreen) { //Pick the bird that will fly out, dont change if bird is already on the screen
					chooseBirdAnim ();
					alreadyOnScreen = true;
				}
			}else{
				if(controlScript.bgmBonusPause){ //Pause music if Bonus song is playing
					if (!pausingAudio) {
						fadeOutMusic ();
						pausingAudio = true;
					}
				}else{
					if(pausingAudio){ //Unpause the intro song after Bonus song finishes
						if(!controlScript.gameOver){
							if(source.time <= (source.clip.length * .65f) && source.time >= .01f){ // 
								fadeInMusic ();
							}else{
								fadeOutMusic();
								controlScript.bgmGreenPause = false;
							}
							pausingAudio = false;
						}
					}
				}
			}

			setRingPostion (); //Angle the ring to match the angle of the tether line

			if (!controlScript.gameOver) {
				if (!attached) { //Bird is flying toward the dude
					transform.position = Vector3.MoveTowards (transform.position, dude.transform.position, Time.deltaTime * speed);
					distance = (transform.position - dude.transform.position).magnitude;

					if (distance <= 2f) {
						if (!dude.GetComponent<Bouncer> ().isAttached () && dude.transform.position.y >= 1.25f) {
							dude.GetComponent<SpringJoint2D> ().enabled = true;
							dude.GetComponent<SpringJoint2D> ().distance = this.distance;
							setDudeAnim ();
							attached = true;
							line.enabled = true;
							line.SetPosition (0, setAttachPoint ());
							line.SetPosition (1, new Vector2 (ring.position.x, ring.position.y - .033f));
							ringKnot.rotation = Quaternion.Euler (0, 0, Mathf.Rad2Deg * (Mathf.Atan2 (dude.transform.position.y - ringKnot.position.y, dude.transform.position.x - ringKnot.position.x)) - 90);
							pEffector.enabled = false;
							boxCol.enabled = false;
						}
					}
				} else { //Dude has attached to the bird
					setDudeAnim ();
					if(this.distance >= .75f){
						this.distance -= .04f;
					}
					dude.GetComponent<SpringJoint2D>().distance = this.distance;
					if (!reachedSeesaw) {
						target2 = new Vector3 (seesaw.transform.position.x, height, 1f);
						transform.position = Vector3.MoveTowards (transform.position, target2, Time.deltaTime * (speed+1));// /.8f
						if (transform.position == target2) {
							reachedSeesaw = true;
						}
					} else {
						transform.position = new Vector3 (seesaw.transform.position.x, height, 1f);
					}
					line.SetPosition (0, setAttachPoint());
					line.SetPosition (1, new Vector2(ring.position.x, ring.position.y - .033f));
					ringKnot.rotation = Quaternion.Euler (0, 0, Mathf.Rad2Deg *(Mathf.Atan2 (dude.transform.position.y - ringKnot.position.y, dude.transform.position.x - ringKnot.position.x)) - 90);
				}
			}else{
				controlScript.powerUpActive = false;
				gameOverHold = true; //Hold position if the game is over
			}
		}else{
			if (!velSet) { //Set the velocity after release from the tether
				velSet = true;
				dude.GetComponent<SpringJoint2D> ().enabled = false;

				if (!gameOverHold) {
					Rigidbody2D dudeRB = dude.GetComponent<Rigidbody2D> ();

					//Control the velocity so the dude doesnt go outside the screen or die immediately after releasing from anchor
					xVel = dudeRB.velocity.x;
					yVel = dudeRB.velocity.y;
					if (dudeRB.velocity.x < -1) {
						xVel = -1;
					} else if (dudeRB.velocity.x > 1) {
						xVel = 1;
					}
					if (dudeRB.velocity.y <= -1) {
						yVel = -1;
					} else if (dudeRB.velocity.y > 5) {
						yVel = 5;
					}
					dudeRB.velocity = new Vector2 (xVel, yVel);
				}

				attached = false;
				soundPlayed = false;
				line.SetPosition (0, resetVector);
				line.SetPosition (1, resetVector);
				line.enabled = false;

				leavingScreen = true;
			}

			//Handle the bird leaving the screen after the powerup expires
			if(leavingScreen){
				if (!gameOverHold) {
					transform.position = Vector3.MoveTowards (transform.position, startEnd, Time.deltaTime * speed);
				}else{
					transform.position = transform.position;
				}

				if(transform.position == startEnd){
					resetAnchor();
					alreadyOnScreen = false;
					anim.SetInteger ("State", 0);
					gameObject.SetActive (false);
				}
			}
		}

		//Flip the bird sprites depending on the direction it is flying
		if(transform.position.x < xPrev){
			rend.flipX = true;
		}else if(transform.position.x > xPrev){
			rend.flipX = false;
		}
		xPrev = transform.position.x;
	}

	//Reset the variables for the bird after powerup expires
	public void resetAnchor(){
		//if (!gameOverHold) {
			leavingScreen = false;
			reachedSeesaw = false;
			velSet = false;
			pEffector.enabled = true;
			boxCol.enabled = true;
		//}
	}

	//Pick the bird type that will fly out
	private void chooseBirdAnim(){
		birdType = Random.Range (1, 5);

		if(birdType == 1){
			anim.SetInteger ("State", 1);
		}else if(birdType == 2){
			anim.SetInteger ("State", 2);
		}else if(birdType == 3){
			anim.SetInteger ("State", 3);
		}else{
			anim.SetInteger ("State", 4);
		}
	}

	//Custom attach points for each dude
	private Vector3 setAttachPoint(){
		if(dude.name == "Skeleton1" || dude.name == "Skeleton2") {
			return new Vector3 (dude.transform.position.x, dude.transform.position.y);
		}else if (dude.name == "Shark1" || dude.name == "Shark2") { 
			return new Vector3 (dude.transform.position.x, dude.transform.position.y);
		}else if (dude.name == "Ninja1" || dude.name == "Ninja2") {
			return new Vector3 (dude.transform.position.x, dude.transform.position.y);
		}else{
			return resetVector;
		}
	}

	//Angle the ring to match the angle of the connecting tether
	public void setRingPostion(){
		info = anim.GetCurrentAnimatorStateInfo(0);
		frame = (int)((Mathf.Repeat(info.normalizedTime, 1.0f)) * 10);
		if(frame != curFrame){
			curFrame = frame;
			if(curFrame == 0){ //Each frame corresponds to an animation position
				ringXPos = -.015f;
				ringYPos = -.1f;
			}else if(curFrame == 1){
				ringXPos = -.015f;
				ringYPos = -.085f;
			}else if(curFrame == 2){
				ringXPos = -.005f;
				ringYPos = -.085f;
			}else if(curFrame == 3){
				ringXPos = .002f;
				ringYPos = -.085f;
			}else if(curFrame == 4){
				ringXPos = .002f;
				ringYPos = -.114f;
			}else if(curFrame == 5){
				ringXPos = -.01f;
				ringYPos = -.143f;
			}else if(curFrame == 6){
				ringXPos = -.02f;
				ringYPos = -.162f;
			}else if(curFrame == 7){
				ringXPos = -.025f;
				ringYPos = -.157f;
			}else if(curFrame == 8){
				ringXPos = -.025f;
				ringYPos = -.152f;
			}else{
				ringXPos = -.02f;
				ringYPos = -.14f;
			}
			ring.localPosition = new Vector2 (ringXPos, ringYPos);
		}
	}

	//Set the animation for each dude while tethered
	private void setDudeAnim(){
		if (dude.GetComponent<Bouncer> ().getAnim () != 1) {
			dude.GetComponent<Bouncer> ().setAnim (1);
		}
	}

	//Fade the music in over time
	void fadeInMusic(){
		source.volume = .00001f;
		//source.clip = controlScript.greenIntroSound;
		source.Play ();
		//soundPlayed = true;
		while(source.volume < .75f){
			source.volume += .00001f;
		}
		source.volume = .75f;
	}

	//Fade the music out over time
	void fadeOutMusic(){
		while(source.volume >= .75f){
			source.volume -= .00001f;
		}
		source.volume = 0f;
		source.Pause ();
	}
}
