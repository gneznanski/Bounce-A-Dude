using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BalloonMovement : MonoBehaviour{
	//public GameObject balloonPrefab1, balloonPrefab2, balloonPrefab3, balloonPrefab4, balloonPrefab5, balloonPrefab6, bombPrefab;
	//public ParticleSystem particle1, particle2, particle3, particle4, particle5, particle6;
	public GameObject[] row1, row2, row3;
	public GameObject start1, start2, start3, target1, target2, target3;
	public Text bonusText;
	public AudioClip bonusSound;
	public bool paused;
	public int index, numBalloons, row1Pop, row2Pop, row3Pop, bombIndex;
	private Controller controlScript;
	private GameObject cannon;
	private AudioSource source;
	private float offset;
	private int curColorRow1, curColorRow2, curColorRow3, row1Multiplier, row2Multiplier, row3Multiplier;
	private bool pausingAudio, bomb, pausingControls, sourceWaitingForUser;

	private void Awake ()
	{
		if (PlayerPrefs.HasKey ("currentBalloons")) { //Load balloon information from PlayerPrefs
			row1Pop = PlayerPrefs.GetInt ("row1Pop"); //Number of popped balloons
			row2Pop = PlayerPrefs.GetInt ("row2Pop");
			row3Pop = PlayerPrefs.GetInt ("row3Pop");

			//Determine which prefab color should be used, get from PlayerPrefs
			if (PlayerPrefs.GetInt ("row1Prefab") == 1) {
				curColorRow1 = 1;
			} else {
				curColorRow1 = 4;
			}
			if (PlayerPrefs.GetInt ("row2Prefab") == 2) {
				curColorRow2 = 2;
			} else {
				curColorRow2 = 6;
			}
			if (PlayerPrefs.GetInt ("row3Prefab") == 3) {
				curColorRow3 = 3;
			} else {
				curColorRow3 = 5;
			}
		} else { //New game, reset values in PlayerPrefs
			PlayerPrefs.SetInt ("currentBalloons", 1);
			row1Pop = 0;
			row2Pop = 0;
			row3Pop = 0;
			PlayerPrefs.SetInt ("row1Pop", row1Pop);
			PlayerPrefs.SetInt ("row2Pop", row2Pop);
			PlayerPrefs.SetInt ("row3Pop", row3Pop);
			PlayerPrefs.SetInt ("row1Prefab", 1);
			PlayerPrefs.SetInt ("row2Prefab", 2);
			PlayerPrefs.SetInt ("row3Prefab", 3);
			PlayerPrefs.SetInt ("row1Reset", 0);
			PlayerPrefs.SetInt ("row2Reset", 0);
			PlayerPrefs.SetInt ("row3Reset", 0);
			curColorRow1 = 1;
			curColorRow2 = 2;
			curColorRow3 = 3;
		}
	}

	// Use this for initialization
	void Start ()
	{
		controlScript = gameObject.GetComponent<Controller> ();
		bonusText = GameObject.Find ("Bonus Text").GetComponent<Text> ();
		source = GetComponent<AudioSource> ();
		bonusText.text = "";
		numBalloons = 10;
		offset = .82f;
		paused = false;
		row1 = new GameObject[numBalloons];
		row2 = new GameObject[numBalloons];
		row3 = new GameObject[numBalloons];
		cannon = GameObject.Find ("Right Cannon Target");
		pausingAudio = false;
		bomb = false;
		bombIndex = 1;
		row1Multiplier = 0;
		row2Multiplier = 0;
		row2Multiplier = 0;
	}

	// Update is called once per frame
	void Update ()
	{
		if (bonusText == null) {
			bonusText = GameObject.Find ("Bonus Text").GetComponent<Text> ();
		}
		if(pausingAudio){
			if(!source.isPlaying){
				cannon.GetComponent<CannonAudioController> ().getSource ().UnPause ();
				pausingAudio = false;
			}else{
			}
		}
		if(SmScript.gameManager.showingAd){ //Stop music playing before an ad
			if(source.isPlaying){
				source.Stop ();
			}
		}
		if(Time.timeScale == 0){
			if(source.isPlaying && controlScript.canAudioPause){
				source.Pause ();
				sourceWaitingForUser = true;
			}else{
				
			}
		}else{
			if(sourceWaitingForUser){
				source.UnPause ();
				sourceWaitingForUser = false;
			}
		}
		if(PoolingManager.pooler.balloonsReady){
			setupBalloons2 ();
			PoolingManager.pooler.balloonsReady = false;
		}

//		if(source.isPlaying){
//			if(controlScript.bgmGreenPause){
//				source.mute = true;
//			}else{
//				source.mute = false;
//			}
//		}
	}

	//Initial setup of balloon rows, using pooled objects
	void setupBalloons2(){
		createBalloonsRecycle (1, curColorRow1, "Row1Balloon", 0, false);
		createBalloonsRecycle (2, curColorRow2, "Row2Balloon", 0, false);
		createBalloonsRecycle (3, curColorRow3, "Row3Balloon", 0, false);
		if(PlayerPrefs.GetInt("row1Reset") == 1){
			checkRow (1, curColorRow1, "Row1Balloon");
		}
		if(PlayerPrefs.GetInt("row2Reset") == 1){
			checkRow (2, curColorRow2, "Row2Balloon");
		}
		if(PlayerPrefs.GetInt("row3Reset") == 1){
			checkRow (3, curColorRow3, "Row3Balloon");
		}
	}

	//Increment when a balloon is popped, call bonus if all balloons in a row are popped
	public void addPopped (int row)
	{
		//Update stuff, not sure why I have to do this, but it wont work unless I update these variables
		bonusText = GameObject.Find ("Bonus Text").GetComponent<Text> ();
		numBalloons = 10;
		offset = .82f;
		row1Pop = PlayerPrefs.GetInt ("row1Pop");
		row2Pop = PlayerPrefs.GetInt ("row2Pop");
		row3Pop = PlayerPrefs.GetInt ("row3Pop");

		//Increment the popped counter for the appropriate row
		if (row == 1) {
			row1Pop++;
			if (row1Pop >= numBalloons) {
				PlayerPrefs.SetInt("row1Reset", 1);
				rowCleared(1);
				row1Pop = 0; //Reset the number of balloons popped
			}
			PlayerPrefs.SetInt ("row1Pop", row1Pop); //Update PlayerPrefs to reflect the number of balloons in the row that have been popped
		} else if (row == 2) {
			row2Pop++;
			if (row2Pop >= numBalloons) {
				PlayerPrefs.SetInt("row2Reset", 1);
				rowCleared(2);
				row2Pop = 0;
			}
			PlayerPrefs.SetInt ("row2Pop", row2Pop);
		} else {
			row3Pop++;
			if (row3Pop >= numBalloons) {
				PlayerPrefs.SetInt("row3Reset", 1);
				rowCleared(3);
				row3Pop = 0;
			}
			PlayerPrefs.SetInt ("row3Pop", row3Pop);
		}
	}

	//Timing of bonus method depends on whether a bomb was exploded
	void rowCleared(int row){
		if(controlScript.isExploding()){
			unPauseStart2 (row);
		}else{
			paused = true; //Pause game
			bonus (row);
		}
	}

	//Run bonus event for clearing a row, create new row of different color
	void bonus (int row)
	{
		//Bonus points for clearing a row
		int row1Bonus = 500 + (75 * row1Multiplier);
		int row2Bonus = 200 + (25 * row2Multiplier);
		int row3Bonus = 400 + (50 * row3Multiplier);
		numBalloons = 10;
		
		if (source.isPlaying) { //Prevent overlapping row clear sounds
			source.Stop ();
		}
		if (!controlScript.gameOver) {
			controlScript.pauseBGMusic ("Bonus", source);
			//source.PlayOneShot (bonusSound, .75f);
			fadeInMusic();
		}
		
		Time.timeScale = 0; //Pause game

		//Dont let the seesaw move while the game is paused during green powerUp
		if(controlScript.fallingDudeName != "Duck2"){
			if(controlScript.fallingDude.GetComponent<Bouncer>().getGreenPowerUp()){
				//GameObject.Find("Scroll View").GetComponent<ScrollRect>().horizontal = false;
				controlScript.pausingControls(true);
				pausingControls = true;
			}
		}

		//Change balloon color, display bonus for popped row, start timer for pause
		if (row == 1) {
			if(PlayerPrefs.GetInt("row1Prefab") == 1){
				curColorRow1 = 4;
				PlayerPrefs.SetInt ("row1Prefab", 4);
			}else{
				curColorRow1 = 1;
				PlayerPrefs.SetInt ("row1Prefab", 1);
			}
			bonusText.text = "Row Cleared! Bonus " + row1Bonus.ToString () + " points!";
			unPauseStart (row, curColorRow1, "Row1Balloon", row1Bonus);
			row1Multiplier++;
		} else if (row == 2) {
			if(PlayerPrefs.GetInt("row2Prefab") == 2){
				curColorRow2 = 6;
				PlayerPrefs.SetInt ("row2Prefab", 6);
			}else{
				curColorRow2 = 2;
				PlayerPrefs.SetInt ("row2Prefab", 2);
			}
			bonusText.text = "Row Cleared! Bonus " + row2Bonus.ToString () + " points!";
			unPauseStart (row, curColorRow2, "Row2Balloon", row2Bonus);
			row2Multiplier++;
		} else {
			if(PlayerPrefs.GetInt("row3Prefab") == 3){
				curColorRow3 = 5;
				PlayerPrefs.SetInt ("row3Prefab", 5);
			}else{
				curColorRow3 = 3;
				PlayerPrefs.SetInt ("row3Prefab", 3);
			}
			bonusText.text = "Row Cleared! Bonus " + row3Bonus.ToString () + " points!";
			unPauseStart (row, curColorRow3, "Row3Balloon", row3Bonus);
			row3Multiplier++;
		}
	}

	//Set up a row of balloons using pooled objects instead of instantiating new prefabs
	void createBalloonsRecycle(int row, int color, string name, int points, bool reset){
		bool placeBomb = false;
		Color rowColor;
		int balloonScore;

		//Create the color for the pop particle effect
		if(color == 1){
			rowColor = new Color (240/255f, 105/255f, 2/255f, 1f); //Orange
		}else if (color == 2){
			rowColor = new Color (237/255f, 251/255f, 0f, 1f); //Yellow
		}else if (color == 3){
			rowColor = new Color (61/255f, 255/255f, 36/255f, 1f); //Green
		}else if(color == 4){
			rowColor = new Color (2/255f, 43/255f, 240/255f, 1f); //Blue
		}else if(color == 5){
			rowColor = new Color (167/255f, 0f, 255/255f, 1f); //Purple
		}else{
			rowColor = new Color (251/255f, 5/255f, 0f, 1f); //Red
		}

		offset = .82f; //Balloon spacing

		GameObject start, target;
		if (row == 1) {
			start = start1;
			target = target1;
			balloonScore = 30;// + (4 * controlScript.speedLevel);
		} else if (row == 2) {
			start = start2;
			target = target2;
			balloonScore = 10;// + (4 * controlScript.speedLevel);
		} else {
			start = start3;
			target = target3;
			balloonScore = 20;// + (4 * controlScript.speedLevel);
		}

		if (reset) {
			resetRowObjects(row);
			controlScript.updateScore (points); //Update Score, bonus row clear points
		}

		for (int i = 0; i < numBalloons; i++) {
			GameObject newBalloon = PoolingManager.pooler.getPooledObject (5);
			float spacing = i * offset; 
			if (row == 3) { //For balloons moving right to left, spacing is negative
				spacing = -spacing;
			}

			if (newBalloon != null) { //If the gameobject returned from the pooler is available, use it as the next balloon
				BalloonCollider balCol = newBalloon.GetComponentInChildren<BalloonCollider> ();
				BalloonAnchor balAnchor = newBalloon.GetComponent<BalloonAnchor> ();
				newBalloon.SetActive (true);
				newBalloon.transform.SetParent (null);

				if (i == 0) { //Position the balloons
					newBalloon.transform.position = new Vector2 (start.transform.position.x, start.transform.position.y);
				} else {
					newBalloon.transform.position = new Vector2 (start.transform.position.x + spacing, start.transform.position.y);
				}

				//Place a bomb instead of balloon if needed
				if (bomb && i == bombIndex) { 
					balCol.bomb = true;
					bomb = false;
					placeBomb = true;
				}

				//Initiallize variables
				balCol.turnOnSafe ();
				newBalloon.name = name + i.ToString () + " anchor";
				balCol.name = name + i.ToString ();
				balCol.row = row;
				balCol.index = i;
				balCol.color = rowColor;
				balCol.pointValue = balloonScore;
				balCol.poppedByExplosion = false;
				balAnchor.begin = start;
				balAnchor.target = target;

				//Set the dispay to bomb animation or the correct color balloon
				if (placeBomb) { 
					balCol.anim.SetInteger ("State", 7);
					placeBomb = false;
				} else {
					balCol.anim.SetInteger ("State", color);
				}

				//Save the balloon info to PlayerPrefs
				if (!PlayerPrefs.HasKey (name + i.ToString ())) { 
					PlayerPrefs.SetInt (name + i.ToString (), 1);
				}

				//Insert the new balloon into the row array
				if (row == 1) { 
					row1 [i] = newBalloon;
				} else if (row == 2) {
					row2 [i] = newBalloon;
				} else {
					row3 [i] = newBalloon;
				}
			}
		}
	}

	//Reset balloons to be reused
	void resetRowObjects(int row){
		if (row == 1) {
			for (int i = 0; i < numBalloons; i++) {
				PoolingManager.pooler.resetBalloon (row1 [i]);
				PlayerPrefs.SetInt ("Row1Balloon" + i.ToString (), 1);
			}
		} else if (row == 2) {
			for (int i = 0; i < numBalloons; i++) {
				PoolingManager.pooler.resetBalloon (row2 [i]);
				PlayerPrefs.SetInt ("Row2Balloon" + i.ToString (), 1);
			}
		} else {
			for (int i = 0; i < numBalloons; i++) {
				PoolingManager.pooler.resetBalloon (row3 [i]);
				PlayerPrefs.SetInt ("Row3Balloon" + i.ToString (), 1);
			}
		}
	}

	//2 methods - delay when row cleared, unpause game, reset bonus text
	public void unPauseStart (int row, int color, string name, int points)
	{
		StartCoroutine (unPause (row, color, name, points));
	}

	IEnumerator unPause (int row, int color, string name, int points)
	{
		yield return new WaitForSecondsRealtime (2.25f);
		createBalloonsRecycle (row, color, name, points, true);
		if(row == 1){
			PlayerPrefs.SetInt("row1Reset", 0);
		}else if(row == 2){
			PlayerPrefs.SetInt("row2Reset", 0);
		}else{
			PlayerPrefs.SetInt("row3Reset", 0);
		}
		if (!controlScript.canAudioPause) {
			Time.timeScale = controlScript.getGameSpeed ();
			if (pausingControls) {
				pausingControls = false;
				//GameObject.Find("Scroll View").GetComponent<ScrollRect>().horizontal = true; //Allow the seesaw to move when the game unpauses
				controlScript.pausingControls (false);
			}
		}
		paused = false;
		bonusText.text = "";
	}

	//2 methods - delay when row cleared, unpause game, reset bonus text
	public void unPauseStart2 (int row)
	{
		StartCoroutine (unPause2 (row));
	}

	IEnumerator unPause2 (int row)
	{
		yield return new WaitForSecondsRealtime (.4f);
		paused = true;
		bonus (row);
	}

	//Unpause the cannon sequence audio after a pause for bonus audio
	void audioHelperUnpause(){
		cannon.GetComponent<CannonAudioController>().getSource().UnPause ();
	}

	//Prep to place a bomb in a row.  Getter/Setter
	public void setBombTemp(){
		bomb = true;
	}
	public bool bombIsSet(){
		return bomb;
	}

	//Ensure that the row clears after a bomb explosion.  Some balloons might be offscreen and not pop, this makes sure they all pop for the row clear
	public void makeSureRowHasCleared(int row){
		string name;
		if (row == 1) {
			name = "Row1Balloon";
		} else if (row == 2) {
			name = "Row2Balloon";
		} else {
			name = "Row3Balloon";
		}

		for (int i = 0; i < numBalloons; i++) {
			GameObject bal = GameObject.Find (name + i.ToString ());
			if (!bal.GetComponent<BalloonCollider> ().popped) {
				bal.GetComponent<BalloonCollider> ().popBalloon ();
			}
		}
	}

	//Check the row to make sure that a row is always shown.  Need this because game can be closed during a row clear bonus pause and not finish remaking the row for next time
	private void checkRow(int row, int color, string name){
		bool needsReset = false;
		int popped;
		if(row == 1){
			popped = row1Pop;
		}else if(row == 2){
			popped = row2Pop;
		}else{
			popped = row3Pop;
		}
		if (popped == 0) {
			for (int i = 0; i < numBalloons; i++) {
				if (PlayerPrefs.GetInt (name + i.ToString ()) == 0) {
					PlayerPrefs.SetInt (name + i.ToString (), 1);
					needsReset = true;
				}
			}
		}
		if (needsReset) {
			createBalloonsRecycle (row, color, name, 0, true);
			PlayerPrefs.SetInt ("row" + row.ToString () + "Reset", 0);
		}
	}

	//Update points after speed level increases
	public void updateBalloonPointValues(int speedLevel){
		for(int i = 0; i < row1.Length; i++){
			row1 [i].GetComponentInChildren<BalloonCollider> ().pointValue += (4 * speedLevel);
		}
		for(int i = 0; i < row2.Length; i++){
			row2 [i].GetComponentInChildren<BalloonCollider> ().pointValue += (4 * speedLevel);
		}
		for(int i = 0; i < row3.Length; i++){
			row3 [i].GetComponentInChildren<BalloonCollider> ().pointValue += (4 * speedLevel);
		}
	}

	void fadeInMusic(){
		source.volume = .00001f;
		source.clip = bonusSound;
		source.Play ();
		while(source.volume < .75f){
			source.volume += .00001f;
		}
		source.volume = .75f;
	}
}
