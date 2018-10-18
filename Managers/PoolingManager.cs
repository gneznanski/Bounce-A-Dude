using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoolingManager : MonoBehaviour {
	public static PoolingManager pooler;
	public GameObject powerUpPrefab, explodePrefab, balloonPrefab, fTextPrefab, start1, target1, start2, target2, start3, target3;
	public ParticleSystem popPSPrefab, redMissPSPrefab, blueMissPSPrefab, greenMissPSPrefab, redCaughtPSPrefab, blueCaughtPSPrefab, greenCaughtPSPrefab;
	public GameObject[] red, blue, green, explode, balloons, fTexts;
	public ParticleSystem[] popPS, redMissPS, blueMissPS, greenMissPS, redCaughtPS, blueCaughtPS, greenCaughtPS;
	public int redIndex, blueIndex, greenIndex, explodeIndex, balloonIndex, popIndex, fTextIndex,redMissPSIndex, blueMissPSIndex, greenMissPSIndex,
				redCaughtPSIndex, blueCaughtPSIndex, greenCaughtPSIndex;
	public bool balloonsReady, firstTime;
	private const int powerUpSize = 20;
	private const int explodeSize = 10;
	private const int balloonSize = 40;
	private const int popSize = 40;
	private const int fTextSize = 40;
	private const int redMissPSSize = 10;
	private const int blueMissPSSize = 10;
	private const int greenMissPSSize = 10;
	private const int redCaughtPSSize = 10;
	private const int blueCaughtPSSize = 10;
	private const int greenCaughtPSSize = 10;
	private int activeMissIndex, activeCaughtIndex;

	void Awake(){
		if (pooler == null) {
			pooler = this;
			DontDestroyOnLoad (this);
		} else if (this != pooler) {
			Destroy (this.gameObject);
		}
	}

	// Use this for initialization
	void Start () {
		firstTime = true;
		green = new GameObject[powerUpSize];
		red = new GameObject[powerUpSize];
		blue = new GameObject[powerUpSize];
		explode = new GameObject[explodeSize];
		balloons = new GameObject[balloonSize];
		popPS = new ParticleSystem[popSize];
		fTexts = new GameObject[fTextSize];
		redMissPS = new ParticleSystem[redMissPSSize];
		blueMissPS = new ParticleSystem[blueMissPSSize];
		greenMissPS = new ParticleSystem[greenMissPSSize];
		redCaughtPS = new ParticleSystem[redCaughtPSSize];
		blueCaughtPS = new ParticleSystem[blueCaughtPSSize];
		greenCaughtPS = new ParticleSystem[greenCaughtPSSize];

		//Instantiate the objects for each array
		for (int i = 0; i < (powerUpSize / 4); i++) {
			createRed ();
		}
		for (int i = 0; i < (powerUpSize / 4); i++) {
			createBlue ();
		}
		for (int i = 0; i < (powerUpSize / 4); i++) {
			createGreen ();
		}
		for (int i = 0; i < explodeSize / 2; i++) {
			createExplode ();
		}
		for (int i = 0; i < balloonSize - 10; i++) {
			createBalloons ();
		}
		balloonsReady = true;
		firstTime = false;
		for (int i = 0; i < popSize - 10; i++) {
			createPopEffects ();
		}
		for (int i = 0; i < fTextSize - 10; i++) {
			createFtexts ();
		}
		for (int i = 0; i < redMissPSSize / 2; i++) {
			createRedMissPS ();
		}
		for (int i = 0; i < blueMissPSSize / 2; i++) {
			createBlueMissPS ();
		}
		for (int i = 0; i < greenMissPSSize / 2; i++) {
			createGreenMissPS ();
		}
		for (int i = 0; i < redCaughtPSSize / 2; i++) {
			createRedCaughtPS ();
		}
		for (int i = 0; i < blueCaughtPSSize / 2; i++) {
			createBlueCaughtPS ();
		}
		for (int i = 0; i < greenCaughtPSSize / 2; i++) {
			createGreenCaughtPS ();
		}

		System.GC.Collect(); //Clear the GC from Instantiate memory before the game action starts to increase fps
	}
	
	// Update is called once per frame
	void Update () {
	}

	//Returns the requested pooled object, only returns one that isnt already active in the game
	public GameObject getPooledObject(int type){
		if (type == 1) { //Red
			for (int i = 0; i < redIndex; i++) {
				if (!red [i].activeInHierarchy) {
					return red [i];
				}
			}
		} else if (type == 2) { //Blue
			for (int i = 0; i < blueIndex; i++) {
				if (!blue [i].activeInHierarchy) {
					return blue [i];
				}
			}
		} else if (type == 3) { //Green
			for (int i = 0; i < greenIndex; i++) {
				if (!green [i].activeInHierarchy) {
					return green [i];
				}
			}
		} else if (type == 4) { //Explode
			for (int i = 0; i < explodeIndex; i++) {
				if (!explode [i].activeInHierarchy) {
					return explode [i];
				}
			}
		}else if (type == 5) { //Balloon
			for (int i = 0; i < balloonIndex; i++) {
				if (!balloons [i].activeInHierarchy) {
					return balloons [i];
				}
			}
		}
		else if (type == 6) { //Pop PS Effect
			for (int i = 0; i < popIndex; i++) {
				if (!popPS [i].gameObject.activeInHierarchy) {
					return popPS [i].gameObject;
				}
			}
		}else if (type == 7) { //Floating Text
			for (int i = 0; i < fTextIndex; i++) {
				if (!fTexts [i].activeInHierarchy) {
					return fTexts [i];
				}
			}
		}else if (type == 8) { //Red Miss PS
			for (int i = 0; i < redMissPSIndex; i++) {
				if (!redMissPS [i].gameObject.activeInHierarchy) {
					return redMissPS [i].gameObject;
				}
			}
		}else if (type == 9) { //Blue Miss PS
			for (int i = 0; i < blueMissPSIndex; i++) {
				if (!blueMissPS [i].gameObject.activeInHierarchy) {
					return blueMissPS [i].gameObject;
				}
			}
		}else if (type == 10) { //Green Miss PS
			for (int i = 0; i < greenMissPSIndex; i++) {
				if (!greenMissPS [i].gameObject.activeInHierarchy) {
					return greenMissPS [i].gameObject;
				}
			}
		}else if (type == 11) { //Red Caught PS
			for (int i = 0; i < redCaughtPSIndex; i++) {
				if (!redCaughtPS [i].gameObject.activeInHierarchy) {
					return redCaughtPS [i].gameObject;
				}
			}
		}else if (type == 12) { //Blue Caught PS
			for (int i = 0; i < blueCaughtPSIndex; i++) {
				if (!blueCaughtPS [i].gameObject.activeInHierarchy) {
					return blueCaughtPS [i].gameObject;
				}
			}
		}else if (type == 13) { //Green Caught PS
			for (int i = 0; i < greenCaughtPSIndex; i++) {
				if (!greenCaughtPS [i].gameObject.activeInHierarchy) {
					return greenCaughtPS [i].gameObject;
				}
			}
		}

		//IF there are no available pooled object of the requested type, create a new one and return it
		if (type == 1) {
			if (redIndex < powerUpSize) {
				createRed ();
			}
			return red [redIndex - 1];
		} else if (type == 2) {
			if (blueIndex < powerUpSize) {
				createBlue ();
			}
			return blue [blueIndex - 1];
		} else if (type == 3) {
			if (greenIndex < powerUpSize) {
				createGreen ();
			}
			return green [greenIndex - 1];
		} else if (type == 4) {
			if (explodeIndex < explodeSize) {
				createExplode ();
			}
			return explode [explodeIndex - 1];
		} else if (type == 5) {
			if (balloonIndex < balloonSize) {
				createBalloons ();
			}
			return balloons [balloonIndex - 1];
		}else if (type == 6) {
			if (popIndex < popSize) {
				createPopEffects ();
			}
			return popPS [popIndex - 1].gameObject;
		}else if (type == 7) {
			if (fTextIndex < fTextSize) {
				createFtexts ();
			}
			return fTexts [fTextIndex - 1];
		}else if (type == 8) {
			if (redMissPSIndex < redMissPSSize) {
				createRedMissPS ();
			}
			return redMissPS [redMissPSIndex - 1].gameObject;
		}else if (type == 9) {
			if (blueMissPSIndex < blueMissPSSize) {
				createBlueMissPS ();
			}
			return blueMissPS [blueMissPSIndex - 1].gameObject;
		}else if (type == 10) {
			if (greenMissPSIndex < greenMissPSSize) {
				createGreenMissPS ();
			}
			return greenMissPS [greenMissPSIndex - 1].gameObject;
		}else if (type == 11) {
			if (redCaughtPSIndex < redCaughtPSSize) {
				createRedCaughtPS ();
			}
			return redCaughtPS [redCaughtPSIndex - 1].gameObject;
		}else if (type == 12) {
			if (blueCaughtPSIndex < blueCaughtPSSize) {
				createBlueCaughtPS ();
			}
			return blueCaughtPS [blueCaughtPSIndex - 1].gameObject;
		}else if (type == 13) {
			if (greenCaughtPSIndex < greenCaughtPSSize) {
				createGreenCaughtPS ();
			}
			return greenCaughtPS [greenCaughtPSIndex - 1].gameObject;
		}else{
		}
		return null;
	}

	//Create red powerUp energy effect
	private void createRed(){
		red [redIndex] = Instantiate (powerUpPrefab);
		red [greenIndex].name = "Red PowerUp " + greenIndex.ToString ();
		red [redIndex].GetComponent<PowerUp> ().initialize2 (1);
		red [redIndex].transform.SetParent (gameObject.transform);
		red [redIndex].SetActive (false);
		redIndex++;
	}

	//Create blue powerUp energy effect
	private void createBlue(){
		blue [blueIndex] = Instantiate (powerUpPrefab);
		blue [blueIndex].name = "Blue PowerUp " + blueIndex.ToString ();
		blue [blueIndex].GetComponent<PowerUp> ().initialize2 (2);
		blue [blueIndex].transform.SetParent (gameObject.transform);
		blue [blueIndex].SetActive (false);
		blueIndex++;
	}

	//Create green powerUp energy effect
	private void createGreen(){
		green [greenIndex] = Instantiate (powerUpPrefab);
		green [greenIndex].name = "Green PowerUp " + greenIndex.ToString ();
		green [greenIndex].GetComponent<PowerUp> ().initialize2 (3);
		green [greenIndex].transform.SetParent (gameObject.transform);
		green [greenIndex].SetActive (false);
		greenIndex++;
	}

	//Create explosion PS effect
	private void createExplode(){
		explode [explodeIndex] = Instantiate (explodePrefab);
		explode [explodeIndex].name = "Explosion " + explodeIndex.ToString ();
		explode [explodeIndex].transform.SetParent (gameObject.transform);
		explode [explodeIndex].SetActive (false);
		explodeIndex++;
	}

	//Create balloon anchor and balloon
	private void createBalloons(){
		balloons [balloonIndex] = Instantiate (balloonPrefab);
		balloons [balloonIndex].name = "Balloon " + balloonIndex.ToString ();
		balloons [balloonIndex].transform.SetParent (gameObject.transform);
		balloons [balloonIndex].GetComponentInChildren<BalloonCollider> ().anim = balloons [balloonIndex].GetComponentInChildren<Animator> ();
		balloons [balloonIndex].SetActive (false);
		balloonIndex++;
	}

	//Create balloon pop PS effect
	private void createPopEffects(){
		popPS[popIndex] = Instantiate(popPSPrefab);
		popPS [popIndex].name = "Pop PS " + popIndex.ToString ();
		popPS [popIndex].transform.SetParent (this.gameObject.transform);
		popPS [popIndex].gameObject.SetActive (false);
		popIndex++;
	}

	//Create floating text
	private void createFtexts(){
		fTexts[fTextIndex] = Instantiate(fTextPrefab);
		fTexts [fTextIndex].name = "Floating Text " + fTextIndex.ToString ();
		fTexts [fTextIndex].transform.SetParent (this.gameObject.transform);
		fTexts [fTextIndex].GetComponent<FloatingText> ().initialize ();
		fTexts [fTextIndex].SetActive (false);
		fTextIndex++;
	}

	//Create red miss PS Effect
	private void createRedMissPS(){
		redMissPS[redMissPSIndex] = Instantiate(redMissPSPrefab);
		redMissPS [redMissPSIndex].name = "Red Miss PowerUp PS " + redMissPSIndex.ToString ();
		redMissPS [redMissPSIndex].transform.SetParent (this.gameObject.transform);
		redMissPS [redMissPSIndex].gameObject.SetActive (false);
		redMissPSIndex++;
	}

	//Create blue miss PS Effect
	private void createBlueMissPS(){
		blueMissPS[blueMissPSIndex] = Instantiate(blueMissPSPrefab);
		blueMissPS [blueMissPSIndex].name = "Blue Miss PowerUp PS " + blueMissPSIndex.ToString ();
		blueMissPS [blueMissPSIndex].transform.SetParent (this.gameObject.transform);
		blueMissPS [blueMissPSIndex].gameObject.SetActive (false);
		blueMissPSIndex++;
	}

	//Create green miss PS Effect
	private void createGreenMissPS(){
		greenMissPS[greenMissPSIndex] = Instantiate(greenMissPSPrefab);
		greenMissPS [greenMissPSIndex].name = "Green Miss PowerUp PS " + greenMissPSIndex.ToString ();
		greenMissPS [greenMissPSIndex].transform.SetParent (this.gameObject.transform);
		greenMissPS [greenMissPSIndex].gameObject.SetActive (false);
		greenMissPSIndex++;
	}

	//Create red caught PS Effect
	private void createRedCaughtPS(){
		redCaughtPS[redCaughtPSIndex] = Instantiate(redCaughtPSPrefab);
		redCaughtPS [redCaughtPSIndex].name = "Red Caught PowerUp PS " + redCaughtPSIndex.ToString ();
		redCaughtPS [redCaughtPSIndex].transform.SetParent (this.gameObject.transform);
		redCaughtPS [redCaughtPSIndex].gameObject.SetActive (false);
		redCaughtPSIndex++;
	}

	//Create blue caught PS Effect
	private void createBlueCaughtPS(){
		blueCaughtPS[blueCaughtPSIndex] = Instantiate(blueCaughtPSPrefab);
		blueCaughtPS [blueCaughtPSIndex].name = "Blue Caught PowerUp PS " + blueCaughtPSIndex.ToString ();
		blueCaughtPS [blueCaughtPSIndex].transform.SetParent (this.gameObject.transform);
		blueCaughtPS [blueCaughtPSIndex].gameObject.SetActive (false);
		blueCaughtPSIndex++;
	}

	//Create green caught PS Effect
	private void createGreenCaughtPS(){
		greenCaughtPS[greenCaughtPSIndex] = Instantiate(greenCaughtPSPrefab);
		greenCaughtPS [greenCaughtPSIndex].name = "Green Caught PowerUp PS " + blueCaughtPSIndex.ToString ();
		greenCaughtPS [greenCaughtPSIndex].transform.SetParent (this.gameObject.transform);
		greenCaughtPS [greenCaughtPSIndex].gameObject.SetActive (false);
		greenCaughtPSIndex++;
	}

	//Disable and reset any pooled objects before changing scenes
	public void checkForActivePooledObjects(){
		for(int i = 0; i < redIndex; i++){
			if(red[i].activeInHierarchy){
				red [i].GetComponent<PowerUp>().disablePowerUp();
			}
		}
		for(int i = 0; i < blueIndex; i++){
			if(blue[i].activeInHierarchy){
				blue [i].GetComponent<PowerUp>().disablePowerUp();
			}
		}
		for(int i = 0; i < greenIndex; i++){
			if(green[i].activeInHierarchy){
				green [i].GetComponent<PowerUp>().disablePowerUp();
			}
		}
		for(int i = 0; i < balloonIndex; i++){
			if(balloons[i].activeInHierarchy){
				resetBalloon (balloons [i]);
			}
		}
		balloonsReady = true;

		for(int i = 0; i < popIndex; i++){
			if(popPS[i].gameObject.activeInHierarchy){
				resetPopEffects (popPS [i].gameObject);
			}
		}
		for(int i = 0; i < fTextIndex; i++){
			if(fTexts[i].activeInHierarchy){
				resetFtext (fTexts [i]);
			}
		}
		for(int i = 0; i < redMissPSIndex; i++){
			if(redMissPS[i].gameObject.activeInHierarchy){
				resetPopEffects (redMissPS [i].gameObject);
			}
		}
		for(int i = 0; i < blueMissPSIndex; i++){
			if(blueMissPS[i].gameObject.activeInHierarchy){
				resetPopEffects (blueMissPS [i].gameObject);
			}
		}
		for(int i = 0; i < greenMissPSIndex; i++){
			if(greenMissPS[i].gameObject.activeInHierarchy){
				resetPopEffects (greenMissPS [i].gameObject);
			}
		}
		for(int i = 0; i < redCaughtPSIndex; i++){
			if(redCaughtPS[i].gameObject.activeInHierarchy){
				resetPopEffects (redCaughtPS [i].gameObject); 
			}
		}
		for(int i = 0; i < blueCaughtPSIndex; i++){
			if(blueCaughtPS[i].gameObject.activeInHierarchy){
				resetPopEffects (blueCaughtPS [i].gameObject); 
			}
		}
		for(int i = 0; i < greenCaughtPSIndex; i++){
			if(greenCaughtPS[i].gameObject.activeInHierarchy){
				resetPopEffects (greenCaughtPS [i].gameObject); 
			}
		}
	}

	//Get new reference to the controller gameobject in the new scene
	public void resetControllers(){
		for(int i = 0; i < redIndex; i++){
			red [i].GetComponent<PowerUp> ().resetController ();
		}
		for(int i = 0; i < blueIndex; i++){
			blue [i].GetComponent<PowerUp> ().resetController ();
		}
		for(int i = 0; i < greenIndex; i++){
			green [i].GetComponent<PowerUp> ().resetController ();
		}
	}

	//Reset the balloon for reuse
	public void resetBalloon(GameObject obj){
		BalloonCollider balCol = obj.GetComponentInChildren<BalloonCollider> ();
		balCol.gameObject.transform.localPosition = new Vector3 (0, 0, 0);
		balCol.popped = false;
		balCol.collided = false;
		balCol.updated = false;
		balCol.alreadyOff = false;
		balCol.bomb = false;
		obj.transform.SetParent (this.gameObject.transform);
		obj.SetActive (false);
	}

	//Reset the pop PS effect for reuse
	public void resetPopEffects(GameObject obj){
		obj.transform.SetParent (this.gameObject.transform);
		obj.GetComponent<ParticleSystem> ().Stop ();
		obj.SetActive (false);
	}

	//Reset the fText for reuse
	public void resetFtext(GameObject obj){
		obj.GetComponentInChildren<Animator> ().SetInteger ("State", 0);
		obj.transform.SetParent (this.gameObject.transform, false);
		obj.SetActive (false);
	}
}
