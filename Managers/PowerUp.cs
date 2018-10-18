using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour {
	public AudioClip powerUpSound, powerHitGroundSound, bombSound, whooshSound;
	public ParticleSystem missedEffectRed, missedEffectBlue, missedEffectGreen, caughtEffectRed, caughtEffectBlue, caughtEffectGreen, redEffect, blueEffect, greenEffect;
	public int index, powerType;
	private Controller controlScript;
	private GameObject target, fText;
	private ParticleSystem effectPS;
	private Animator anim;
	private FloatingText ftScript;
	private TeeterBase teeterScript;
	private int greenType;
	private bool caught, collided, haveTarget, processed;
	private float speed = 1f;

	void Awake(){
		anim = gameObject.GetComponent<Animator> ();
	}

	// Use this for initialization
	void Start () {
		controlScript = GameObject.Find ("Controller").GetComponent<Controller> ();
		ftScript = gameObject.GetComponent<FloatingText> ();
	}
	
	// Update is called once per frame
	void Update () {
		if(ftScript == null){
			ftScript = gameObject.GetComponent<FloatingText> ();
		}
		if(controlScript == null){
			controlScript = GameObject.Find ("Controller").GetComponent<Controller> ();
		}
		if(target != null){
			transform.position = Vector3.MoveTowards (transform.position, target.transform.position, Time.deltaTime * speed);
		}
		if(teeterScript == null){
			teeterScript = GameObject.Find ("Teeter(Player)").GetComponent<TeeterBase> ();
		}

		if(collided && !processed){
			processed = true;
			removePowerUp ();
		}
	}

	public void OnCollisionEnter2D(Collision2D col){
		if (!collided) {
			collided = true;

			if (col.gameObject.tag == "Teeter") {
				caught = true;
				//collided = true;
				//removePowerUp ();
			}
			if (col.gameObject.tag == "Ground") {
				//collided = true;
				//removePowerUp ();
			}
		}
	}

	public void initialize2(int type){
		powerType = type;

		target = gameObject.transform.GetChild (0).gameObject;
		target.name = gameObject.name + " Fall Target";
		target.SetActive (true);

		//Set animation and particle effect
		if (type == 1) { //Bomb
			anim.SetInteger ("State", 1);
			effectPS = Instantiate (redEffect, gameObject.transform.position, Quaternion.identity);
		} else if (type == 2) { //Big Mode
			anim.SetInteger ("State", 2);
			effectPS = Instantiate (blueEffect, gameObject.transform.position, Quaternion.identity);
		} else if (type == 3) {
			anim.SetInteger ("State", 3);
			effectPS = Instantiate (greenEffect, gameObject.transform.position, Quaternion.identity);
		}else{ //Might need later
		}

		effectPS.name = gameObject.name + " PS";
		effectPS.transform.SetParent (gameObject.transform);
		effectPS.gameObject.SetActive (true);
	}

	//Remove powerUp from view, apply if player caught it, otherwise do nothing
	public void removePowerUp(){
		if(caught){ //Apply the powerup
			if (powerType == 2) { //Big Mode
				effectPS.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
				GetComponent<SpriteRenderer> ().enabled = false;
				GetComponent<CapsuleCollider2D> ().enabled = false;
				createCaughtEffect (1, 250);

				if (!controlScript.fallingDude.GetComponent<Bouncer> ().inBigMode () && !controlScript.attachedDude.GetComponent<Bouncer> ().inBigMode ()) {
					controlScript.toggleBigMode (true);
					fText = PoolingManager.pooler.getPooledObject (7);
					fText.SetActive (true);
					fText.GetComponent<FloatingText> ().createBigModeText (GameObject.Find ("Teeter(Player)").transform.position);
					controlScript.delayedResetFText(fText);
					if (gameObject.activeInHierarchy) { //This is here because of repeat collisions, try to fix those instead of this condition
						StartCoroutine (endBigMode ());
					}
					GameObject.Find("SceneManager").GetComponent<SmScript>().setActivePowerUp (this.gameObject);
					teeterScript.playPowerUpSound (powerUpSound, .35f);
				} else {
					disablePowerUp ();
					teeterScript.playPowerUpSound (powerUpSound);
					givePointsInsteadOfPowerUp (250);
				}
			} else if (powerType == 1) { //Bomb
				effectPS.Stop();
				disablePowerUp ();
				createCaughtEffect (2, 250);

				if (!controlScript.gameObject.GetComponent<BalloonMovement> ().bombIsSet ()) {
					controlScript.gameObject.GetComponent<BalloonMovement> ().setBombTemp ();
					fText = PoolingManager.pooler.getPooledObject (7);
					fText.SetActive (true);
					fText.GetComponent<FloatingText> ().createBombSetText (GameObject.Find ("Teeter(Player)").transform.position);
					controlScript.delayedResetFText(fText);
					teeterScript.playPowerUpSound (powerUpSound);
					//teeterScript.playPowerUpSound (bombSound, .6f);
				} else {
					givePointsInsteadOfPowerUp (250);
					teeterScript.playPowerUpSound (powerUpSound);
				}
			} else if (powerType == 3) { //Bird - Duck recycle
				effectPS.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
				GetComponent<SpriteRenderer> ().enabled = false;
				GetComponent<CapsuleCollider2D> ().enabled = false;
				createCaughtEffect (3, 250);

				if (!controlScript.fallingDude.GetComponent<Bouncer>().getGreenPowerUp()) {
					fText = PoolingManager.pooler.getPooledObject (7);
					fText.SetActive (true);
					controlScript.fallingDude.GetComponent<Bouncer> ().setGreenPowerUp (true);
					if (checkDudeType ()) { //Bird Green powerup
						greenType = 1;
						fText.GetComponent<FloatingText> ().createSpringText (GameObject.Find ("Teeter(Player)").transform.position);

						if (controlScript.springAnchor.activeInHierarchy) {
							if (controlScript.springAnchor.GetComponent<BungieController> ().leavingScreen) {
								controlScript.springAnchor.GetComponent<BungieController> ().resetAnchor ();
							}
						}
						controlScript.springAnchor.SetActive (true);
						controlScript.powerUpActive = true;
						if (gameObject.activeInHierarchy) { //This is here because of repeat collisions, try to fix those instead of this condition
							StartCoroutine (endSpring ());
						}
						teeterScript.playPowerUpSound (powerUpSound, .35f);
					}else{ //Ducks Green powerup
						greenType = 2;
						fText.GetComponent<FloatingText> ().createVortexText (GameObject.Find ("Teeter(Player)").transform.position);
						controlScript.powerUpActive2 = true;
						teeterScript.playPowerUpSound (powerUpSound, .35f);
						teeterScript.playPowerUpSound (whooshSound);

						int rand = Random.Range (4, 7);
						for (int i = 0; i < rand; i++) {
							if (controlScript.deadDucks.Count != 0) {
								if (controlScript.deadDucks.Peek () != null) { //Pop a previously dead duck from the queue and launch into the air
									GameObject duck = (GameObject)controlScript.deadDucks.Dequeue ();
									duck.GetComponent<Rigidbody2D> ().AddForce (duck.transform.up * (Random.Range (.7f, 1.01f) * 5.15f), ForceMode2D.Impulse);
									duck.GetComponent<SpriteRenderer> ().sortingOrder = 5;
									RubberDuckMovement duckScript = duck.GetComponent<RubberDuckMovement> ();
									duckScript.hitGround = false;
									duckScript.delayLayerChange ();
									controlScript.ducksDead--;
									if(controlScript.ducksDead < 0){
										controlScript.ducksDead = 0;
									}
//									Debug.Log ("-1 dead duck: " + controlScript.ducksDead.ToString () + " dead ducks.");
//									Debug.Log (controlScript.deadDucks.Count.ToString() + " ducks in the queue");
								}
							}
						}

						controlScript.powerUpActive2 = false;
						controlScript.fallingDude.GetComponent<Bouncer> ().setGreenPowerUp (false);
					}
					controlScript.delayedResetFText (fText);
					GameObject.Find ("SceneManager").GetComponent<SmScript> ().setActiveSpringPowerUp (this.gameObject);
				} else {
					disablePowerUp ();
					teeterScript.playPowerUpSound (powerUpSound);
					givePointsInsteadOfPowerUp (250);
				}
			}else{ //Might need later
			}
		}else{
			GameObject.Find ("Teeter(Player)").GetComponent<TeeterBase> ().playPowerUpSound (powerHitGroundSound);
			disablePowerUp ();
			controlScript.displayPowerMissEffect (transform.position, powerType);
		}
	}

	//Decide which type of powerUp the object is going to be
	private void determinePowerUp(){
		powerType = Random.Range(1, 4);

		//Set animation and particle effect
		if (powerType == 1) { //Bomb
			anim.SetInteger ("State", 1);
			effectPS = Instantiate (redEffect, gameObject.transform.position, Quaternion.identity);
		} else if (powerType == 2) { //Big Mode
			anim.SetInteger ("State", 2);
			effectPS = Instantiate (blueEffect, gameObject.transform.position, Quaternion.identity);
		} else if (powerType == 3) {
			anim.SetInteger ("State", 3);
			effectPS = Instantiate (greenEffect, gameObject.transform.position, Quaternion.identity);
		}else{ //Might need later
		}
		effectPS.transform.SetParent (gameObject.transform);
	}

	//Prepare the powerup to show and fall on the screen
	public void prep(Vector3 location){
		gameObject.transform.position = location;
		target.transform.SetParent (null);
		target.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 6.5f, gameObject.transform.position.z);
		gameObject.SetActive (true);
		effectPS.Play ();
		if (powerType == 1) { //Bomb
			anim.SetInteger ("State", 1);
		} else if (powerType == 2) { //Big Mode
			gameObject.GetComponent<SpriteRenderer> ().enabled = true;
			anim.SetInteger ("State", 2);
		} else if (powerType == 3) {
			anim.SetInteger ("State", 3);
		}else{ //Might need later
		}
	}

	//After 10 seconds end Blue powerup
	IEnumerator endBigMode(){
		yield return new WaitForSeconds (10f);
		controlScript.toggleBigMode (false);
		disablePowerUp ();
	}

	//After 6 seconds end Bird Green powerup
	IEnumerator endSpring(){
		yield return new WaitForSeconds (6f);
		controlScript.powerUpActive = false;
		controlScript.fallingDude.GetComponent<Bouncer>().setGreenPowerUp(false);
		disablePowerUp ();
	}

	//Not used (tornado)
	IEnumerator endVortex(){
		yield return new WaitForSeconds (6.25f);
		controlScript.powerUpActive2 = false;
		controlScript.fallingDude.GetComponent<Bouncer>().setGreenPowerUp(false);
		disablePowerUp ();
	}

	//Handle catching a powerup while it is already active
	private void givePointsInsteadOfPowerUp(int points){
		controlScript.updateScore (points);
		fText = PoolingManager.pooler.getPooledObject (7);
		fText.SetActive (true);
		fText.GetComponent<FloatingText> ().createPowerUpAlternateText (transform.position, points);
		controlScript.delayedResetFText(fText);
	}

	//Display the particle effect for catching the powerup
	private void createCaughtEffect(int select, int points){
		controlScript.displayPowerCaughtEffect (transform.position, powerType);
		controlScript.updateScore (points);
	}

	//Reset variables before setting active to be used again from pooler
	public void disablePowerUp(){
		target.transform.SetParent (gameObject.transform);
		effectPS.transform.SetParent (gameObject.transform);
		gameObject.transform.SetParent (PoolingManager.pooler.gameObject.transform);
		if (powerType == 2 || powerType == 3) {
			gameObject.GetComponent<SpriteRenderer> ().enabled = true;
			gameObject.GetComponent<CapsuleCollider2D> ().enabled = true;
		}
		collided = false;
		processed = false;
		caught = false;
		greenType = 0;
		gameObject.SetActive (false);
	}

	//If game ends before Big mode expires, stop the coroutine waiting to end it(endBigMode())
	public void stopBigModeCoroutine(){
		StopAllCoroutines ();
		controlScript.toggleBigMode (false);
		disablePowerUp ();
	}

	//If game ends before Bird Green powerup expires, stop the coroutine waiting to end it(endSpring())
	public void stopGreenCoroutine(){
		StopAllCoroutines ();
		if (greenType == 1) {
			controlScript.powerUpActive = false;
		}else if(greenType == 2){
			controlScript.powerUpActive2 = false;
		}
		controlScript.fallingDude.GetComponent<Bouncer>().setGreenPowerUp(false);
		disablePowerUp ();
	}

	//Clear the reference to the controller before changing scenes
	public void resetController(){
		controlScript = null;
	}

	//Check if the dude is ducks or not, important for green powerup
	private bool checkDudeType(){
		if(controlScript.fallingDudeName == "Duck2" || controlScript.attachedDudeName == "Duck1"){
			return false;
		}else{
			return true;
		}
	}
}
