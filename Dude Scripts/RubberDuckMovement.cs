using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Duck Team movement control/interaction script
public class RubberDuckMovement : Bouncer {
	public AudioClip duckSound1, duckSound2;
	public bool bounce1, bounce2, movingUp, switchCollider, attachedL, attachedR, hitGround, greenPowerUpBool, attachVortex, alreadyVortex;
	public float bounceVal1, bounceVal2, yPrev;
	private GameObject dude, teeter, fText;
	private Rigidbody2D rigidB;
	private BoxCollider2D dudeTeeterCol;
	private CapsuleCollider2D dudeBalloonCol;
	private Controller controlScript;
	private Animator anim;
	private AudioSource source;
	private Vector2 zeroVector = new Vector2(0,0);
	private bool duckCollision, bigMode, growing, shrinking, collided, ignoringTeeter;
	private float topEdge, lEdge, rEdge, bEdge, lenStandX, lenStandY, localPosX;
	private float duckHeight = .039f;

	//Initiallize conditions if dude is riding the seesaw
	public override void setupAttached(string name, Transform teeter){
		gameObject.name = name;
		gameObject.transform.parent = teeter;
		if(dudeTeeterCol == null){
			dudeTeeterCol = gameObject.GetComponent<BoxCollider2D>();
		}
		dudeTeeterCol.enabled = false;
		attachedL = true;
		anim.SetInteger("State", 1);
	}

	public override void setupFalling(string name){
		gameObject.name = name;
		determineDuckSkin(name);
		switchCollider = true;
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
		yPrev = dude.transform.position.y; //Used to determine the direction of dude movement in the Y direction
		dudeBalloonCol.enabled = false;
		ignoringTeeter = false;
		collided = false;
		bigMode = false;
		greenPowerUpBool = false;
		growing = false;
		shrinking = false;
		duckCollision = false;
		bounce1 = false;
		bounce2 = false;
		hitGround = false;
		topEdge = 5.09f;
		lEdge = -3.788f;
		rEdge = 3.788f;
		bEdge = .895f;
		duckHeight = .039f;
		lenStandX = .691f;
		lenStandY = .954f;
	}

	// Update is called once per frame
	public override void Update () {
		if(controlScript == null){
			controlScript = GameObject.Find("Controller").GetComponent<Controller>();
		}
		if(bigMode){
			if(growing){
				if (transform.localScale.x < .7f) {
					transform.localScale += new Vector3 (.05f, .05f, 1);
				}else{
					transform.localScale = new Vector3 (.7f, .7f, 1);
					growing = false;
				}
			}else if(shrinking){
				if (transform.localScale.x > .4f) {
					transform.localScale -= new Vector3 (.05f, .05f, 1);
				}else{
					transform.localScale = new Vector3 (.4f, .4f, 1);
					shrinking = false;
					bigMode = false;
				}
			}else{
				transform.localScale = new Vector3 (.7f, .7f, 1);
			}
		}else{
			transform.localScale = new Vector3 (.4f, .4f, 1);
		}

		if (attachedL) //Dude is on left side of teeter
		{
			if (bigMode) { //Dude is bigger, adjust placement on the seesaw
				localPosX = -.342f;
				if(name.Equals("Duck2")){
					duckHeight = .146f;
				}else if(name.Equals("Duck4")){
					duckHeight = .126f;
				}else if(name.Equals("Duck5")){
					duckHeight = .139f;
				}else if(name.Equals("Duck8")){
					duckHeight = .127f;
				}else if(name.Equals("Duck9")){
					duckHeight = .131f;
				}else{
					duckHeight = .104f;
				}
			} else {
				localPosX = -.418f;
				if(name.Equals("Duck2")){
					duckHeight = .06f;
				}else if(name.Equals("Duck4")){
					duckHeight = .048f;
				}else if(name.Equals("Duck5")){
					duckHeight = .059f;
				}else if(name.Equals("Duck8")){
					duckHeight = .05f;
				}else if(name.Equals("Duck9")){
					duckHeight = .052f;
				}else{
					duckHeight = .04f;
				}
			}

			dude.transform.localPosition = new Vector2(localPosX, duckHeight);
			rigidB.velocity = zeroVector;
			dude.transform.rotation = Quaternion.Euler (0, 0, 9); //Rotate dude to correctly stand on the seesaw
		}
		else if (attachedR) //Dude is on right side of teeter
		{
			if (bigMode) { //Dude is bigger, adjust placement on the seesaw
				localPosX = .386f;
				if (name.Equals ("Duck2")) {
					duckHeight = .139f;
				} else if (name.Equals ("Duck4")) {
					duckHeight = .117f;
				} else if (name.Equals ("Duck5")) {
					duckHeight = .131f;
				} else if (name.Equals ("Duck8")) {
					duckHeight = .118f;
				} else if (name.Equals ("Duck9")) {
					duckHeight = .126f;
				} else {
					duckHeight = .101f;
				}
			}else{
				localPosX = .44f;
				if (name.Equals ("Duck2")) {
					duckHeight = .06f;
				} else if (name.Equals ("Duck4")) {
					duckHeight = .047f;
				} else if (name.Equals ("Duck5")) {
					duckHeight = .056f;
				} else if (name.Equals ("Duck8")) {
					duckHeight = .045f;
				} else if (name.Equals ("Duck9")) {
					duckHeight = .053f;
				} else {
					duckHeight = .039f;
				}
			}
			dude.transform.localPosition = new Vector2(localPosX, duckHeight);
			rigidB.velocity = zeroVector;
			dude.transform.rotation = Quaternion.Euler (0, 0, -9); //Rotate dude to correctly stand on the seesaw
		}
		else //Dude is falling
		{
			if (!gameObject.GetComponent<RubberDuckMovement> ().getHitGround ()) {
				dude.transform.rotation = Quaternion.Euler (0, 0, 0); //Reset dudes rotation to zero

				//Determine if dude is moving up or down on the screen
				if (dude.transform.position.y >= yPrev) {
					movingUp = true;
				} else {
					movingUp = false;
				}
				yPrev = dude.transform.position.y;

				//Turn on colliders, or turn off capsule below platforms
				if (dude.transform.position.y > 1.6f) {
					dudeTeeterCol.enabled = true;
					dudeBalloonCol.enabled = true;
				} else {
					dudeBalloonCol.enabled = false;
				}
			}else{
				float offsetHeight;
				if (controlScript.fallingDude.GetComponent<Bouncer> ().inBigMode ()) {
					offsetHeight = getDeathOffset ();
					gameObject.transform.position = new Vector2 (gameObject.transform.position.x, offsetHeight);

				} else {
					if (name.Equals ("Duck2")) {
						duckHeight = .021f;
					} else if (name.Equals ("Duck5")) {
						duckHeight = .017f;
					} else if (name.Equals ("Duck9") || name.Equals ("Duck8")) {
						duckHeight = .011f;
					} else if (name.Equals ("Duck4")) {
						duckHeight = .008f;
					} else {
						duckHeight = 0f;
					}
					gameObject.transform.position = new Vector2 (gameObject.transform.position.x, bEdge + duckHeight);
				}
			}
		}
		if(ignoringTeeter){
			Physics2D.IgnoreCollision (teeter.GetComponentInChildren<EdgeCollider2D> (), GetComponent<CapsuleCollider2D> ());
		}
		updateVector(); //Determine bounce value
		checkBounds(); //Make sure the dude is still inside bounds of scene
		if(SmScript.gameManager.showingAd){ //Stop music playing before an ad
			if(source.isPlaying){
				source.Stop ();
			}
		}
	}

	//If dude hits the ground he dies
	public override void OnCollisionEnter2D(Collision2D col){
		if (col.gameObject.tag == "Ground")
		{
			if (!hitGround) { //Prevent repeat collisions with same object 
				hitGround = true;
				source.PlayOneShot (duckSound2, .25f); //Play alternate duck squeek audio sound
				controlScript.ducksDead++;
				controlScript.deadDucks.Enqueue (gameObject);
				dudeBalloonCol.enabled = true;
				gameObject.layer = 12;
				Vector2 tempVector = new Vector2 (0, 0);
				rigidB.velocity = tempVector;
				float yOffset;
				if (controlScript.fallingDude.GetComponent<Bouncer> ().inBigMode ()) {
					yOffset = getDeathOffset ();
				} else {
					yOffset = bEdge;
				}
				transform.position = new Vector2 (transform.position.x, yOffset);
				if (controlScript.ducksDead > (controlScript.duckCount - 2)) { //If all but the attached duck has died, game is over
					controlScript.gameOver = true; //Signal that the dude has died and game is over
					controlScript.animationDone = true; //Signal that the animation is ready for game pause
				}
				if (gameObject.transform.parent == null) {
					GetComponent<SpriteRenderer> ().sortingOrder = 11; //Draw dead dudes in front of the seesaw and rider for clarity near ground
				}
				if (!collided) {
					collided = true;
					fText = PoolingManager.pooler.getPooledObject (7);
					fText.SetActive (true);
					fText.GetComponent<FloatingText> ().createDeathText (transform.position, 1);
					Invoke ("resetText", 1.4f);
					//ftScript.createDeathText (transform.position, 1); 
				}
			}
		}
		if(col.gameObject.tag == "Dude"){ //Duck on duck collision
			if (!attachedL && !attachedR) {
				if (!col.gameObject.GetComponent<Bouncer>().duckIsAttached()) {
					if (!duckCollision) { //Prevent repeat collisions with same object
						duckCollision = true;
						source.PlayOneShot (duckSound1, .25f); //Play duck squeek audio sound

						//Determine the appropriate force to apply to the dude
						//Each condition sets the force to create a natural bounce for each duck, there are multiple positional situations that change how the force should be applied
						Vector2 bounceOffDuckForce;
						if (rigidB.velocity.x > col.gameObject.GetComponent<Rigidbody2D> ().velocity.x) {
							bounceOffDuckForce = new Vector2 (1f, .5f);
						} else if (rigidB.velocity.x < col.gameObject.GetComponent<Rigidbody2D> ().velocity.x) {
							bounceOffDuckForce = new Vector2 (-1f, .5f);
						} else {
							if (rigidB.velocity.y < col.gameObject.GetComponent<Rigidbody2D> ().velocity.y) {
								bounceOffDuckForce = new Vector2 (-1f, -.1f);
							} else if (rigidB.velocity.y > col.gameObject.GetComponent<Rigidbody2D> ().velocity.y) {
								bounceOffDuckForce = new Vector2 (1f, .5f);
							} else {
								if (dude.transform.position.x < col.transform.position.x) {
									bounceOffDuckForce = new Vector2 (-1f, -.1f);
								} else if (dude.transform.position.x > col.transform.position.x) {
									bounceOffDuckForce = new Vector2 (1f, .5f);
								} else {
									if (dude.transform.position.y <= col.transform.position.y) {
										bounceOffDuckForce = new Vector2 (-1f, -.1f);
									} else {
										bounceOffDuckForce = new Vector2 (1f, .5f);
									}
								}
							}
						}
						rigidB.velocity = bounceOffDuckForce; //Apply the force
						Invoke ("resetDuckCollision", .3f); //Wait, then allow for another duck collision, prevents repeat collisions with the same duck
					}
				}
			}
		}
		if(col.gameObject.tag == "Boundary Collider"){
			source.PlayOneShot (duckSound2, .25f); //Play alternate duck squeek audio sound
		}
		if(col.gameObject.tag == "Explosion" || col.gameObject.tag == "PowerUp"){
			Physics2D.IgnoreCollision (GetComponent<BoxCollider2D> (), col.collider);
			Physics2D.IgnoreCollision (GetComponent<CapsuleCollider2D> (), col.collider);
		}
		if(col.gameObject.tag == "Teeter(Player)"){
			if (hitGround) {
				ignoringTeeter = true;
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
				dude.transform.position = new Vector2 (sawPositionX - lenStandX, lenStandY);
			} else {
				dude.transform.position = new Vector2 (sawPositionX + lenStandX, lenStandY);
			}
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
	public void pauseAfterDeadAnimationDuck(){ //Callback after dead animation has completed, needed so dead animation is shown before pausing the game
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
		//Dont change duck animation, static animation set when duck is instantiate to allow for different duck skins
		//anim.SetInteger ("State", value);
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
	public override bool getHitGround(){ //Return value on whether the dude has hit the ground or not
		return hitGround;
	}

	void resetDuckCollision(){ //Reset value to allow for new duck collisions to occur
		duckCollision = false;
	}

	void turnOnColliders(){ //Turn on both of the dudes colliders
		dudeTeeterCol.enabled = true;
		dudeBalloonCol.enabled = true;
	}

	public void playSound(){ //Play the duck squeek audio sound
		source.PlayOneShot (duckSound1, .25f);
	}

	//Determine which animation skin should be used for the different ducks
	void determineDuckSkin(string name){
		if(name.Equals("Duck2")){
			anim.SetInteger("State", 2);
		}else if(name.Equals("Duck3")){
			anim.SetInteger("State", 3);
		}else if(name.Equals("Duck4")){
			anim.SetInteger("State", 4);
		}else if(name.Equals("Duck5")){
			anim.SetInteger("State", 5);
		}else if(name.Equals("Duck6")){
			anim.SetInteger("State", 6);
		}else if(name.Equals("Duck7")){
			anim.SetInteger("State", 7);
		}else if(name.Equals("Duck8")){
			anim.SetInteger("State", 8);
		}else if(name.Equals("Duck9")){
			anim.SetInteger("State", 9);
		}else if(name.Equals("Duck10")){
			anim.SetInteger("State", 10);
		}else if(name.Equals("Duck11")){
			anim.SetInteger("State", 11);
		}else if(name.Equals("Duck12")){
			anim.SetInteger("State", 12);
		}else{
			anim.SetInteger ("State", 0);
		}
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

	private float getDeathOffset(){
		if(name.Equals("Duck2")){
			return .974f;
		}else if(name.Equals("Duck5")){
			return .97f;
		}else if(name.Equals("Duck9") || name.Equals("Duck8")){
			return .957f;
		}else if(name.Equals("Duck4")){
			return .956f;
		}else {
			return .935f;
		}
	}

	public override bool duckIsAttached(){
		if(attachedL){
			return true;
		}
		if(attachedR){
			return true;
		}
		return false;
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

	public void delayLayerChange(){
		Invoke ("changeLayer", .2f);
	}
	private void changeLayer(){
		gameObject.layer = 8;
	}

	public void tornadoReset(){
		hitGround = false;
		collided = false;
		dudeTeeterCol.enabled = true;
		GetComponent<SpriteRenderer> ().sortingOrder = 5;
		gameObject.layer = 13;
	}
}
