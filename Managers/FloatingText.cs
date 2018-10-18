using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour {
	private Controller controlScript;
	public GameObject floatingTextAnchor;
	public GameObject textLocation;
	//public Color speedColor, popColor, deathColor, bombColor, alternatePowerUpColor, bombSetColor, bigModeColor, springColor;
	public Text text;
	private TextMovement textMovement;
	private Animator anim;
	private const float eventTextOffset = 1.8f;
	
	// Update is called once per frame
	void Update () {
//		if(controlScript == null){
//			controlScript = GameObject.Find ("Controller").GetComponent<Controller> ();
//		}
	}

	public void initialize(){
		floatingTextAnchor = gameObject;
		//controlScript = GameObject.Find ("Controller").GetComponent<Controller> ();
		textMovement = gameObject.GetComponent<TextMovement> ();
		text = gameObject.GetComponentInChildren<Text> ();
		anim = gameObject.GetComponentInChildren<Animator> ();
//		speedColor = new Color (134/255f, 234/255f, 45/255f, 1f); //lime
//		popColor = new Color (0f, 192/255f, 1f, 1f); //medium blue
//		bombColor = new Color (252/255f, 1f, 28/255f, 1f); //yellow
//		alternatePowerUpColor = new Color (0f, 192/255f, 1f, 1f); //medium blue
//		bombSetColor = new Color (1f, 0f, 0f, 1f); //red
//		bigModeColor = new Color(102/255f, 1f, 1f, 1f); //light blue
//		deathColor = new Color (1f, 100/255f, 0f, 1f); //orange
//		springColor = new Color(68/255f, 232/255f, 73/255f, 1f); //darker green
	}

	//Prep a text object for use from pooler
	private void setUpText(Vector3 location, bool moving, int fontSize){
		gameObject.transform.SetParent (GameObject.Find ("Canvas").transform, false);
		gameObject.transform.position = location;
		gameObject.transform.SetAsFirstSibling ();
		textMovement.moving = moving;
		//text.color = color;
		//text.resizeTextForBestFit = resize;
		text.fontSize = fontSize;
		//controlScript = GameObject.Find ("Controller").GetComponent<Controller> ();
	}

	//Set variables for balloon pop text from pooler
	public void createPopText(Vector3 location, int pointValue){
		location = new Vector3 (Random.Range (-.2f, .2f) + location.x, location.y + .1f, location.z);
		Vector2 textLocation = Camera.main.WorldToScreenPoint (location);
		setUpText(textLocation, false, 20);
		if(controlScript == null){
			controlScript = GameObject.Find ("Controller").GetComponent<Controller> ();
		}
		text.text = (pointValue + (4 * controlScript.speedMultiplier)).ToString ();
		anim.SetInteger ("State", 2);
	}

	//Set variables for death emote text from pooler
	public void createDeathText(Vector3 location, int dudeNum){
		float yOffset;
		if(dudeNum == 1){
			yOffset = .15f;
		}else if(dudeNum == 2){
			yOffset = .2f;
		}else{
			yOffset = .15f;
		}
		location = new Vector3 (Random.Range (-.2f, .2f) + location.x, location.y + yOffset, location.z);
		Vector2 textLocation = Camera.main.WorldToScreenPoint (location);
		setUpText(textLocation, true, 21);
		text.text = chooseText(4);
		anim.SetInteger ("State", 4);
	}

	//Set variables for speed up text from pooler
	public void createSpeedText(string inText){
		Vector3 resetLocation = textLocation.transform.position;
		textLocation.transform.position = new Vector3 (Random.Range (-2f, 2f) + textLocation.transform.position.x, textLocation.transform.position.y + 1.5f, textLocation.transform.position.z);
		Vector2 location = Camera.main.WorldToScreenPoint (textLocation.transform.position);
		setUpText(location, false, 22);
		text.text = inText;
		textLocation.transform.position = resetLocation;
		anim.SetInteger ("State", 5);
	}

	//Set variables for bomb explosion text from pooler
	public void createBombText(float x, float y){
		Vector2 textLocation = Camera.main.WorldToScreenPoint (new Vector3 (x, y, 0));
		setUpText(textLocation, true, 21);
		text.text = chooseText (1);
		anim.SetInteger ("State", 3);
	}

	//Set variables for powerup collected for points text from pooler
	public void createPowerUpAlternateText(Vector3 location, int pointValue){
		location = new Vector3 (Random.Range (-.2f, .2f) + location.x, location.y + .2f, location.z);
		Vector2 textLocation = Camera.main.WorldToScreenPoint (location);
		setUpText (textLocation, true, 21);
		text.text = pointValue.ToString ();
		anim.SetInteger ("State", 7);
	}

	//Set variables for bomb powerup collected text from pooler
	public void createBombSetText(Vector3 location){
		location = new Vector3 (Random.Range (-.2f, .2f) + location.x, location.y + eventTextOffset, location.z);
		Vector2 textLocation = Camera.main.WorldToScreenPoint (location);
		text.gameObject.transform.localScale = new Vector3 (.5f, .5f, 1f);
		setUpText(textLocation, true, 22);
		text.text = chooseText (2);
		anim.SetInteger ("State", 6);
	}

	//Set variables for big mode powerup collected text from pooler
	public void createBigModeText(Vector3 location){
		location = new Vector3 (Random.Range (-.2f, .2f) + location.x, location.y + eventTextOffset, location.z); //y - .4f
		Vector2 textLocation = Camera.main.WorldToScreenPoint (location);
		text.gameObject.transform.localScale = new Vector3 (.5f, .5f, 1f);
		setUpText(textLocation, true, 22);
		text.text = chooseText (3);
		anim.SetInteger ("State", 1);
	}

	//Set variables for bird/ducks powerup collected text from pooler
	public void createSpringText(Vector3 location){
		location = new Vector3 (Random.Range (-.2f, .2f) + location.x, location.y + eventTextOffset, location.z); //y - .4f
		Vector2 textLocation = Camera.main.WorldToScreenPoint (location);
		text.gameObject.transform.localScale = new Vector3 (.5f, .5f, 1f);
		setUpText(textLocation, true, 22);
		text.text = chooseText (5);
		anim.SetInteger ("State", 8);
	}

	//Not used (tornado)
	public void createVortexText(Vector3 location){
		location = new Vector3 (Random.Range (-.2f, .2f) + location.x, location.y + eventTextOffset, location.z); //y - .4f
		Vector2 textLocation = Camera.main.WorldToScreenPoint (location);
		text.gameObject.transform.localScale = new Vector3 (.5f, .5f, 1f);
		setUpText(textLocation, true, 22);
		text.text = chooseText (6);
		anim.SetInteger ("State", 8);
	}

	//Choose the emote to display for the appropriate text
	private string chooseText(int type){
		int rand;
		switch(type){
		case 1: //Bomb explode text
			rand = Random.Range (0, 5);
			if(rand == 0){
				return "Kablam!";
			}else if(rand == 1){
				return "Boom!";
			}else if(rand == 2){
				return "Spladow!";
			}else if(rand == 3){
				return "Bam!";
			}else if(rand == 4){
				return "Pow!";
			}else{
				return "Bang!";
			}
			//break;
		case 2: //Bomb set text
			rand = Random.Range (0, 3);
			if (rand == 0) {
				return "Bomb Armed";
			}else if(rand == 1){
				return "Fireballs Activated";
			}else {
				return "Explosion Imminent";
			}
			//break;
		case 3: //Big mode text
			rand = Random.Range (0, 3);
			if(rand == 0){
				return "Big Mode";
			}else if(rand == 1){
				return "Huge!";
			}else{
				return "Whoa!";
			}
			//break;
		case 4: //Death emote text
			rand = Random.Range (0, 6);
			if(rand == 0){
				return "Splat!";
			}else if(rand == 1){
				return "Thud!";
			}else if(rand == 2){
				return "Ouch!";
			}else if(rand == 3){
				return "Zowie!";
			}else if(rand == 4){
				return "Biff!";
			}else if(rand == 5){
				return "Whack!";
			}else{
				return "Wham!";
			}
			//break;
		case 5: //Bird powerup text
			rand = Random.Range (0, 3);
			if(rand == 0){
				return "Aww Yeah!";
			}else if(rand == 1){
				return "Falconer!";
			}else{
				return "Wild Ride!";
			}
		case 6: //Green Ducks powerup text
			rand = Random.Range (0, 3);
			if(rand == 0){
				return "More Ducks!";
			}else if(rand == 1){
				return "Anti Gravity!";
			}else{
				return "Sweet!";
			}
		default:
			return "";
			//break;
		}
	}

	//Text for speed up after max
	public string getColorChangeText(){
		switch(Random.Range(1, 6)){
		case 1:
			return "Doing Great!";
		case 2:
			return "Keep it up!";
		case 3:
			return "Wow!";
		case 4:
			return "New Colors!";
		case 5:
			return "Outstanding!";
		default:
			return "Color Change!";
		}
	}
}
