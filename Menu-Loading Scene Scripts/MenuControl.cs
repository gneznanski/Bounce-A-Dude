using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

//Controls the game objects in the Menu scene
public class MenuControl : MonoBehaviour {
	public Sprite playerHighlightSprite;
	public AudioClip selectSound, acceptSound, warningSound, removeSound, chooseSound, backSound, clickSound, loginSound;
	private SmScript smScript;
	private Text player, character, playerScore, charPanelText, confirmText, playerSelButtonText, playerRemButtonText, charSelButtonText, loginInfoTextIOS,
				inputHint, inputHintFirst;
	private Text[] scoreTexts, playerTexts, charTexts;
	private GameObject input, playerList, charList, charSelectButton, playerSelectButton, playerRemoveButton, animWindow, confirmPanel, newPlayerButton, maxPlayerImage, 
				lbPanel, loginInfoPanelIOS, loginInfoPanelCover, lbIconAndroid, lbIconIOS, inputFirstTime, inputInnerPanel, inputFirstInnerPanel;
	private Button cancelNewPlayerButton, lbButton, tempButton;
	private InputField inputField, inputFieldFirst;
	private Animator anim;
	private AudioSource source;
	private FadeController fadeScript;
	private Vector3 inputMoveVector, inputFirstMoveVector, inputResetVector, inputFirstResetVector;
	private Color textBaseColor = new Color(36/255f, 191/255f, 1f, 1f); //Light blue
	private Color bGBaseColor = new Color(1f, 1f, 1f, 1f); //White
	private Color charTextHighlightColor = new Color(251/255f, 81/255f, 81/255f, 1f); //Light red
	private Color charBGHighlightColor = new Color(71/255f, 109/255f, 161/255f, 1f); //Light blue
	private Color playerTextHighlightColor = new Color(40/255f, 219/255f, 108/255f, 1f); //Seafoam
	private Color playerBGHighlightColor = new Color(168/255f, 87/255f, 191/255f, 1f); //Purply
	private string playerName, textNum, charTextNum, removeName;
	private int playerNum, count, charHighlightNum;
	private bool scoresUpdated, cancellingNewPlayer, playedScrollSound, fadingAnim, fadeIn, playSignInSound, manualSignInIOS, noPlayer, inputCaretSet, inputFirstCaretSet;
	private float volLevel = .5f;
	private int numOfPlayers = 10;
	private const int numOfChars = 4;
	private const string char1 = "Bones McGee";
	private const string char2 = "Bruce";
	private const string char3 = "Ninja";
	private const string char4 = "Duck Team";

	// Use this for initialization
	void Start () {
		smScript = GameObject.Find ("SceneManager").GetComponent<SmScript> ();
		smScript.enabled = true;
		player = GameObject.Find ("PlayerText").GetComponent<Text>();
		character = GameObject.Find ("CharacterText").GetComponent<Text> ();
		playerScore = GameObject.Find ("PlayerHiScoreText").GetComponent<Text> ();
		input = GameObject.Find ("InputPanel Cancel Option");
		inputField = GameObject.Find ("InputField Cancel Option").GetComponent<InputField> ();
		inputFirstTime = GameObject.Find ("InputPanel First Time");
		inputFieldFirst = GameObject.Find ("InputField First Time").GetComponent<InputField> ();
		inputInnerPanel = GameObject.Find("InputPanel Cancel Option Inner Panel");
		inputFirstInnerPanel = GameObject.Find ("InputPanel First Time Inner Panel");
		inputHint = GameObject.Find ("PlaceholderCancelOption").GetComponent<Text>();
		inputHintFirst = GameObject.Find ("PlaceholderFirstTime").GetComponent<Text>();
		playerList = GameObject.Find ("Player Select Panel");
		charList = GameObject.Find ("Character Select Panel");
		anim = GameObject.Find ("AnimationSprite").GetComponent<Animator>();
		charSelectButton = GameObject.Find ("Char Select Button Text");
		charSelButtonText = charSelectButton.GetComponent<Text> ();
		playerSelectButton = GameObject.Find("Player Select Button Text");
		playerSelButtonText = playerSelectButton.GetComponent<Text> ();
		playerRemoveButton = GameObject.Find ("Remove Player Button Text");
		playerRemButtonText = playerRemoveButton.GetComponent<Text> ();
		cancelNewPlayerButton = GameObject.Find ("Cancel New Player Button").GetComponent<Button> ();
		animWindow = GameObject.Find ("Animation Window");
		confirmPanel = GameObject.Find ("Remove Player Panel");
		confirmText = GameObject.Find ("Remove Name Text").GetComponent<Text>();
		newPlayerButton = GameObject.Find("New Player Button");
		maxPlayerImage = GameObject.Find("Max Player Image");
		lbButton = GameObject.Find ("Leaderboard Button").GetComponent<Button>();
		lbButton.onClick.AddListener (smScript.gameObject.GetComponent<SocialManager>().displayLeaderBoard);
		lbPanel = GameObject.Find ("Leaderboard Button Cover");
		loginInfoPanelIOS = GameObject.Find ("Login Info Panel IOS");
		loginInfoPanelCover = GameObject.Find ("Login Panel Coverup");
		loginInfoTextIOS = GameObject.Find ("InfoPanel Text IOS").GetComponent<Text>();
		lbIconAndroid = GameObject.Find ("GPGS Icon Background");
		lbIconIOS = GameObject.Find ("GameCenter Icon Background");
		fadeScript = GameObject.Find ("Fade Image1").GetComponent<FadeController> ();
		fadeScript.fadeInMenu (); //Fade in the image
		inputMoveVector = new Vector3(13, 135, 0);
		inputFirstMoveVector = new Vector3(13, 165, 0);
		inputResetVector = new Vector3(13, 0, 0);
		inputFirstResetVector = new Vector3(13, 31, 0);
		source = GetComponent<AudioSource>();
		cancellingNewPlayer = false;
		playedScrollSound = false;
		input.SetActive (false); //Turn off input field until needed
		inputFirstTime.SetActive(false); //Turn off first time input field until needed
		confirmPanel.SetActive(false); //Turn off panel until needed
		scoreTexts = new Text[10];
		charTexts = new Text[10];
		textNum = "";
		charTextNum = "";
		Time.timeScale = 1;
		smScript.resetShowingAd ();
		
		//Get number of saved players from playerprefs, or add entry if first time playing
		if(PlayerPrefs.HasKey("playersSaved")){
			numOfPlayers = 10;
			playerNum = PlayerPrefs.GetInt ("playersSaved");
			if(playerNum >= 10){ //Toggle button or image if max players are saved
				newPlayerButton.SetActive (false);
			}else{
				maxPlayerImage.SetActive (false);
			}
		}else{
			PlayerPrefs.SetInt ("playersSaved", 0); //Create PlayerPref entry if first time playing
			playerNum = 0;
			maxPlayerImage.SetActive(false);
		}
		playerTexts = new Text[numOfPlayers];
		setupPlayerSelectWindow (); //Setup panels for selecting player and character
		setupCharSelectWindow ();
		
		//Set up High Score window
		for (int i = 0; i < scoreTexts.Length; i++) {
			scoreTexts [i] = GameObject.Find ("Score" + (i+1).ToString () + "Text").GetComponent<Text> ();

			//Get high scores from player prefs
			if (PlayerPrefs.HasKey("hallScore" + i.ToString())){
				if(PlayerPrefs.GetInt ("hallScore" + i.ToString ()) == 0){
					scoreTexts [i].text = "";
				}else{
					scoreTexts [i].text = PlayerPrefs.GetString("hallName" + i.ToString()) + ": " + PlayerPrefs.GetInt ("hallScore" + i.ToString ()).ToString("N0") + " points"; //set text with score
				}
			}else{
				PlayerPrefs.SetInt ("hallScore" + i.ToString (), 0); //Create playerpref entries if first time playing
				PlayerPrefs.SetString("hallName" + i.ToString(), "");
			}
		}

		//Set up current player texts and player prefs keys, or assign if first time playing
		if (PlayerPrefs.HasKey ("curPlayer")) {
			player.text = PlayerPrefs.GetString ("curPlayer");
			if (player.text == "") {
				playerScore.text = "";
				noPlayer = true;
			} else {
				playerScore.text = player.text + "'s High Score: " + PlayerPrefs.GetInt ("player" + PlayerPrefs.GetString (player.text) + "Score").ToString ("N0");
			}
		}else{
			player.text = "";
			playerScore.text = "";
			inputFirstTime.SetActive (true); //Turn on first time input field
			noPlayer = true;
		}
		if (PlayerPrefs.HasKey ("curChar")) {
			character.text = PlayerPrefs.GetString ("curChar");
		}else{
			PlayerPrefs.SetString ("curChar", char1);
		}
		updateAnim (); //Update the character animation for the current player
		if(player.text == ""){ //If there is no saved player, turn off animation until the player enters a name
			anim.GetComponent<SpriteRenderer>().enabled = false;
			character.text = "";
		}

		smScript.gameObject.GetComponent<SocialManager>().mScript = gameObject.GetComponent<MenuControl>();
		SocialManager.sManager.mScript = gameObject.GetComponent<MenuControl> ();
		smScript.gameObject.GetComponent<ConnectionMonitor> ().mScript = gameObject.GetComponent<MenuControl>();
		SocialManager.sManager.cmScript = smScript.gameObject.GetComponent<ConnectionMonitor> ();
		
		toggleLBButton (true);
		setLoginInfoPanelText ();
		setLBButtonIcon ();
		loginInfoPanelIOS.SetActive (false);
		loginInfoPanelCover.SetActive (false);

		if (smScript.ready) {
			smScript.checkAdStatus ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(noPlayer){
			if(!SocialManager.sManager.loggingIn){
				anim.GetComponent<SpriteRenderer>().enabled = false;
				inputFirstTime.SetActive(true); //Turn on input field until a name is entered
			}
		}

		if(smScript.showingAd){ //Stop music playing before an ad
			if(source.isPlaying){
				source.Stop ();
			}
		}

		//Move the name input fields above the keyboard when the user is typing
		if(inputField.gameObject.activeInHierarchy){
			if(inputField.isFocused){
				inputInnerPanel.GetComponent<RectTransform> ().localPosition = inputMoveVector;
				inputHint.text = "";
				if (!inputCaretSet) {
					inputCaretSet = true;
					StartCoroutine (setCaret (false));
				}
			}
		}else if(inputFieldFirst.gameObject.activeInHierarchy){
			if(inputFieldFirst.isFocused){
				inputFirstInnerPanel.GetComponent<RectTransform> ().localPosition = inputFirstMoveVector;
				inputHintFirst.text = "";
				if (!inputFirstCaretSet) {
					inputFirstCaretSet = true;
					StartCoroutine (setCaret (true));
				}
			}
		}
	}
	
	//Return the highest score on the Hall of Fame
	public string getHallHighScore(){
		return scoreTexts [0].text;
	}

	//4 methods handle Character selection and updating UI at different stages in the process
	public void charButtonClicked(){ //Change Character button onClick method
		playUISound(1); //Play menu sound
		charList.SetActive (true); //Turn on the character select Panel
		charSelButtonText.text = "Accept";
		updateCharHighlighting(); //Refresh the panel to show the current character highlight image
	}
	public void changeChar(Text text){ //Change the current character to another model
		if(!charTextNum.Equals("")){
			//Change the old character from highlighted to default colors
			GameObject.Find ("CharSelectButton" + charTextNum.ToString()).GetComponent<Button> ().image.overrideSprite = null;
			GameObject.Find ("CharButtonText" + charTextNum.ToString()).GetComponent<Text>().color = textBaseColor;
			GameObject.Find ("CharSelectButton" + charTextNum.ToString()).GetComponent<Image>().color = bGBaseColor;
		}
		character.text = text.text;
		charTextNum = Regex.Replace (text.name, "[^0-9]", "");

		//Highlight the currently selected character on the character panel
		GameObject.Find ("CharSelectButton" + charTextNum.ToString ()).GetComponent<Button> ().image.overrideSprite = playerHighlightSprite;
		GameObject.Find ("CharButtonText" + charTextNum.ToString()).GetComponent<Text>().color = charTextHighlightColor;
		GameObject.Find ("CharSelectButton" + charTextNum.ToString ()).GetComponent<Image> ().color = charBGHighlightColor;

		smScript.reset = 3; //Reset the game scene after a new character has been selected
		PlayerPrefs.SetString("player" + PlayerPrefs.GetString(PlayerPrefs.GetString("curPlayer")) + "Char", character.text);
		PlayerPrefs.SetString ("curChar", character.text);
		updateAnim ();
	}
	public void updateSelectButton(Text text){ //Update the select button text
		playUISound(1);
		charSelButtonText.text = "Accept";
	}
	public void setChar(){ //Apply the changed character settings
		charList.SetActive(false);
		playUISound(5);
	}

	//Methods to handle Player selection and updating UI at different stages in the process
	public void playerButtonClicked(){ //Change Player button onClick method
		if (!cancellingNewPlayer) { //Avoid playing multiple sounds if cancel adding new player button is clicked
			playUISound (1);
		}
		cancellingNewPlayer = false;
		playerList.SetActive (true);
		playerSelButtonText.text = "Accept"; //Update player panel button texts
		playerRemButtonText.text = "Remove Player";
		playerName = player.text;
		textNum = PlayerPrefs.GetString (playerName);
		animWindow.SetActive (false); //Turn off the animation window to not show over player select panel
		updatePlayerHighlighting ();
	}
	public void changePlayer(Text text){ //Change the current player to either a saved char or a new one
		if(!textNum.Equals("")){
			//Change the old player from highlighted to default colors
			GameObject.Find ("PlayerSelectButton" + textNum.ToString ()).GetComponent<Button> ().image.overrideSprite = null;
			GameObject.Find ("Player" + textNum.ToString() + "Text").GetComponent<Text>().color = textBaseColor;
			GameObject.Find ("PlayerSelectButton" + textNum.ToString()).GetComponent<Image>().color = bGBaseColor;
		}
		playerName = text.text;
		textNum = Regex.Replace (text.name, "[^0-9]", "");

		//Highlight the currently selected player on the player panel
		GameObject.Find ("PlayerSelectButton" + textNum.ToString ()).GetComponent<Button> ().image.overrideSprite = playerHighlightSprite;
		GameObject.Find ("Player" + textNum.ToString() + "Text").GetComponent<Text>().color = playerTextHighlightColor;
		GameObject.Find ("PlayerSelectButton" + textNum.ToString ()).GetComponent<Image> ().color = playerBGHighlightColor;

		smScript.reset = 3;

	}
	public void updatePlayerSelectButton(Text text){ //Update the select button text, when player slot is highlighted
		playUISound(1);
		playerSelButtonText.text = "Accept"; //Update player panel button texts
		playerRemButtonText.text = "Remove Player";
		playerName = text.text;
		textNum = Regex.Replace (text.name, "[^0-9]", "");
	}
	public void setPlayer(){ //Apply the changed character settings
		if (!cancellingNewPlayer) {
			playUISound (5);
		}
		if(playerName == null){
			playerName = PlayerPrefs.GetString ("curPlayer");
		}else{
			PlayerPrefs.SetString ("curPlayer", playerName); //Update the current character saved info to selected player values
			PlayerPrefs.SetString ("curChar", PlayerPrefs.GetString ("player" + textNum + "Char"));
		}
		player.text = playerName;
		playerScore.text = playerName + "'s High Score: " + PlayerPrefs.GetInt("player" + PlayerPrefs.GetString(playerName) + "Score").ToString("N0");
		animWindow.SetActive(true);
		updateAnim ();
		playerList.SetActive(false);
	}
	public void confirmRemovePlayer(){ //Give the player the option to cancel removing a player
		playUISound(3);
		confirmPanel.SetActive(true);
		removeName = playerName;
		confirmText.text = removeName + "?";
	}
	public void removePlayer(){ //Remove the player info from PlayerPrefs
		playUISound(4);
		int curPlayerNum = int.Parse(PlayerPrefs.GetString (PlayerPrefs.GetString("curPlayer")));
		GameObject.Find ("PlayerSelectButton" + textNum.ToString ()).GetComponent<Button> ().image.overrideSprite = null; //Unhighlight the selected player
		refreshPlayerSavedInfo (); //Remove chosen player, move following players up to fill removed players position in the panel
		setupPlayerSelectWindow (); //Refresh the player panel to reflect the removal and repositioning of players in the panel
		if(removeName.Equals(PlayerPrefs.GetString("curPlayer"))){ //If current player is removed, current player becomes the first player in the list
			if (PlayerPrefs.HasKey ("player1Name")) {
				PlayerPrefs.SetString ("curPlayer", PlayerPrefs.GetString ("player1Name"));
				PlayerPrefs.SetString ("curChar", PlayerPrefs.GetString ("player1Char"));
				playerName = PlayerPrefs.GetString ("curPlayer");
				player.text = playerName;
				character.text = PlayerPrefs.GetString ("curChar");
				playerScore.text = playerName + "'s High Score: " + PlayerPrefs.GetInt ("player" + PlayerPrefs.GetString (playerName) + "Score").ToString("N0");
				playerSelButtonText.text = "Accept"; //Update player panel button texts
				playerRemButtonText.text = "Remove Player";
			}else{ //If no players remaining, get a name
				PlayerPrefs.SetString ("curPlayer", "");
				PlayerPrefs.SetString ("curChar", char1);
				playerName = "";
				player.text = playerName;
				character.text = "";
				playerScore.text = "";
			}
		}else if(int.Parse(textNum) < curPlayerNum){ //If removed player was not the current player, do not change current player
			curPlayerNum -= 1;
			playerName = PlayerPrefs.GetString ("player" + curPlayerNum + "Name");
			character.text = PlayerPrefs.GetString ("player" + curPlayerNum + "Char");
			PlayerPrefs.SetString ("curPlayer", playerName);
			PlayerPrefs.SetString ("curChar", character.text);
			playerScore.text = playerName + "'s High Score: " + PlayerPrefs.GetInt("player" + curPlayerNum + "Score").ToString("N0");
		}
		PlayerPrefs.SetInt ("playersSaved", PlayerPrefs.GetInt ("playersSaved") - 1); //Update number of players that have been saved
		playerNum = PlayerPrefs.GetInt ("playersSaved");
		if(playerNum < 10){ //If number saved is less than 10, toggle what is shown, allow for another player to be saved
			newPlayerButton.SetActive (true);
			maxPlayerImage.SetActive (false);
		}
		confirmPanel.SetActive (false);
		animWindow.SetActive(true);
		if(playerNum != 0){
			anim.GetComponent<SpriteRenderer> ().enabled = true;
			updateAnim ();
		}
		if(player.text == ""){ //Turn off the animation until a name is entered
			anim.GetComponent<SpriteRenderer>().enabled = false;
			noPlayer = true;
		}
	}
	public void cancelRemovePlayer(){ //Method for when player selects cancel instead of removing the player
		playUISound(6);
		updatePlayerHighlightingAfterCancel (removeName);
		confirmPanel.SetActive (false);
		cancellingNewPlayer = false;
	}
	public void checkPlayerName (string name){ //Make sure the entered name has not already been saved
		resetInputPanel();
		if (PlayerPrefs.HasKey (name.ToLower ())) {
			if (!name.Equals ("")) { //Repeat name
				reEnterPlayerName();
				playUISound (3);
			}
		} else {
			if (!name.Equals ("")) { //No text entered
				setNewPlayer (name.ToLower ()); //Prepare to create the new player
			}
		}
	}
	public void reEnterPlayerName(){ //Set up panel to allow player to enter a new name after a repeat
		if(noPlayer){
			inputFieldFirst.text = "";
		}else{
			inputField.text = "";
		}
	}
	public void cancelNewPlayer(){ //If the player chooses to cancel entering a new player name
		cancellingNewPlayer = true; //Used to avoid playing multiple sounds
		playerName = PlayerPrefs.GetString ("curPlayer");
		inputField.text = "";
		input.SetActive (false);
		playUISound (6);
		setPlayer ();
		playerButtonClicked ();
	}
	//Get the name of the player, first time playing
	void setNewPlayer(string name){
		if (!cancellingNewPlayer) {
			playUISound (5);
			playerName = name;
			PlayerPrefs.SetString ("curPlayer", playerName);
			PlayerPrefs.SetString ("curChar", char1);
			player.text = playerName;
			playerList.SetActive (true);
			GameObject.Find ("PlayerSelectButton" + (playerNum + 1).ToString ()).GetComponent<Button> ().interactable = true;
			playerTexts [playerNum].text = playerName;
			playerNum++;
			if (playerNum == 10) {
				newPlayerButton.SetActive (false);
				maxPlayerImage.SetActive (true);
			}
			playerList.SetActive (false);
			PlayerPrefs.SetString ("player" + playerNum.ToString () + "Name", playerName);
			PlayerPrefs.SetString ("player" + playerNum.ToString () + "Char", char1);
			PlayerPrefs.SetString (playerName, playerNum.ToString ());
			PlayerPrefs.SetInt ("player" + playerNum.ToString () + "Score", 0);
			PlayerPrefs.SetInt ("playersSaved", PlayerPrefs.GetInt ("playersSaved") + 1);
			playerNum = PlayerPrefs.GetInt ("playersSaved");
			playerScore.text = playerName + "'s High Score: " + 0.ToString ();

			if(noPlayer){
				noPlayer = false;
				inputFieldFirst.text = "";
				inputFirstTime.SetActive(false);
			}else{
				inputField.text = "";
				input.SetActive (false);
			}
			animWindow.SetActive (true);
			anim.GetComponent<SpriteRenderer>().enabled = true;
			updateAnim ();
		}else{
			cancellingNewPlayer = false;
		}
	}
	//Turn on the inputfield, wait for name input
	public void createNewPlayer(){
		if (playerNum <= 10) {
			playUISound (5);
			input.SetActive (true);
		}
	}

	//Start the game, load game scene
	public void playGame(){
		playUISound(2);
		loginInfoPanelCover.SetActive (true); 
		smScript.playButtonOnClick2 ();
	}

	//Update the animation window to show the current selected character's animation
	void updateAnim(){
		if(PlayerPrefs.GetString("curChar") == char1){
			anim.SetInteger("State", 1); //State is name of animator condition, 1 is state value to move to Bones Mcgee animation in menu animation controller
			character.text = char1;
		}else if(PlayerPrefs.GetString("curChar") == char2){
			anim.SetInteger ("State", 2); //Bruce animation value
			character.text = char2;
		}else if(PlayerPrefs.GetString("curChar") == char3){
			anim.SetInteger ("State", 3); //Ninja animation value
			character.text = char3;
		}else if(PlayerPrefs.GetString("curChar") == char4){
			anim.SetInteger ("State", 4); //Duck Team animation value
			character.text = char4;
		}else{ //Insert else if if adding new character here
			anim.SetInteger ("State", 0); //Dont show any animation
		}
	}

	//Set up the player select window to only show available options
	void setupPlayerSelectWindow(){
		for(int i = 0; i < numOfPlayers; i++){
			playerTexts [i] = GameObject.Find ("Player" + (i+1).ToString () + "Text").GetComponent<Text>();

			if(PlayerPrefs.HasKey ("player" + (i+1).ToString () + "Name")){
				playerTexts [i].text = PlayerPrefs.GetString ("player" + (i+1).ToString () + "Name");
				//playerNum++;
			}else{
				playerTexts [i].text = "";
				GameObject.Find ("PlayerSelectButton" + (i+1).ToString ()).GetComponent<Button> ().interactable = false;
			}
		}
		updatePlayerHighlighting ();
		playerList.SetActive (false); //Turn off player list until needed
	}

	//Set up the char select window to only show available options
	void setupCharSelectWindow(){
		for(int i = 0; i < 10; i++){
			charTexts [i] = GameObject.Find ("CharButtonText" + (i+1).ToString ()).GetComponent<Text>();

			if(charTexts[i].text.Equals("")){
				GameObject.Find ("CharSelectButton" + (i+1).ToString ()).GetComponent<Button>().interactable = false;
			}
		}
		updateCharHighlighting ();
		charList.SetActive (false); //Turn off player list until needed
	}

	//Update highlighting to only highlight the current character selected
	void updateCharHighlighting(){
		for(int i = 0; i < numOfChars; i++){
			if(charTexts[i].text.Equals(character.text)) {
				GameObject.Find ("CharSelectButton" + (i+1).ToString()).GetComponent<Button> ().image.overrideSprite = playerHighlightSprite;
				GameObject.Find ("CharButtonText" + (i+1).ToString()).GetComponent<Text>().color = charTextHighlightColor;
				GameObject.Find ("CharSelectButton" + (i+1).ToString ()).GetComponent<Image> ().color = charBGHighlightColor;

				charTextNum = (i + 1).ToString ();
				charSelButtonText.text = "Accept";
			}else {
				GameObject.Find ("CharSelectButton" + (i+1).ToString()).GetComponent<Button> ().image.overrideSprite = null;
				GameObject.Find ("CharButtonText" + (i+1).ToString()).GetComponent<Text>().color = textBaseColor;
				GameObject.Find ("CharSelectButton" + (i+1).ToString ()).GetComponent<Image> ().color = bGBaseColor;
			}
		}
	}

	//Update highlighting to only highlight the current player selected
	void updatePlayerHighlighting(){
		for(int i = 0; i < playerNum; i++){
			if(playerTexts[i].text.Equals(player.text)) {
				GameObject.Find ("PlayerSelectButton" + (i+1).ToString()).GetComponent<Button> ().image.overrideSprite = playerHighlightSprite;
				GameObject.Find ("Player" + (i+1).ToString() + "Text").GetComponent<Text>().color = playerTextHighlightColor;
				GameObject.Find ("PlayerSelectButton" + (i+1).ToString ()).GetComponent<Image> ().color = playerBGHighlightColor;

				textNum = (i + 1).ToString ();
				playerSelButtonText.text = "Accept";
			}else {
				GameObject.Find ("PlayerSelectButton" + (i+1).ToString()).GetComponent<Button> ().image.overrideSprite = null;
				GameObject.Find ("Player" + (i+1).ToString() + "Text").GetComponent<Text>().color = textBaseColor;
				GameObject.Find ("PlayerSelectButton" + (i+1).ToString ()).GetComponent<Image> ().color = bGBaseColor;
			}
		}
	}

	//Update highlighting after canceling remove player
	void updatePlayerHighlightingAfterCancel(string selectedPlayer){
		for(int i = 0; i < playerNum; i++){
			if(playerTexts[i].text.Equals(selectedPlayer)) {
				GameObject.Find ("PlayerSelectButton" + (i+1).ToString()).GetComponent<Button> ().image.overrideSprite = playerHighlightSprite;
				textNum = (i + 1).ToString ();
				playerSelButtonText.text = "Accept";
			}else {
				GameObject.Find ("PlayerSelectButton" + (i+1).ToString()).GetComponent<Button> ().image.overrideSprite = null;
			}
		}
	}

	//Remove the selected player, move following players up the panel
	void refreshPlayerSavedInfo(){
		PlayerPrefs.DeleteKey(removeName);
		PlayerPrefs.DeleteKey("player" + textNum.ToString() + "Char");
		PlayerPrefs.DeleteKey("player" + textNum.ToString() + "Name");
		PlayerPrefs.DeleteKey("player" + textNum.ToString() + "Score");
		if (int.Parse (textNum) != playerNum) {
			for (int i = int.Parse (textNum); i < playerNum; i++) {
				PlayerPrefs.SetString(PlayerPrefs.GetString("player" + (i+1).ToString() + "Name"), (int.Parse(PlayerPrefs.GetString(PlayerPrefs.GetString("player" + (i+1).ToString() + "Name"))) - 1).ToString());
				PlayerPrefs.SetString ("player" + i.ToString () + "Char", PlayerPrefs.GetString ("player" + (i + 1).ToString () + "Char"));
				PlayerPrefs.SetString ("player" + i.ToString () + "Name", PlayerPrefs.GetString ("player" + (i + 1).ToString () + "Name"));
				PlayerPrefs.SetInt ("player" + i.ToString () + "Score", PlayerPrefs.GetInt ("player" + (i + 1).ToString () + "Score"));
			}
			PlayerPrefs.DeleteKey("player" + playerNum.ToString() + "Char");
			PlayerPrefs.DeleteKey("player" + playerNum.ToString() + "Name");
			PlayerPrefs.DeleteKey("player" + playerNum.ToString() + "Score");
		}
	}

	//Method to handle which menu sound should be played
	public void playUISound(int soundNum){
		if(soundNum == 1){
			source.PlayOneShot (selectSound, volLevel);
		}else if(soundNum == 2){
			source.PlayOneShot (acceptSound, volLevel);
		}else if(soundNum == 3){
			source.PlayOneShot (warningSound, volLevel);
		}else if(soundNum == 4){
			source.PlayOneShot (removeSound, volLevel);
		}else if(soundNum == 5){
			source.PlayOneShot (chooseSound, volLevel);
		}else if(soundNum == 6){
			source.PlayOneShot (backSound, volLevel);
		}else if(soundNum == 7){
			source.PlayOneShot (clickSound, volLevel-.25f);
		}else if(soundNum == 8){
			source.PlayOneShot (loginSound, volLevel-.25f);
		}else{
			source.PlayOneShot (clickSound, volLevel-.25f);
		}
	}

	//Sound played when scrolling the player or character select scrollviews
	public void scrollSound(float value){
		if(value >= .98f && value <= .99f){
			if (!playedScrollSound) {
				playUISound (7);
				playedScrollSound = true;
			}
		}else if(value >= .89f && value <= .92f){
			if (!playedScrollSound) {
				playUISound (7);
				playedScrollSound = true;
			}
		}else if(value >= .79f && value <= .82f){
			if (!playedScrollSound) {
				playUISound (7);
				playedScrollSound = true;
			}
		}else if(value >= .69f && value <= .72f){
			if (!playedScrollSound) {
				playUISound (7);
				playedScrollSound = true;
			}
		}else if(value >= .59f && value <= .62f){
			if (!playedScrollSound) {
				playUISound (7);
				playedScrollSound = true;
			}
		}else if(value >= .49f && value <= .52f){
			if (!playedScrollSound) {
				playUISound (7);
				playedScrollSound = true;
			}
		}else if(value >= .39f && value <= .42f){
			if (!playedScrollSound) {
				playUISound (7);
				playedScrollSound = true;
			}
		}else if(value >= .29f && value <= .32f){
			if (!playedScrollSound) {
				playUISound (7);
				playedScrollSound = true;
			}
		}else if(value >= .19f && value <= .22f){
			if (!playedScrollSound) {
				playUISound (7);
				playedScrollSound = true;
			}
		}else if(value >= .09f && value <= .12f){
			if (!playedScrollSound) {
				playUISound (7);
				playedScrollSound = true;
			}
		}else if(value >= 0f && value <= .02f){
			if (!playedScrollSound) {
				playUISound (7);
				playedScrollSound = true;
			}
		}else{
			playedScrollSound = false;
		}
	}
	
//	//Decide whether to sign in to GPGS or not based on players previous sign in decision
//	public void gpgsLogin(){
//		if (!smScript.gpgsLoginChecked) {
//			if (PlayerPrefs.GetInt("gpgsLoginCheck", 5) != 5) { //No value saved previously
//				if (PlayerPrefs.GetInt ("gpgsLoginCheck") == 0) { //Player was previously logged in
//					SocialManager.sManager.logIn ();
//				}else{ //Player signed out or declined to sign in previously
//					smScript.gpgsLoginDeclined = true;
//				}
//			} else { //First time playing, try to log the player in
//				setGPGSLoginCheck (true);
//				SocialManager.sManager.logIn ();
//			}
//			smScript.gpgsLoginChecked = true;
//		}
//	}
//	
//	//Update Playerprefs for players decision to login or not to GPGS
//	public void setGPGSLoginCheck(bool result){
//		if(result){ //Player allows login
//			PlayerPrefs.SetInt ("gpgsLoginCheck", 0);
//			smScript.gpgsLoginDeclined = false;
//		}else{ //Player declines login
//			PlayerPrefs.SetInt ("gpgsLoginCheck", 1);
//			smScript.gpgsLoginDeclined = true;
//		}
//	}

	//Make the leaderboard button clickable or not
	public void toggleLBButton(bool toggle){
		if(toggle){
			lbButton.interactable = true;
		}else{
			lbButton.interactable = false;
			lbPanel.SetActive (true);
		}
	}
	
	//Display the appropriate icon on the leaderboard button for the game service on the device
	private void setLBButtonIcon(){
		#if UNITY_EDITOR
			lbIconAndroid.SetActive(false);
			//lbIconIOS.SetActive(false);
		#elif UNITY_ANDROID
			lbIconIOS.SetActive(false);
		#elif UNITY_IPHONE
			lbIconAndroid.SetActive(false);
		#endif
	}
	
	//Set the text on the panel for when the player selects leaderboard button but is not signed in to a game service
	private void setLoginInfoPanelText(){
		#if UNITY_EDITOR
			loginInfoTextIOS.text = "Leaderboard requires Game Center to be enabled.\n\nTo sign in to Game Center go to your device's Settings  >  Applications  >  Game Center.";
		#elif UNITY_IPHONE
			loginInfoTextIOS.text = "Leaderboard requires Game Center to be enabled.\n\nTo sign in to Game Center go to your device's Settings  >  Applications  >  Game Center.";
		#endif
	}

	//Display or hide the IOS info panel when the player is not signed in to Game Center
	public void toggleLoginInfoPanel(bool toggle){
		if (toggle) {
			loginInfoPanelIOS.SetActive (true);
			loginInfoPanelIOS.GetComponent<Animator> ().SetInteger ("Switch", 1);
		}else{
			playUISound (6); //5
			loginInfoPanelIOS.GetComponent<Animator> ().SetInteger ("Switch", 2);
		}
	}
	
	//Getter/Setter for toggling when to play signin sound
	public bool getPlaySignInSound(){
		return playSignInSound;
	}
	public void setPlaySignInSound(bool toggle){
		if (toggle) {
			playSignInSound = true;
		}else{
			playSignInSound = false;
		}
	}
	
	//Getter/Setter for toggling login info panel coverup
	public GameObject getLoginCoverup(){
		return loginInfoPanelCover;
	}
	public void setLoginPanelCoverup(bool toggle){
		if(toggle){
			loginInfoPanelCover.SetActive (true);
		}else{
			loginInfoPanelCover.SetActive (false);
		}
	}
	
	//Reset the input field panel position after done entering name(moved up above keyboard - in Update())
	public void resetInputPanel(){
		if (input.activeInHierarchy) {
			inputInnerPanel.GetComponent<RectTransform> ().localPosition = inputResetVector;
			inputHint.text = "Press Here";
		} else if (inputFieldFirst.gameObject.activeInHierarchy) {
			inputFirstInnerPanel.GetComponent<RectTransform> ().localPosition = inputFirstResetVector;
			inputHintFirst.text = "Press Here";
		}
	}
	
	//Position the typing caret correctly in the center of the inputfield
	IEnumerator setCaret(bool first){
		yield return null; //Wait until the next frame, need this because the gameobject is created at runtime when inputfield is first focused
		GameObject caret = null;
		if(!first){ //CancelOption
			caret = GameObject.Find ("InputField Cancel Option Input Caret");
		}else{
			caret = GameObject.Find ("InputField First Time Input Caret");
		}
		if(caret != null){
			caret.GetComponent<RectTransform> ().pivot = new Vector2 (.5f, .5f); //Set the caret in the center of the input area
		}
	}
}
