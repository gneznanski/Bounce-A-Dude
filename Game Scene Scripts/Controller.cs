using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Controls the game objects in the Game scene
public class Controller : MonoBehaviour
{
	public static int score;
	public Transform skeleton, shark, ninja, ducks;
    public Text loseText, scoreText, livesText, newHiText, hiScoreText, speedLevelText;
    public GameObject leftLimit, rightLimit, leftMid, rightMid, attachedDude, fallingDude, introPlatform, leftBarrel, rightBarrel, bombEffectL, bombEffectR, springAnchor, vortexAnchor, 
				cannonRecoilOutL, cannonRecoilOutR;
	public Button newGameButton, stuckButton;
	public SmScript smScript;
	public AudioClip deathSound, selectSound, introSound, cannonSound, cannonIntroSound, newGameSound, speedUpSound, greenIntroSound, growSound;
	public AudioClip hallHiSound, playerHiSound, pauseSound, unPauseSound, backSound, warningSound, personalHiSound;
	public ParticleSystem cannonSmokeL, cannonSmokeR;
	public GameObject[] powerUps, dudeArray;
	public Queue deadDucks;
	public Color speedColor, speedWarningColor;
	public bool gameOver, animationDone, duckSequenceDone, powerUpActive, powerUpActive2, canAudioPause, bgmBonusPause, bgmGreenPause;
	public string attachedDudeName, fallingDudeName;
	public int duckCount, ducksDead, numBalPopPreExplode, speedLevel, speedMultiplier, powerUpCounter, powerUpMax;
	private Transform dude;
	private TeeterBase teeterScript;
	private FadeController fadeScript;
	private GameObject scrollBar, cart, teeter, cannonR, cannonL, cannonTarL, cannonTarR, newGameDisplay, bEffectL, bEffectR, speedFText, menuDisplay, pauseDisplay, pauseMenuDisplay, pausePanelViewPort,
				gameOverDisplay, gameOverPS, platformBanners, confirmPanel;
	private GameObject[] explodeArray, bombFTexts;
	private ParticleSystem[] missEffects, caughtEffects;
	private AudioSource source, source2, bonusAudioSource, greenAudioSource;
	private ScrollRect controlScroller;
	private Vector3 cannonRStartVector, cannonLStartVector;
	private Button pauseTouchField, menuTouchField, resumeTouchField, pauseMenuTouchField, confirmPanelYesButton, confirmPanelYesButtonTouchField, confirmPanelNoButton, confirmPanelNoButtonTouchField;
	private Animator hiScoreBanner1, hiScoreBanner2;
    private bool soundPlayed = false;
	private float startingPointAttachedX, startingPointAttachedY, startingPointFallingX, startingPointFallingY, startX, duckTimer, 
				introTime, offset, speed, gameSpeed, explodeSpeed;
	private bool intro, cartReady, cartSet, movingInR, movingInL, introSoundPlayed, exploding, deathTextDisplayed, pausing, recoilOut, cannonRecoil, recoilOutDoneR, recoilOutDoneL, 
				recoilInDoneR, recoilInDoneL;
	private int lives, hiScore, ducksSpawned, cannonSide, instructionCounter, scoreInterval, powerUpIndex, explodeIndex, activeExplosions, bombFTextsIndex, missEffectsIndex, caughtEffectsIndex,
				speedBreakPoint, scoreHalf;
	private float speedRecoilIn = .25f;
	private float speedRecoilOut = 100f;
	private const float skeleAttachedX = 2.52f;
	private const float skeleAttachedY = .5f;
	private const float skeleFallingX = 3.5f;//3.559
	private const float skeleFallingY = 2.973f;
	private const float sharkAttachedX = 2.52f;
	private const float sharkAttachedY = .5f;
	private const float sharkFallingX = 3.5f;
	private const float sharkFallingY = 3f;
	private const float ninjaAttachedX = 2.52f;
	private const float ninjaAttachedY = .5f;
	private const float ninjaFallingX = 3.49f;
	private const float ninjaFallingY = 2.904f; 
	private const float duckAttachedX = 2.52f;
	private const float duckAttachedY = .5f;
	private const float duckFallingX = 3.58f;//3.6
	private const float duckFallingY = 3.05f;//2.7
	private const string char1 = "Bones McGee";
	private const string char2 = "Bruce";
	private const string char3 = "Ninja";
	private const string char4 = "Duck Team";

    private void Awake()
    {
		if (PlayerPrefs.HasKey("currentGame")) //Load from playerprefs	
        {
            score = PlayerPrefs.GetInt("score");
            lives = PlayerPrefs.GetInt("lives");
        }
        else //New game
        {
            score = 0;
            lives = 2;
            PlayerPrefs.SetInt("currentGame", 0);
            PlayerPrefs.SetInt("score", score);
            PlayerPrefs.SetInt("lives", lives);
            if (!PlayerPrefs.HasKey("hiScore"))
            {
                PlayerPrefs.SetInt("hiScore", 0);
            }
        }
        gameOver = false;
    }

    // Use this for initialization
    void Start()
	{
		Time.timeScale = 1;
		source = GetComponent<AudioSource> ();
		source2 = GameObject.Find ("AudioEmpty").GetComponent<AudioSource> ();
		loseText.text = "";
		scoreText.text = "Score: " + score.ToString ("N0");
		livesText.text = "Lives: " + lives.ToString ();
		hiScore = PlayerPrefs.GetInt ("hallScore0");
		if (score >= hiScore) { //Make sure the highest score is showing, either the saved high score, or the current game high score if it is greater than saved
			hiScoreText.text = "High Score: " + score.ToString("N0");
		} else {
			hiScoreText.text = "High Score: " + hiScore.ToString("N0");
		}
		newHiText.text = "";
		teeter = GameObject.FindWithTag ("Teeter");
		teeterScript = teeter.GetComponent<TeeterBase> ();
		smScript = GameObject.FindWithTag ("SceneManager").GetComponent<SmScript> ();
		fadeScript = GameObject.Find ("Fade Image1").GetComponent<FadeController> ();
		newGameButton.onClick.AddListener (smScript.startNewGame2);
		newGameDisplay = GameObject.Find ("New Game Display");
		newGameDisplay.gameObject.SetActive (false);
		menuDisplay = GameObject.Find ("Menu Display");
		menuTouchField = GameObject.Find ("Touch Field for Menu Button").GetComponent<Button> ();
		pauseDisplay = GameObject.Find ("Pause Display");
		pauseDisplay.gameObject.SetActive(true);
		pauseMenuDisplay = GameObject.Find ("Pause Panel");
		resumeTouchField = GameObject.Find ("Touch Field: PausePanel Resume Button").GetComponent<Button> ();
		pauseMenuTouchField = GameObject.Find ("Touch Field: PausePanel Menu Button").GetComponent<Button> ();
		pauseMenuDisplay.SetActive (false);
		pausePanelViewPort = GameObject.Find ("Pause Panel ViewPort");
		confirmPanel = GameObject.Find ("Confirmation Panel");
		confirmPanelYesButton = GameObject.Find ("Exit To Menu Confirm Button").GetComponent<Button>();
		confirmPanelYesButtonTouchField = GameObject.Find ("Touch Field: Exit To Menu Confirm Button").GetComponent<Button>();
		confirmPanelNoButton = GameObject.Find ("Exit To Menu Cancel Button").GetComponent<Button>();
		confirmPanelNoButtonTouchField = GameObject.Find ("Touch Field: Exit To Menu Cancel Button").GetComponent<Button>();
		confirmPanel.SetActive (false);
		pausePanelViewPort.SetActive (false);
		menuDisplay.gameObject.SetActive (false);
		scrollBar = GameObject.Find ("Control Image Scrollbar");
		controlScroller = GameObject.Find ("Scroll View").GetComponent<ScrollRect> ();
		cart = GameObject.Find ("Seesaw Base Image");
		cannonR = GameObject.Find ("Right Cannon");
		cannonL = GameObject.Find ("Left Cannon");
		springAnchor = GameObject.Find ("Spring Anchor");
		teeterScript.bungieScript = springAnchor.GetComponent<BungieController> ();
		pauseTouchField = GameObject.Find ("Touch Field for Pause Button").GetComponent<Button> ();
		gameOverDisplay = GameObject.Find ("GameOver Display");
		gameOverDisplay.SetActive (false);
		hiScoreBanner1 = GameObject.Find ("GapBannerL").GetComponent<Animator> ();
		hiScoreBanner2 = GameObject.Find ("GapBannerR").GetComponent<Animator> ();
		platformBanners = GameObject.Find ("PlatformBannersArch");
		explodeArray = new GameObject[20];
		powerUps = new GameObject[100];
		bombFTexts = new GameObject[5];
		missEffects = new ParticleSystem[20];
		caughtEffects = new ParticleSystem[20];
		bombFTextsIndex = 0; 
		intro = true;
		cartReady = false;
		cartSet = false;
		introSoundPlayed = false;
		exploding = false;
		deathTextDisplayed = false;
		powerUpActive = false;
		powerUpActive2 = false;
		pausing = false;
		canAudioPause = false;
		bgmBonusPause = false;
		bgmGreenPause = false;
		ducksSpawned = 3;
		startX = 0;
		duckTimer = -1;
		duckCount = 12;
		ducksDead = 0;
		cannonSide = 1;
		introTime = 0;
		animationDone = false;
		gameSpeed = 1;
		scoreInterval = 0;
		speedLevel = 0;
		speedMultiplier = 0;
		speedBreakPoint = 1000;
		scoreHalf = 500;
		speedLevelText.text = "Speed " + (speedLevel + 1).ToString() + ": " + speedBreakPoint.ToString ("N0");
		explodeSpeed = 5f; //9.25
		powerUpIndex = 0;
		explodeIndex = 0;
		numBalPopPreExplode = 0;
		powerUpCounter = 0;
		powerUpMax = Random.Range (9, 17);

		if (PlayerPrefs.GetInt ("player" + PlayerPrefs.GetString(PlayerPrefs.GetString ("curPlayer")) + "Score") >= 1000) { //Set up or turn off the control instructions. Depends on player experience
			GameObject.Find ("Instruction Text").SetActive (false);
			GameObject.Find ("Instruction Border Panel").SetActive (false);
			instructionCounter = -1;
		}else{
			instructionCounter = 0;
		}
		
		fadeScript.fadeInGame ();

		toggleTouchFields (false);
		determinePrefab (); //Determine which prefab to use for the game
		source.PlayOneShot (introSound, .3f);

		//Instantiate attached dude, set up values as a rider on the seesaw
		attachedDude = Instantiate (dude, new Vector2 (teeter.transform.position.x - startingPointAttachedX, teeter.transform.position.y + startingPointAttachedY), Quaternion.identity).gameObject;
		attachedDude.GetComponent<Bouncer> ().setupAttached (attachedDudeName, teeter.transform);
		teeter.GetComponent<TeeterBase> ().dudeOnSaw = attachedDude;

		//Instantiate falling dude, set up values as the first falling dude on the screen
		fallingDude = Instantiate (dude, new Vector2 (startingPointFallingX, startingPointFallingY), Quaternion.identity).gameObject;
		fallingDude.GetComponent<Bouncer> ().setupFalling (fallingDudeName);

		//Set starting spot for teeter(offset), intro platform position
		float introOffsetX, introOffsetY;
		if (dude == skeleton) {
			offset = .352f;
			introOffsetX = .05f;//0
			introOffsetY = .257f;//.282
		}else if(dude == shark){
			offset = .349f;
			introOffsetX = .07f;//0
			introOffsetY = .06f;//.09
		}else if(dude == ninja){
			offset = .351f;
			introOffsetX = .05f;//.023
			introOffsetY = .195f;//.22
		}else if(dude == ducks){
			offset = .372f;
			introOffsetX = .05f;//0
			introOffsetY = .09f;//.12
		}else{ //new char offset here
			offset = 0;
			introOffsetX = 0;
			introOffsetY = 0;
		}
		introPlatform.transform.position = new Vector2(startingPointFallingX + introOffsetX, startingPointFallingY - introOffsetY);

		if(dude == ducks){ //If duck team is the chosen character, use duck platforms, turn off regular platforms
			dudeArray = new GameObject[duckCount]; //Array to hold all of the duck objects 
			dudeArray [0] = attachedDude;
			dudeArray [1] = fallingDude;
			deadDucks = new Queue ();
			cannonTarL = GameObject.Find ("Left Cannon Target");
			cannonTarR = GameObject.Find ("Right Cannon Target");
			cannonRStartVector = new Vector3 (4.2f, 2.484f, 0);
			cannonLStartVector = new Vector3 (-4.2f, 2.484f, 0);
			speed = 1;
			movingInR = true;
			movingInL = true;
			duckSequenceDone = false;
			GameObject.Find ("Platform Top Left").SetActive (false);
			GameObject.Find ("Platform Top Right").SetActive (false);
			GameObject.Find ("Platform Bottom Left").SetActive (false);
			GameObject.Find ("Platform Bottom Right").SetActive (false);
		}else{ //Prefab is not duck, use regular platforms instead of duck platforms
			GameObject.Find ("Duck Platform Top Left").SetActive (false);
			GameObject.Find ("Duck Platform Top Right").SetActive (false);
			GameObject.Find ("Duck Platform Bottom Left").SetActive (false);
			GameObject.Find ("Duck Platform Bottom Right").SetActive (false);
		}
		cannonL.SetActive (false); //Turn off cannons either because not ducks, or until they should be turned on
		cannonR.SetActive (false);
		BackgroundColorChanger.bgcc.waitingForController = true;
		smScript.resetShowingAd ();
    }

    // Update is called once per frame
    void Update(){
        if (smScript == null)
        {
            smScript = GameObject.FindWithTag("SceneManager").GetComponent<SmScript>();
        }
        if (gameOver)
        {
            if (!soundPlayed)
            {
				source.PlayOneShot(deathSound, .75f);
                soundPlayed = true;
                lives--; //Remove a life from the player
            }
			if (animationDone) { //Once dead animation has been switched to, prepare to reset scene
				Time.timeScale = 0; //Pause the game
				animationDone = false;

				//Check lives, reset level if lives remaining
				if (lives >= 0) {
					if (smScript.reset != 1) {
						//Save game info
						PlayerPrefs.SetInt ("score", score);
						PlayerPrefs.SetInt ("lives", lives);
						PlayerPrefs.Save ();

						//Reset level
						smScript.reset = 1;
						smScript.waitToLoad ();
					}
				} else { //No lives, game over
					if (smScript.reset != 2) {
						toggleTouchFields (true);
						string curPlayer = PlayerPrefs.GetString ("curPlayer");
						smScript.reset = 2;
						smScript.removeKeys (); //Remove values from playerprefs, clean for next game

						displayResult ();
						if (Social.localUser.authenticated) {
							SocialManager.sManager.sendLBScore (score);
						}

						hiScoreText.text = "High Score: " + PlayerPrefs.GetInt ("hallScore0").ToString("N0"); //Set the high score text
						pauseDisplay.gameObject.SetActive(false);
						menuDisplay.gameObject.SetActive (true);
					}
				}
			}
        }
        else //Game is not over
        {
			leftMid.transform.position = new Vector2(teeter.transform.position.x - .272f, teeter.transform.position.y); //Update the left and right transforms, used to determine bounce values on seesaw
			rightMid.transform.position = new Vector2(teeter.transform.position.x + .272f, teeter.transform.position.y);
            if (!gameObject.GetComponent<BalloonMovement>().paused)
            {
				if(!smScript.getShowingAd()){
					if(!pausing){
						Time.timeScale = gameSpeed; //Unpause game
					}
				}
            }
        }

		if(dude == ducks){ //Manage timer and spawning of ducks
			cannonSequence();
		}
		if(intro){ //Hold the falling dude in place until song is played and game begins
			introTime += Time.deltaTime;
			fallingDude.transform.position = new Vector2 (startingPointFallingX, startingPointFallingY);
			fallingDude.GetComponent<Rigidbody2D> ().velocity = new Vector2 (0, 0);
			if(introTime >= 2){ //After 2 seconds, drop the falling dude to start the game
				intro = false;
				introPlatform.SetActive (false); //Turn off the platform holding the falling dude
				fallingDude.GetComponent<Bouncer> ().setAnim (1); //Turn on the prefabs falling animation
			}
		}
		if(cartReady){ //If the cart image has been initiallized
			if(!cartSet){ //If the cart starting position hasnt been set, set the values
				scrollBar.GetComponent<Scrollbar> ().value = offset;
				cartSet = true;
			}
		}else{ //Otherwise try to get the reference to the cart image
			cart = GameObject.Find ("Seesaw Base Image");
			if(cart != null){
				cartReady = true;
			}
		}
		if(exploding){ //Handle moving and cleanup of bomb explosions
			activeExplosions = 0;
			for(int i = 0; i < explodeArray.Length; i++){
				if(explodeArray[i] != null){
					activeExplosions++;
					if(explodeArray[i].transform.position.x != explodeArray[i].GetComponent<ExplodeColliderHandler>().target.transform.position.x){
						explodeArray[i].transform.position = Vector3.MoveTowards (explodeArray[i].transform.position, explodeArray[i].GetComponent<ExplodeColliderHandler>().target.transform.position, Time.deltaTime * explodeSpeed);
					}else{
						if (!explodeArray[i].GetComponent<ExplodeColliderHandler>().finished) { //Disable components after bomb finishes
							explodeArray[i].GetComponent<BoxCollider2D> ().enabled = false;
							explodeArray[i].GetComponentInChildren<ParticleSystem> ().Stop ();
							explodeArray[i].GetComponent<SpriteRenderer> ().enabled = false;
							explodeArray[i].GetComponent<ExplodeColliderHandler>().finished = true;
						}else{
							//Wait for both explosions to finish before recycling explosion PS
							if(explodeArray[i].GetComponent<ExplodeColliderHandler>().explodePair != null){
								if(!explodeArray[i].GetComponent<ExplodeColliderHandler>().waitingOnCoroutine){
									if (explodeArray [i].GetComponent<ExplodeColliderHandler> ().finished && explodeArray [i].GetComponent<ExplodeColliderHandler> ().explodePair.GetComponent<ExplodeColliderHandler> ().finished) {
										if (numBalPopPreExplode < 10 && !explodeArray [i].GetComponent<ExplodeColliderHandler> ().explodePair.GetComponent<ExplodeColliderHandler> ().waitingOnCoroutine) {
											GetComponent<BalloonMovement> ().makeSureRowHasCleared (explodeArray [i].GetComponent<ExplodeColliderHandler> ().row);
										}
										explodeArray [i].GetComponent<ExplodeColliderHandler> ().explodePair.GetComponent<ExplodeColliderHandler> ().remove = true;
										StartCoroutine(disableExplosionCo (i));
										explodeArray [i].GetComponent<ExplodeColliderHandler> ().waitingOnCoroutine = true;
									}
								}else{
									activeExplosions--;
								}
							}else{
								if(explodeArray [i].GetComponent<ExplodeColliderHandler> ().remove){
									StartCoroutine(disableExplosionCo (i));
								}
							}
						}
					}
				}
			}

			if(activeExplosions == 0){
				exploding = false;
			}
		}
		
		if(smScript.showingAd){ //Stop music playing before an ad
			if(source.isPlaying){
				source.Stop ();
			}
		}

		if(bgmBonusPause){ //If row clear song needs to pause bgmusic
			if(bonusAudioSource != null && !bonusAudioSource.isPlaying){
				bgmBonusPause = false;
				bonusAudioSource = null;
			}
		}
		if(bgmGreenPause){ //If green powerUp song needs to pause bgmusic
			if(greenAudioSource != null && !greenAudioSource.isPlaying){
				if (!bgmBonusPause) {
					bgmGreenPause = false;
					greenAudioSource.time = 0f;
					greenAudioSource = null;
				}
			}
		}
    }

	//Add the points to the score
    public void updateScore(int points)
    {
		if (points != 0) {
			if (points < 100) { //Balloon pop
				score += (points + (4 * speedMultiplier));
				scoreInterval += (points + (4 * speedMultiplier));
			}else{ //Row clear or powerup points
				score += points;
				scoreInterval += points;
			}
			scoreText.text = "Score: " + score.ToString("N0");
			if(score >= hiScore){
				hiScoreText.text = "High Score: " + score.ToString ("N0");
			}
			if (speedLevel < 5) { //Only 5 speedLevels. Cap the speed, game gets wierd at high values
				if (scoreInterval >= speedBreakPoint) { //Increase speed of the game as the player scores more points
					gameSpeed += .1f;
					Time.timeScale = gameSpeed;
					speedBreakPoint = getNextBreakPoint (speedBreakPoint);
					speedLevel++;
					if (speedLevel == 6) {
						speedMultiplier = 5;
					}else{
						speedMultiplier = speedLevel;
					}
					source.PlayOneShot (speedUpSound, .75f);
					BackgroundColorChanger.bgcc.changeBackgroundColor ();

					//Setup and display the floating text for a speed increase
					if (!gameOver) {
						speedFText = PoolingManager.pooler.getPooledObject (7);
						speedFText.SetActive (true);
						speedLevelText.color = speedColor;
						if (speedLevel == 5) {
							speedLevelText.text = "MAX: " + (speedBreakPoint - scoreInterval).ToString ("N0");
							speedFText.GetComponent<FloatingText> ().createSpeedText ("Speed Level MAX");
						} else {
							speedLevelText.text = "Speed " + (speedLevel + 1).ToString () + ": " + (speedBreakPoint - scoreInterval).ToString ("N0");
							speedFText.GetComponent<FloatingText> ().createSpeedText ("Speed Level " + speedLevel.ToString ());
						}
						delayedResetFText (speedFText);
					}
				}else{ //If score doesn't trigger a speed up, update the points remaining
					if ((speedBreakPoint - scoreInterval) <= scoreHalf) {
						speedLevelText.color = speedWarningColor;
					}else{
						speedLevelText.color = speedColor;
					}
					speedLevelText.text = "Speed " + (speedLevel + 1).ToString () + ": " + (speedBreakPoint - scoreInterval).ToString ("N0");
				}
			}else{ //Max speed level condition
				if(speedBreakPoint == 20000){
					speedBreakPoint = 6000; //Adjust here if color changes too fast
					scoreInterval = 0;
					scoreHalf = 3000;
					speedLevelText.text = "MAX: 0";
				}

				//Change background color, setup and display the floating text(non level up version)
				if (scoreInterval >= speedBreakPoint) {
					source.PlayOneShot (speedUpSound, .75f);
					BackgroundColorChanger.bgcc.changeBackgroundColor ();
					scoreInterval = 0;
					if (!gameOver) {
						speedFText = PoolingManager.pooler.getPooledObject (7);
						speedFText.SetActive (true);
						speedFText.GetComponent<FloatingText> ().createSpeedText (speedFText.GetComponent<FloatingText> ().getColorChangeText());
						delayedResetFText (speedFText);
					}
				}else{ //If score doesn't trigger a color change, update the points remaining
					if ((speedBreakPoint - scoreInterval) <= scoreHalf) {
						speedLevelText.color = speedWarningColor;
					}else{
						speedLevelText.color = speedColor;
					}
					speedLevelText.text = "MAX: " + (speedBreakPoint - scoreInterval).ToString("N0");
				}
			}
		}
    }

	//Increment the point total for the next speed level increase
	private int getNextBreakPoint(int curPoint){
		if(curPoint == 1000){
			scoreHalf = 2000;
			return 5000;
		}else if(curPoint == 5000){ //Change these?
			scoreHalf = 2500;
			return 10000;
		}else if(curPoint == 10000){
			scoreHalf = 2500;
			return 15000;
		}else{
			scoreHalf = 2500;
			return 20000;
		}
	}
//	private int getNextBreakPoint(int curPoint){ //Test Method
//		if(curPoint == 1000){
//			return 1100;
//		}else if(curPoint == 1100){ //Change these?
//			return 1200;
//		}else if(curPoint == 1200){
//			return 1300;
//		}else{
//			return 1400;
//		}
//	}

	//Menu button onClick method
	public void menuButtonOnClick(){
		source.PlayOneShot(selectSound, .75f);
		PlayerPrefs.SetInt ("score", score);
		PlayerPrefs.SetInt ("lives", lives);
		smScript.menuButtonOnClick2 ();
	}
	public void menuButtonNoSaveOnClick(){
		source2.PlayOneShot (selectSound, .75f);
		smScript.reset = 2;
		smScript.removeKeys (); //Remove values from playerprefs, clean for next game
		smScript.menuButtonOnClick2 ();
	}

	//Pause button onClick method
	public void pauseButtonOnClick(){
		if(pausing){
			pausing = false;
			Time.timeScale = gameSpeed;
			playPauseSound (false); //Play the pause/unpause sound
			canAudioPause = false;
			pausingControls (false);
			pauseMenuDisplay.SetActive (false);
			pausePanelViewPort.SetActive (false);
			pauseTouchField.interactable = true;
		}else{
			if (!gameOver) {
				pausing = true;
				Time.timeScale = 0;
				playPauseSound (true); //Play the pause/unpause sound
				canAudioPause = true;
				pausingControls (true);
				pauseMenuDisplay.SetActive (true);
				resumeTouchField.interactable = true;
				pauseMenuTouchField.interactable = true;
				pausePanelViewPort.SetActive (true);
			}
		}
	}

	//OnClick method for yes button in confirm panel >mainmenu button >pause panel
	public void confirmPanelCancelButtonOnClick(){
		source2.PlayOneShot(backSound, .75f);
		confirmPanel.SetActive (false);
	}

	//OnClick method for main menu button in the pause panel
	public void pausePanelMenuButtonOnClick(){
		source2.PlayOneShot(warningSound, .75f);
		confirmPanel.SetActive (true);
		confirmPanelYesButton.interactable = true;
		confirmPanelYesButtonTouchField.interactable = true;
		confirmPanelNoButton.interactable = true;
		confirmPanelNoButtonTouchField.interactable = true;
	}

	//Check to see if the score should be in the hall of fame
	int checkHallScores(){
		int hallPlace = -1;
		for(int i = 9; i >= 0; i--){
			if(score >= PlayerPrefs.GetInt("hallScore" + (i).ToString())){
				hallPlace = i;
			}
		}
		return hallPlace;
	}

	//Update the hall of fame scores by shifting scores down
	void updateHallScores(int pos){
		for(int i = 9; i > pos; i--){
			PlayerPrefs.SetInt ("hallScore" + i.ToString (), PlayerPrefs.GetInt ("hallScore" + (i - 1).ToString()));
			PlayerPrefs.SetString ("hallName" + i.ToString (), PlayerPrefs.GetString ("hallName" + (i - 1)).ToString());
		}
	}

	//Determine which prefab will be used, decided by the players chosen character
	void determinePrefab(){
		if(PlayerPrefs.GetString("curChar").Equals(char1)){ //Bones Mcgee prefab was selected
			attachedDudeName = "Skeleton1";
			fallingDudeName = "Skeleton2";
			dude = skeleton;
			startingPointAttachedX = skeleAttachedX;
			startingPointAttachedY = skeleAttachedY;
			startingPointFallingX = skeleFallingX;
			startingPointFallingY = skeleFallingY;
		}else if(PlayerPrefs.GetString("curChar").Equals(char2)){ //Bruce prefab was selected
			attachedDudeName = "Shark1";
			fallingDudeName = "Shark2";
			dude = shark;
			startingPointAttachedX = sharkAttachedX;
			startingPointAttachedY = sharkAttachedY;
			startingPointFallingX = sharkFallingX;
			startingPointFallingY = sharkFallingY;
		}else if(PlayerPrefs.GetString("curChar").Equals(char3)){ //Ninja prefab was selected
			attachedDudeName = "Ninja1";
			fallingDudeName = "Ninja2";
			dude = ninja;
			startingPointAttachedX = ninjaAttachedX;
			startingPointAttachedY = ninjaAttachedY;
			startingPointFallingX = ninjaFallingX;
			startingPointFallingY = ninjaFallingY;
		}else if(PlayerPrefs.GetString("curChar").Equals(char4)){ //Duck Team prefab was selected 
			attachedDudeName = "Duck1";
			fallingDudeName = "Duck2";
			dude = ducks;
			startingPointAttachedX = duckAttachedX;
			startingPointAttachedY = duckAttachedY;
			startingPointFallingX = duckFallingX;
			startingPointFallingY = duckFallingY;
		}else{ //new char info here
		}
	}

	//Setup correct spawning position, begin shooting the ducks
	private void spawnDuck(){
		if(startX == 0){ //Initial spawn location
			startingPointFallingX = 3.27f;
			startingPointFallingY = 2.55f;
			startX = startingPointFallingX;
		}else{ //Toggle spawn location
			if(startX < 0){
				startX = startingPointFallingX;
			}else{
				startX = -startingPointFallingX;
			}
		}
		if (ducksSpawned <= duckCount) { //If the number of ducks already spawned is less than the number that should spawn, spawn the ducks
			Invoke ("shootDuck2", 1f); //Shoot first duck after short delay
			Invoke ("shootDuck2", 1.5f); //Shoot second duck after short delay
			Invoke ("shootDuck2", 2.2f); //Shoot third duck after short delay
			if(ducksSpawned == 3){
				Invoke ("shootDuck2", 2.8f); //Shoot extra duck after short delay
			}else{
			}
		}
	}

	//Instantiate and apply force to a new duck
	void shootDuck(int forceType){
		Vector2 forceVector;
		if(forceType == 1){
			forceVector = new Vector2 ((1f * cannonSide), 1f);
		}else if(forceType == 2){
			forceVector = new Vector2 ((2f * cannonSide), 1.5f);
		}else if(forceType == 3){
			forceVector = new Vector2 ((2.75f * cannonSide), 2.0f);
		}else{
			forceVector = new Vector2 ((3.75f * cannonSide), 2.5f);
		}
		GameObject barrel;
		if(cannonSide == 1){
			barrel = leftBarrel;
			barrel.transform.position = new Vector3 (barrel.transform.position.x + .04f, barrel.transform.position.y, 0);
		}else{
			barrel = rightBarrel;
			barrel.transform.position = new Vector3 (barrel.transform.position.x - .04f, barrel.transform.position.y, 0);
		}
		dudeArray [ducksSpawned-1] = Instantiate (dude, barrel.transform.position, Quaternion.identity).gameObject;
		dudeArray [ducksSpawned-1].GetComponent<Bouncer> ().setupFalling ("Duck" + ducksSpawned.ToString ());
		dudeArray [ducksSpawned - 1].GetComponent<Rigidbody2D> ().velocity = new Vector2 (0, 0);
		dudeArray [ducksSpawned - 1].GetComponent<Rigidbody2D> ().AddForce (forceVector, ForceMode2D.Impulse);
		if(fallingDude.GetComponent<RubberDuckMovement>().inBigMode()){
			StartCoroutine (toggleBigModeNewDucks(ducksSpawned - 1));
		}
		ducksSpawned++; //Increment the number of ducks spawned

		source.PlayOneShot(cannonSound, .85f);
		recoilOut = true;
		cannonRecoil = true;

		//Create and destroy the cannon smoke particle effect
		ParticleSystem ps;
		if (cannonSide == 1) {
			ps = Instantiate (cannonSmokeL, leftBarrel.transform.position, Quaternion.identity);
		}else{
			ps = Instantiate (cannonSmokeR, rightBarrel.transform.position, Quaternion.identity);
		}
		Destroy (ps.gameObject, .7f);
	}

	//Invoke helper method to apply correct force to the duck
	void shootDuck2(){
		//Set up the duck starting position for each cannon shot, moves with the barrel angle
		GameObject cannon, barrel, flat, low, mid, high;
		if(cannonSide == 1){
			cannon = cannonL;
			barrel = leftBarrel;
			flat = GameObject.Find ("Flat Barrel TargetL");
			low = GameObject.Find ("Low Barrel TargetL");
			mid = GameObject.Find ("Mid Barrel TargetL");
			high = GameObject.Find ("Flat Barrel TargetL");
		}else{
			cannon = cannonR;
			barrel = rightBarrel;
			flat = GameObject.Find ("Flat Barrel TargetR");
			low = GameObject.Find ("Low Barrel TargetR");
			mid = GameObject.Find ("Mid Barrel TargetR");
			high = GameObject.Find ("High Barrel TargetR");
		}

		//Display the different angles of the cannon barrel, prepare to shoot a duck
		if (ducksSpawned == 3 || ducksSpawned == 7 || ducksSpawned == 10) { //First duck, cannon shooting flat position
			cannon.GetComponent<Animator> ().SetInteger ("State", 1);
			barrel.transform.position = flat.transform.position;
			StartCoroutine (stall(1));
		} else if (ducksSpawned == 4 || ducksSpawned == 8 || ducksSpawned == 11) { //Second duck, 15deg cannon shooting position
			cannon.GetComponent<Animator> ().SetInteger ("State", 2);
			barrel.transform.position = low.transform.position;
			StartCoroutine (stall(2));
		} else if (ducksSpawned == 5 || ducksSpawned == 9 || ducksSpawned == 12) { //Third duck, 30deg cannon shooting position
			cannon.GetComponent<Animator> ().SetInteger ("State", 3);
			barrel.transform.position = mid.transform.position;
			StartCoroutine (stall(3));
		} else { //duckSpawned == 6 //Last duck from first round, 45deg cannon shooting position
			cannon.GetComponent<Animator> ().SetInteger ("State", 4);
			barrel.transform.position = high.transform.position;
			StartCoroutine (stall(4));
		}
	}

	//Method to control cannon movement and duck spawning sequence
	void cannonSequence(){
		if(ducksSpawned <= duckCount){
			duckTimer += Time.deltaTime;
			if (duckTimer >= 2.1f) {
				if(cannonSide == 1){ //Change the multiplier to affect the forces applied to the ducks
					cannonSide = -1;
				}else{
					cannonSide = 1;
				}
				spawnDuck (); //Spawn the next duck
				duckTimer = -3; //Reset the timer
			}
			if(duckTimer <= -2.9f){
				if (!introSoundPlayed) {
					cannonTarR.GetComponent<CannonAudioController> ().getSource ().Play ();
					introSoundPlayed = true;
				}
				if (cannonSide == -1) {
					cannonR.SetActive (true);
					cannonR.transform.position = new Vector3 (4.165f, 2.484f, 0);
					movingInR = true;
				}else{
					cannonL.SetActive (true);
					cannonL.transform.position = new Vector3 (-4.165f, 2.484f, 0);
					movingInL = true;
				}
			}
			if(cannonSide == -1){ //Right side cannon movement
				if(movingInR){ //Move in
					if(cannonR.transform.position.x != cannonTarR.transform.position.x && !cannonRecoil){ //Cannon reached right inside target
						cannonR.transform.position = Vector3.MoveTowards(cannonR.transform.position, cannonTarR.transform.position, speed * Time.deltaTime);
					}else{
						if(duckTimer >= 0){ //After ducks have been shot
							movingInR = false; //Set to move out
							cannonR.GetComponent<SpriteRenderer> ().sortingOrder = -6;
							GameObject.Find ("Right Cannon Cover(White)").GetComponent<SpriteRenderer> ().sortingOrder = -5;
							cannonRecoil = false;
						}else{ //Recoil the cannon after a duck is shot out
							if(cannonRecoil){
								if (recoilOut) {
									if (cannonR.transform.position.x != cannonRecoilOutR.transform.position.x) { //Cannon reached right inside target
										cannonR.transform.position = Vector3.MoveTowards (cannonR.transform.position, cannonRecoilOutR.transform.position, speedRecoilOut * Time.deltaTime);
									} else {
										recoilOut = false;
									}
								}else{
									if (cannonR.transform.position.x != cannonTarR.transform.position.x) { //Cannon reached right inside target
										cannonR.transform.position = Vector3.MoveTowards (cannonR.transform.position, cannonTarR.transform.position, speedRecoilIn * Time.deltaTime);
									} else {
										cannonRecoil = false;
									}
								}
							}
						}
					}
				}else{ //Move out
					if(cannonR.transform.position.x != cannonRStartVector.x){ //Cannon reached right outside target
						cannonR.transform.position = Vector3.MoveTowards(cannonR.transform.position, cannonRStartVector, speed * Time.deltaTime);
					}else{
						cannonR.SetActive(false); //Turn off cannon sprite
						cannonRecoil = false;
						recoilOut = false;
					}
				}
			}else{ //Left side cannon movement
				if(movingInL){ //Move in
					if (cannonL.transform.position.x != cannonTarL.transform.position.x && !cannonRecoil) { //Cannon reached right inside target
						cannonL.transform.position = Vector3.MoveTowards (cannonL.transform.position, cannonTarL.transform.position, speed * Time.deltaTime);
					}else{
						if(duckTimer >= 0){ //After ducks have been shot
							movingInL = false; //Set to move out
							cannonL.GetComponent<SpriteRenderer> ().sortingOrder = -6;
							GameObject.Find ("Left Cannon Cover(White)").GetComponent<SpriteRenderer> ().sortingOrder = -5;
							cannonRecoil = false;
						}else{ //Recoil the cannon after a duck is shot out
							if(cannonRecoil){
								if (recoilOut) {
									if (cannonL.transform.position.x != cannonRecoilOutL.transform.position.x) { //Cannon reached right inside target
										cannonL.transform.position = Vector3.MoveTowards (cannonL.transform.position, cannonRecoilOutL.transform.position, speedRecoilOut * Time.deltaTime);
									} else {
										recoilOut = false;
									}
								}else{
									if (cannonL.transform.position.x != cannonTarL.transform.position.x) { //Cannon reached right inside target
										cannonL.transform.position = Vector3.MoveTowards (cannonL.transform.position, cannonTarL.transform.position, speedRecoilIn * Time.deltaTime);
									} else {
										cannonRecoil = false;
									}
								}
							}
						}
					}
				}else{ //Move out
					if (cannonL.transform.position.x != cannonLStartVector.x) { //Cannon reached right outside target
						cannonL.transform.position = Vector3.MoveTowards (cannonL.transform.position, cannonLStartVector, speed * Time.deltaTime);
					} else {
						cannonL.SetActive(false); //Turn off cannon sprite
					}
				}
			}
		}else{
			if (!duckSequenceDone) {
				cannonR.GetComponent<SpriteRenderer> ().sortingOrder = -6;
				if (cannonR.transform.position.x != cannonRStartVector.x) {
					cannonR.transform.position = Vector3.MoveTowards (cannonR.transform.position, cannonRStartVector, speed * Time.deltaTime);
				} else {
					cannonR.SetActive (false);
					duckSequenceDone = true;
					introSoundPlayed = false; 
				}
			}
		}
	}

	//Reset the cannon animation
	private void cannonReset(){
		if(cannonSide == 1){
			cannonL.GetComponent<Animator> ().SetInteger ("State", 1);
		}else{
			cannonR.GetComponent<Animator> ().SetInteger ("State", 1);
		}
	}

	//Delay before shooting the next duck from the cannon
	IEnumerator stall(int fort){
		yield return new WaitForSeconds(.2f);
		shootDuck (fort);
	}

	//Inform the scenemanager to switch from game scene to menu scene
	private void loadMenuAfterDelay(){
		smScript.menuButtonOnClick2 ();
	}

	//Update the image shown on the cart 
	public void updateCart(float value){
		if(scrollBar.GetComponent<Scrollbar>().value != .2406f && scrollBar.GetComponent<Scrollbar>().value != .745f){
			cart.GetComponent<SeesawCartWheels> ().rollWheels (value); //Change the image of the cart
		}
		if (instructionCounter != -1) {
			if (instructionCounter < 5) { //Condition to turn off the instructions after the player moves the cart
				instructionCounter++;
			} else if (instructionCounter == 5) {
				GameObject.Find ("Instruction Text").SetActive (false);
				GameObject.Find ("Instruction Border Panel").SetActive (false);
				instructionCounter++;
			}
		}
	}


	//Various getters
	public Transform getDude1(){ //Return the chosen prefab
		return dude;
	}
	public bool isCartReady(){ //Return whether the cart is ready or not
		return cartReady;
	}
	public bool checkIntro(){ //Return whether the game is in intro time or not
		return intro;
	}
	public AudioSource getSource(){
		return source;
	}
	public GameObject getTeeter(){
		return teeter;
	}
	public bool isExploding(){
		return exploding;
	}

	//Setup the explosion effect when the red powerUp explosion balloon is triggered
	public void explode(float x, float y, int index, int row){
		int bombIndex = GetComponent<BalloonMovement> ().bombIndex;
		spaceOutNextBomb(index, bombIndex);
		GetComponent<BalloonMovement> ().bombIndex = bombIndex;

		//Get the bomb PS from the pooler and place them
		bEffectL = PoolingManager.pooler.getPooledObject(4);
		initializeExplode(1, x, y);
		bEffectR = PoolingManager.pooler.getPooledObject (4);
		initializeExplode(2, x, y);
		bEffectL.tag = "Explosion";
		bEffectR.tag = "Explosion";
		bEffectL.GetComponent<SpriteRenderer> ().flipX = true;
		exploding = true;
		bEffectL.GetComponent<ExplodeColliderHandler> ().row = row;
		bEffectR.GetComponent<ExplodeColliderHandler> ().row = row;

		//Get the bomb floating text from the pooler and set it up
		bombFTexts[bombFTextsIndex] = PoolingManager.pooler.getPooledObject(7);
		bombFTexts [bombFTextsIndex].SetActive (true);
		bombFTexts[bombFTextsIndex].GetComponent<FloatingText>().createBombText (x, y);
		bombFTextsIndex++;
		if(bombFTextsIndex >= 5){
			bombFTextsIndex = 0;
		}
		Invoke ("resetBombFText", 1.4f);

		bEffectL.GetComponent<ExplodeColliderHandler> ().playSound ();
		numBalPopPreExplode = PlayerPrefs.GetInt ("row" + row.ToString () + "Pop");

		//Place the active explode PS into the active array in case there is a chain reaction and need to control more than one set of explosions
		explodeArray [explodeIndex] = bEffectL;
		int lIndex = explodeIndex;
		updateExplodeIndex ();
		explodeArray [explodeIndex] = bEffectR;
		int rIndex = explodeIndex;
		updateExplodeIndex ();
		explodeArray [lIndex].GetComponent<ExplodeColliderHandler> ().explodePair = explodeArray [rIndex];
		explodeArray [rIndex].GetComponent<ExplodeColliderHandler> ().explodePair = explodeArray [lIndex];
		explodeArray [lIndex].GetComponent<ExplodeColliderHandler> ().index = lIndex;
		explodeArray [rIndex].GetComponent<ExplodeColliderHandler> ().index = rIndex;
	}

	//Try to place the next bomb away from the current explosion so it wont start an endless chain reaction
	private void spaceOutNextBomb(int index, int bombIndex){
		bool done = false;
		bool same, less, greater, tooFarLeft, tooFarRight;
		if(index == bombIndex){
			do{
				same = false;
				less = false;
				greater = false;
				tooFarLeft = false;
				tooFarRight = false;

				bombIndex = Random.Range(1, 10);
				if(bombIndex == index) {same = true;}
				if(bombIndex == (index - 1)) {less = true;}
				if(bombIndex == (index + 1)) {greater = true;}
				if(bombIndex <= (index - 4)) {tooFarLeft = true;}
				if(bombIndex >= (index + 4)) {tooFarRight = true;}
				if(!same){
					if(!less){
						if(!greater){
							if(!tooFarLeft){
								if(!tooFarRight){
									done = true;
				}}}}}
			}while(!done);
		}
	}

	//Setup the explosion PS depending on the direction it will travel
	private void initializeExplode(int effect, float xPos, float yPos){
		if(effect == 1){
			bEffectL.transform.SetParent (null);
			bEffectL.transform.position = new Vector3 (xPos, yPos, 0);
			bEffectL.GetComponent<ExplodeColliderHandler>().target = bEffectL.transform.GetChild (0).gameObject;
			bEffectL.GetComponent<ExplodeColliderHandler> ().target.transform.SetParent (null);
			bEffectL.GetComponent<ExplodeColliderHandler> ().target.transform.position = new Vector3 (-3.82f, yPos, 0);
			bEffectL.SetActive (true);
		}else if(effect == 2){
			bEffectR.transform.SetParent (null);
			bEffectR.transform.position = new Vector3 (xPos, yPos, 0);
			bEffectR.GetComponent<ExplodeColliderHandler>().target = bEffectR.transform.GetChild (0).gameObject;
			bEffectR.GetComponent<ExplodeColliderHandler> ().target.transform.SetParent (null);
			bEffectR.GetComponent<ExplodeColliderHandler> ().target.transform.position = new Vector3 (3.82f, yPos, 0);
			bEffectR.SetActive (true);
		}
	}

	//Coroutine to disable explosion effect after a delay
	IEnumerator disableExplosionCo (int index){
		yield return new WaitForSecondsRealtime (4.0f);
		disableExplosion (index);
	}

	//Stops any active coroutines before the scene changes
	public void stopExplodeCoroutines(){
		bool stop = false;
		for(int i = 0; i < explodeArray.Length; i++){
			if(explodeArray[i] != null){
				stop = true;
				break;
			}
		}
		if (stop) {
			StopAllCoroutines ();
			for (int i = 0; i < explodeArray.Length; i++) {
				if (explodeArray [i] != null) {
					disableExplosion (i);
				}
			}
		}
	}

	//Resets explosion prefab, parents to the pooler object for reuse, disables until needed
	private void disableExplosion(int index){
		ExplodeColliderHandler exCol = explodeArray [index].GetComponent<ExplodeColliderHandler> ();
		explodeArray[index].GetComponent<SpriteRenderer> ().flipX = false;
		explodeArray[index].GetComponent<SpriteRenderer> ().enabled = true;
		explodeArray[index].GetComponent<BoxCollider2D> ().enabled = true;
		explodeArray[index].GetComponentInChildren<ParticleSystem> ().Play ();
		exCol.target.transform.SetParent (explodeArray[index].transform);
		exCol.target.transform.SetAsFirstSibling ();
		exCol.target = null;
		exCol.explodePair = null;
		exCol.remove = false;
		exCol.finished = false;
		exCol.waitingOnCoroutine = false;
		exCol.row = 1;
		exCol.soundPlayed = false;
		explodeArray[index].transform.SetParent (PoolingManager.pooler.gameObject.transform);
		explodeArray[index].SetActive (false);
		explodeArray[index] = null;
	}

	private void updateExplodeIndex(){
		explodeIndex++;
		if(explodeIndex >= 20){
			explodeIndex = 0;
		}
	}

	//Toggle big mode on and off for all of the active dudes
	public void toggleBigMode(bool bigModeToggleBool){
		if(bigModeToggleBool){
			if (dude == ducks) {
				for (int i = 0; i < dudeArray.Length; i++) {
					if (dudeArray [i] != null) {
						dudeArray [i].GetComponent<Bouncer> ().toggleBigMode (bigModeToggleBool);
					}
				}
			} else {
				fallingDude.GetComponent<Bouncer> ().toggleBigMode (bigModeToggleBool);
				attachedDude.GetComponent<Bouncer> ().toggleBigMode (bigModeToggleBool);
			}
			source.PlayOneShot (growSound, .75f);
			bigModeToggleBool = false;
		}else{
			if (dude == ducks) {
				for (int i = 0; i < dudeArray.Length; i++) {
					if (dudeArray [i] != null) {
						dudeArray [i].GetComponent<Bouncer> ().toggleBigMode (bigModeToggleBool);
					}
				}
			} else {
				fallingDude.GetComponent<Bouncer> ().toggleBigMode (bigModeToggleBool);
				attachedDude.GetComponent<Bouncer> ().toggleBigMode (bigModeToggleBool);
			}
			bigModeToggleBool = true;
		}
	}

	//Returns powerUp index
	public int getPowerUpIndex(){
		return powerUpIndex;
	}

	//Increments powerUp index or resets to 0
	public void updatePowerUpIndex(){
		if(powerUpIndex >= 99){
			powerUpIndex = 0;
		}else{
			powerUpIndex++;
		}
	}

	//Flags when text has been displayed to prevent duplicates if multiple collisions with the ground are detected
	public void updateDeathTextDisplayed(){
		deathTextDisplayed = true;
	}

	//Return whether the text has been displayed or not
	public bool checkDeathTextDisplay(){
		return deathTextDisplayed;
	}

	//Coroutine to toggle big mode after a short delay
	IEnumerator toggleBigModeNewDucks(int index){
		yield return new WaitForSeconds(.2f);
		dudeArray [index].GetComponent<Bouncer> ().toggleBigMode (true);
	}

	//Play the audio clip when new game button is pressed
	public void playNewGameSound(){
		source.PlayOneShot(newGameSound, .75f);
	}

	//Return the value of current gamespeed
	public float getGameSpeed(){
		return this.gameSpeed;
	}

	//Reset the speed up floating text
	private void resetText(){
		PoolingManager.pooler.resetFtext (speedFText);
	}

	//Recycle the bomb floating text back into the pool of reusable objects
	private void resetBombFText(){
		for(int i = 0; i < bombFTexts.Length; i++){
			if(bombFTexts[i] != null){
				PoolingManager.pooler.resetFtext (bombFTexts [i]);
				bombFTexts [i] = null;
				break;
			}
		}
	}

	//Show the appropriate powerUp missed effect, prep recycling into pool
	public void displayPowerMissEffect(Vector3 location, int type){
		if (type == 1) {
			missEffects [missEffectsIndex] = PoolingManager.pooler.getPooledObject(8).GetComponent<ParticleSystem>();
		} else if (type == 2) {
			missEffects [missEffectsIndex] = PoolingManager.pooler.getPooledObject(9).GetComponent<ParticleSystem>();
		}else if(type == 3){
			missEffects [missEffectsIndex] = PoolingManager.pooler.getPooledObject(10).GetComponent<ParticleSystem>();
		}else{
			missEffects [missEffectsIndex] = PoolingManager.pooler.getPooledObject(8).GetComponent<ParticleSystem>();
		}
		missEffects [missEffectsIndex].gameObject.SetActive (true);
		missEffects [missEffectsIndex].transform.position = location;
		missEffects [missEffectsIndex].Play ();
		missEffectsIndex++;
		if(missEffectsIndex >= 10){
			missEffectsIndex = 0;
		}
		Invoke("resetPowerUpMissEffect", .9f);
	}

	//Show the appropriate powerUp caught effect, prep recycling into pool
	public void displayPowerCaughtEffect(Vector3 location, int type){
		if (type == 1) {
			caughtEffects [caughtEffectsIndex] = PoolingManager.pooler.getPooledObject(11).GetComponent<ParticleSystem>();
		} else if (type == 2) {
			caughtEffects [caughtEffectsIndex] = PoolingManager.pooler.getPooledObject(12).GetComponent<ParticleSystem>();
		}else if(type == 3){
			caughtEffects [caughtEffectsIndex] = PoolingManager.pooler.getPooledObject(13).GetComponent<ParticleSystem>();
		}else{
			caughtEffects [caughtEffectsIndex] = PoolingManager.pooler.getPooledObject(11).GetComponent<ParticleSystem>();
		}
		caughtEffects [caughtEffectsIndex].gameObject.SetActive (true);
		caughtEffects [caughtEffectsIndex].transform.position = location;
		caughtEffects [caughtEffectsIndex].Play ();
		caughtEffectsIndex++;
		if(caughtEffectsIndex >= 10){
			caughtEffectsIndex = 0;
		}
		Invoke("resetPowerUpCaughtEffect", 1.1f);
	}

	//Recycle the powerUp missed effect back into the pool of reusable objects
	private void resetPowerUpMissEffect(){
		for(int i = 0; i < missEffects.Length; i++){
			if(missEffects[i] != null){
				PoolingManager.pooler.resetPopEffects (missEffects [i].gameObject);
				missEffects [i] = null;
				break;
			}
		}
	}

	//Recycle the powerUp caught effect back into the pool of reusable objects
	private void resetPowerUpCaughtEffect(){
		for(int i = 0; i < caughtEffects.Length; i++){
			if(caughtEffects[i] != null){
				PoolingManager.pooler.resetPopEffects (caughtEffects [i].gameObject);
				caughtEffects [i] = null;
				break;
			}
		}
	}

	//Recycle the floating text back into the pool of reusable objects
	public void delayedResetFText(GameObject obj){
		StartCoroutine (resetPowerUpFText (obj));
	}
	IEnumerator resetPowerUpFText(GameObject obj){
		yield return new WaitForSecondsRealtime (1.4f);
		PoolingManager.pooler.resetFtext (obj);
	}

	//Recycle the balloon pop PS back into the pool of reusable objects
	public void delayedResetPopEffect(GameObject obj){
		StartCoroutine (resetPopEffect (obj));
	}
	IEnumerator resetPopEffect(GameObject obj){
		yield return new WaitForSeconds (.3f);
		PoolingManager.pooler.resetPopEffects (obj);
	}

	//Restrict the seesaw movement when green power up is active and row is cleared
	public void pausingControls(bool shouldPause){
		if(shouldPause){
			controlScroller.horizontal = false;
		}else{
			controlScroller.horizontal = true;
		}
	}

	//Play the pause/unpause sound when pause button is pressed
	private void playPauseSound(bool pause){
		if(pause){
			source2.PlayOneShot (pauseSound, .25f);
		}else{
			source2.PlayOneShot (unPauseSound, .25f);
		}
	}

	//Set variables when pausing the background game music
	public void pauseBGMusic(string name, AudioSource source){
		if(name == "Bonus"){
			bgmBonusPause = true;
			bonusAudioSource = source;
		}else{
			bgmGreenPause = true;
			greenAudioSource = source;
		}
	}

	//Toggle between the button areas for the pause/menu display
	private void toggleTouchFields(bool showMenu){
		if(showMenu){
			pauseTouchField.gameObject.SetActive(false);
			menuTouchField.gameObject.SetActive (true);
		}else{
			pauseTouchField.gameObject.SetActive(true);
			menuTouchField.gameObject.SetActive (false);
		}
	}

	//Display the game over panel
	private void displayResult(){
		gameOverDisplay.SetActive (true);
		GameObject newPersonalText = GameObject.Find ("New Personal Text");
		GameObject newHallText = GameObject.Find ("New Hall Text");
		GameObject scoreText = GameObject.Find ("Game Score Text");
		GameObject personalText = GameObject.Find ("Personal Score Text");
		GameObject hallText = GameObject.Find ("Hall Rank Text");
		string curPlayer = PlayerPrefs.GetString ("curPlayer");

		//Check game score against previous high scores
		bool personal = false;
		bool hall = false;
		bool hallFirst = false;
		scoreText.GetComponent<Text>().text = score.ToString ("N0");

		//Check if score is new personal high score, set up texts
		int playerHiScore = PlayerPrefs.GetInt ("player" + PlayerPrefs.GetString (curPlayer) + "Score");
		if (score > playerHiScore) { //If score beats players previous high score
			PlayerPrefs.SetInt ("player" + PlayerPrefs.GetString (curPlayer) + "Score", score); //Update the players high score value
			personal = true;
			personalText.GetComponent<Text>().text = score.ToString ("N0");
		}else{
			personalText.GetComponent<Text> ().text = playerHiScore.ToString ();
		}

		//Check whether score should be in the HOF, set up texts
		int hallPlace = checkHallScores ();
		if (score > 0 && hallPlace != -1) { //If score makes the highest score list
			hallText.GetComponent<Text>().text = "#" + (hallPlace + 1).ToString();
			hall = true;
			updateHallScores (hallPlace);
			PlayerPrefs.SetInt ("hallScore" + hallPlace.ToString (), score);
			PlayerPrefs.SetString ("hallName" + hallPlace.ToString (), curPlayer);
			if(hallPlace == 0){
				hallFirst = true;
			}
		}else{
			hallText.GetComponent<Text>().text = "--";
		}

		//Turn on new texts if player gets new personal or new HOF high score
		if (newPersonalText != null) {
			if (personal) {
				newPersonalText.SetActive (true);
			} else {
				newPersonalText.SetActive (false);
			}
		}
		if (newHallText != null) {
			if (hall) {
				newHallText.SetActive (true);
			} else {
				newHallText.SetActive (false);
			}
		}

		//Check if high score celebration should be played
		if(hallFirst){
			StartCoroutine (playNewHighScoreSong ()); //New High Score (HOF #1) Fanfare, gap banners, platform banners, triumphant sound
		}else if(hall){
			StartCoroutine (playHallOfFameScoreSong ()); //New Hall of Fame score (HOF #2-10) fanfare, only gap banners, sound
		}else if(personal){
			//StartCoroutine (playPersonalScoreSong ()); //New personal high score but not in HOF(no song for this yet)
		}

		//Testing
		//StartCoroutine (playNewHighScoreSong ());
		//StartCoroutine (playHallOfFameScoreSong ());
	}

	//New High Score Fanfare, gap banners, platform banners, triumphant sound
	IEnumerator playNewHighScoreSong(){
		if(source.isPlaying){
			yield return null;
		}
		yield return new WaitForSecondsRealtime (.7f);
		hiScoreBanner1.SetBool ("NewHighScore", true);
		hiScoreBanner2.SetBool ("NewHighScore", true);
		platformBanners.SetActive(true);
		source.PlayOneShot (hallHiSound, .5f);
	}

	//New Hall of Fame score fanfare, only gap banners, sound
	IEnumerator playHallOfFameScoreSong(){
		if(source.isPlaying){
			yield return null;
		}
		yield return new WaitForSecondsRealtime (.7f);
		hiScoreBanner1.SetBool ("NewHighScore", true);
		hiScoreBanner2.SetBool ("NewHighScore", true);
		source.PlayOneShot (playerHiSound, .5f);
	}

	//New Hall of Fame score fanfare, only gap banners, sound
	IEnumerator playPersonalScoreSong(){
		if(source.isPlaying){
			yield return null;
		}
		yield return new WaitForSecondsRealtime (.7f);
		//hiScoreBanner1.SetBool ("NewHighScore", true);
		//hiScoreBanner2.SetBool ("NewHighScore", true);
		source.PlayOneShot (personalHiSound, .5f);
	}
}
