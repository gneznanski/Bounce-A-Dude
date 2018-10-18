using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeeterBase : MonoBehaviour{
	public GameObject control, dudeOnSaw, sawImageL, sawImageR, sawObjL, sawObjR, leftMid, rightMid, leftLimit, rightLimit, handle, scrollMin, scrollMax, gameMin, gameMax, baseImage;
	public AudioClip bounceSound;
	public BungieController bungieScript;
	public ParticleSystem landingPS;
	public float velocity, baseImageHeight;
	private SeesawCartWheels cartScript;
	private Rigidbody2D rigidB;
	private Scrollbar scrollBar;
	private ScrollRect scrollRect;
	private AudioSource source;
	private Transform controlEmpty;
    private Transform saw;
    private Controller controlScript;
    private DudeMovement dudeScript;
	private Bouncer bouncerScript;
	private Vector2 zeroVector = new Vector2(0,0);
    private float rEdge, lEdge, botEdge;  //Scene bound values
	private float cartPosition, curPos, lastPos;
    private char teeterSide;

    // Use this for initialization
    void Start()
    {
        saw = gameObject.transform;
		rigidB = GetComponent<Rigidbody2D> ();
        controlScript = control.GetComponent<Controller>();
		scrollBar = GameObject.Find ("Control Image Scrollbar").GetComponent<Scrollbar>();
		scrollRect = GameObject.Find ("Scroll View").GetComponent<ScrollRect>();
		baseImage = GameObject.Find ("Seesaw Base Image");
		cartScript = baseImage.GetComponent<SeesawCartWheels> ();
        sawObjL = Instantiate(sawImageL, new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + .045f), Quaternion.identity); //Create the seesaw image prefabs
        sawObjR = Instantiate(sawImageR, new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + .045f), Quaternion.identity);
        sawObjL.transform.parent = saw; //Assign as children of teeterbase 
        sawObjR.transform.parent = saw;
        source = GetComponent<AudioSource>();
		handle = GameObject.Find ("Handle");
		controlEmpty = GameObject.Find ("Control Empty 1").transform;
		dudeOnSaw = controlScript.attachedDude.gameObject;
		if (dudeOnSaw != null) {
			bouncerScript = dudeOnSaw.GetComponent<Bouncer> ();
		}
        teeterSide = 'l';
		//rEdge = 3.275f;
		rEdge = 3.18f;
		//lEdge = -3.279f;
		lEdge = -3.18f;
        botEdge = .93f;
    }

    // Update is called once per frame
    void Update()
    {
		if(controlScript == null){
			controlScript = control.GetComponent<Controller>();
		}

		//Update the position of the seesaw and rider
        if (!controlScript.gameOver)
        {
			if (dudeOnSaw != null && bouncerScript != null)
            {
                placeDude(); //Position the rider to stand on the seesaw
            }
            else
            {
				dudeOnSaw = controlScript.attachedDude;
				bouncerScript = dudeOnSaw.GetComponent<Bouncer> ();
            }
            checkBounds(); //Make sure the seesaw remains inside the bounds of the game
        }
        else //Keeps dude on saw from falling when other dude dies 
        {
			//dudeOnSaw.GetComponent<Bouncer>().setTeeterCol(true);
			bouncerScript.setTeeterCol(true);
        }

		calcVelocity ();
		if(SmScript.gameManager.showingAd){ //Stop music playing before an ad
			if(source.isPlaying){
				source.Stop ();
			}
		}
    }

    //When a dude hits the teeter
    void OnCollisionEnter2D(Collision2D col)
    {
		if (col.gameObject.tag == "Dude")
        {
			if(controlScript.fallingDude.name.Equals("Duck2")){
				if(!col.gameObject.GetComponent<Bouncer> ().getHitGround ()){ //Only interact with ducks that are not sitting on the ground
					bounceDude (col);
				}else{
					Physics2D.IgnoreCollision (GetComponent<EdgeCollider2D> (), col.collider);
				}
			}else{ //Collision happened, bounce the dude
				bounceDude (col); //Apply the force to the opposite dude
			}
        }
    }

	//Position the riding dude at the correct location on the seesaw
    void placeDude()
    {
        if (teeterSide == 'l') //Switch the seesaw image to other side
        {
			sawObjL.SetActive(true);
            sawObjR.SetActive(false);
			bouncerScript.setPositionOnTeeter(saw.position.x, 'l'); //Position rider on the appropriate side
        }
		else //Switch the seesaw image to other side
        {
			sawObjR.SetActive(true);
			sawObjL.SetActive(false);
			bouncerScript.setPositionOnTeeter(saw.position.x, 'r'); //Position rider on the appropriate side
        }
    }

	//Check the location of the falling dude after a collision, determine which bounce value will be used on the rider
	void checkHit(Collision2D col)
	{
		if(teeterSide == 'l'){
			if(col.transform.position.x >= rightMid.transform.position.x){ //Right side, big bounce
				bouncerScript.setBounce(1);
			}else{ //Right side, small bounce
				bouncerScript.setBounce(2);
			}
			teeterSide = 'r'; //Switch the side, used for determining which seesaw image is shown and which side the rider is placed
		}else{
			if(col.transform.position.x <= rightMid.transform.position.x){ //Left side, big bounce 
				bouncerScript.setBounce(1);
			}else{ //Left side, small bounce
				bouncerScript.setBounce(2);
			}
			teeterSide = 'l'; //Switch the side, used for determining which seesaw image is shown and which side the rider is placed
		}
	}

//	//Make sure the cart on the scrolling window stays within the bounds of the game
//	public void checkScrollBoundsOLD(){
//		cartPosition = mapScrollToGame(controlEmpty.transform.position.x); //Map the value from the scrollview to the game dimensions
//		if (cartPosition >= rEdge){
//			saw.position = new Vector2 (rEdge, saw.position.y);
//			rigidB.velocity = zeroVector; //Stop movement of the teeter base
//			scrollBar.value = .2406f; //Right bound value
//			scrollRect.StopMovement(); //Stop movement of the scroll rect
//		}else if (cartPosition <= lEdge){
//			saw.position = new Vector2 (lEdge, saw.position.y);
//			rigidB.velocity = zeroVector; //Stop movement of the teeter base
//			scrollBar.value = .745f; //Left bound value
//			scrollRect.StopMovement(); //Stop movement of the scroll rect
//		}else{
//			saw.position = new Vector2 (cartPosition, saw.position.y);
//		}
//	}

	//Make sure the cart on the scrolling window stays within the bounds of the game
	public void checkScrollBounds(){
		cartPosition = mapScrollToGame(controlEmpty.transform.position.x); //Map the value from the scrollview to the game dimensions
		if (cartPosition >= rEdge){
			cartScript.ok2Roll = false;
			scrollBar.value = .2406f; //Right bound value
			scrollRect.StopMovement(); //Stop movement of the scroll rect
			baseImage.transform.position = new Vector2 (mapGameToScroll(rEdge), baseImageHeight);
			rigidB.velocity = zeroVector; //Stop movement of the teeter base
			saw.position = new Vector2 (rEdge, saw.position.y);
		}else if (cartPosition <= lEdge){
			cartScript.ok2Roll = false;
			scrollBar.value = .745f; //Left bound value
			scrollRect.StopMovement(); //Stop movement of the scroll rect
			baseImage.transform.position = new Vector2 (mapGameToScroll(lEdge), baseImageHeight);
			rigidB.velocity = zeroVector; //Stop movement of the teeter base
			saw.position = new Vector2 (lEdge, saw.position.y);
		}else{
			cartScript.ok2Roll = true;
			baseImage.transform.position = new Vector2 (controlEmpty.transform.position.x, baseImageHeight);
			saw.position = new Vector2 (cartPosition, saw.position.y);
		}
	}

	//After collision with a falling dude, bounce the other dude and set up conditions/variables for each dude
	private void bounceDude(Collision2D col){
//		ParticleSystem ps;
//		if(teeterSide == 'l'){
//			ps = Instantiate(landingPS, new Vector2(transform.position.x + .2f, transform.position.y + .1f), Quaternion.identity);
//		}else{
//			ps = Instantiate(landingPS, new Vector2(transform.position.x - .2f, transform.position.y + .1f), Quaternion.identity);
//		}
//		Destroy (ps, .4f);

		checkHit (col); //Saw 1/2

		if (!controlScript.gameOver) {
			bouncerScript.setAttached ('l', false);
			bouncerScript.setAttached ('r', false);
			bouncerScript.setParent (null);
			bungieScript.dude = dudeOnSaw;
			dudeOnSaw = col.gameObject;
			bouncerScript = dudeOnSaw.GetComponent<Bouncer> ();
			bouncerScript.setParent (saw);
			bouncerScript.setTeeterCol (false);
			bouncerScript.setBalloonCol (false);
			bouncerScript.switchPlatformCollider (false);
			if (teeterSide == 'l') {
				bouncerScript.setAttached ('l', true);
			} else {
				bouncerScript.setAttached ('r', true);
			}
			source.PlayOneShot (bounceSound, .75f); // Play the bounce sound
		}
	}

    //Make sure the teeter stays in the bounds of the game
    void checkBounds()
    {
        if (saw.position.x > rEdge)
        {
            saw.position = new Vector2(rEdge, saw.position.y);
        }
        else if (saw.position.x < lEdge)
        {
            saw.position = new Vector2(lEdge, saw.position.y);
		}
		else if (saw.position.y < botEdge)
        {
            saw.position = new Vector2(saw.position.x, botEdge);
        }
    }

	//Mapping function, maps from scrollview position to seesaw game position
	public float mapScrollToGame(float scrollPosition){
		return (scrollPosition - scrollMin.transform.position.x) * (gameMax.transform.position.x - gameMin.transform.position.x) / (scrollMax.transform.position.x - scrollMin.transform.position.x) + gameMin.transform.position.x;
	}

	public float mapGameToScroll(float gamePosition){
		return (gamePosition - gameMin.transform.position.x) * (scrollMax.transform.position.x - scrollMin.transform.position.x) / (gameMax.transform.position.x - gameMin.transform.position.x) + scrollMin.transform.position.x;
	}

	public Transform getControlEmpty(){
		return controlEmpty;
	}

	private void calcVelocity(){
		curPos = transform.position.x;
		float distance = curPos - lastPos;
		velocity = distance / Time.deltaTime;
		lastPos = curPos;

		if(velocity >= 10){
			velocity = 10;
		}
	}

	public void playPowerUpSound(AudioClip clip){
		source.PlayOneShot (clip, .75f);
	}
	public void playPowerUpSound(AudioClip clip, float volume){
		source.PlayOneShot (clip, volume);
	}

	public char getTeeterSide(){
		return teeterSide;
	}
}

