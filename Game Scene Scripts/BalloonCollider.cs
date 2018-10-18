using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonCollider : MonoBehaviour {
	public GameObject control, anchor, powerUpPrefab, bombCollider;
	public Animator anim;
	public AudioClip balloonPopSound;
	public Color color;
	public int pointValue, row, index;
	public bool popped, collided, bomb, alreadyOff, poppedByExplosion;
	public bool updated = false;
	private ParticleSystem particle;
    private Controller controlScript;
    private BalloonMovement balMove;
	private SpriteRenderer spriteRender;
	private CircleCollider2D cirCol;
    private AudioSource source;
	private GameObject fText;
	private float lEdge = -3f;
	private float rEdge = 3f;

    // Use this for initialization
    void Start () {
        control = GameObject.Find("Controller");
        controlScript = control.GetComponent<Controller>();
        balMove = control.GetComponent<BalloonMovement>();
        source = GetComponent<AudioSource>();
		collided = false;
		spriteRender = GetComponent<SpriteRenderer> ();
		cirCol = GetComponent<CircleCollider2D> ();
    }

    // Update is called once per frame
    void Update() {
		if(control == null){
			control = GameObject.Find("Controller");
			controlScript = control.GetComponent<Controller>();
		}
		if (balMove == null)
        {
            balMove = control.GetComponent<BalloonMovement>();
        }

		//Set up variables from playerprefs for the balloon
        if (!updated)
        {
            name = "Row" + row + "Balloon" + index; //Set up correct name for saving purposes
            if (PlayerPrefs.HasKey(name)) //Load from playerprefs
            {
                if (PlayerPrefs.GetInt(name) == 1)
                {
                    popped = false;
					turnOn ();
                }
                else
                {
                    popped = true;
					turnOff ();
					alreadyOff = true;
                }
                updated = true;
            }
            else //New game
            {
                PlayerPrefs.SetInt(name, 1);
                popped = false;
                updated = true;
            }
        }

		if (spriteRender == null || cirCol == null) {
			spriteRender = GetComponent<SpriteRenderer> ();
			cirCol = GetComponent<CircleCollider2D> ();
		} else {
			if (!alreadyOff) {
				if(popped){
					turnOff ();
					alreadyOff = true;
				}
			}
		}
		if(SmScript.gameManager.showingAd){ //Stop music playing before an ad
			if(source.isPlaying){
				source.Stop ();
			}
		}
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
		if (col.transform.tag == "Dude")
        {
			if (!collided) { //Avoid repeat collisions with same dude
				popBalloon();
			}
        }

		//Ignore colliders on edges
		if (col.transform.tag == "Boundary Collider"){
			Physics2D.IgnoreCollision (GetComponent<CircleCollider2D> (), col.collider);
		}
		if(col.transform.tag == "Explosion"){
			controlScript.numBalPopPreExplode++;
			poppedByExplosion = true;
			if (bomb) {
				col.gameObject.GetComponent<BoxCollider2D> ().enabled = false;
			}
			if (!collided) { //Avoid repeat collisions with same dude
				popBalloon();
			}
		}
    }

    public void turnOn()
    {
		spriteRender.enabled = true;
		cirCol.enabled = true;
    }

	public void turnOnSafe(){
		if(spriteRender == null){
			spriteRender = GetComponent<SpriteRenderer> ();
		}
		if(cirCol == null){
			cirCol = GetComponent<CircleCollider2D> ();
		}
		turnOn ();
	}

    public void turnOff()
    {
		spriteRender.enabled = false;
		cirCol.enabled = false;
    }

	public void popBalloon(){
		collided = true;
		turnOff ();
		source.PlayOneShot (balloonPopSound, .75f);
		if (!poppedByExplosion) {
			particle = PoolingManager.pooler.getPooledObject (6).GetComponent<ParticleSystem> ();
			particle.gameObject.SetActive (true);
			ParticleSystem.MainModule ma = particle.main;
			ma.startColor = color;
			particle.transform.position = transform.position;
			particle.Play ();
			controlScript.delayedResetPopEffect(particle.gameObject); 
		}
		controlScript.updateScore (pointValue);
		popped = true;
		PlayerPrefs.SetInt (name, 0);
		balMove.addPopped (row);
		fText = PoolingManager.pooler.getPooledObject (7);
		fText.SetActive (true);
		if (bomb) {
			controlScript.explode (gameObject.transform.parent.gameObject.transform.position.x, gameObject.transform.parent.gameObject.transform.position.y, index, row);
			fText.GetComponent<FloatingText>().createPopText (new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + .2f), pointValue);
		}else{
			fText.GetComponent<FloatingText>().createPopText (gameObject.transform.position, pointValue);
			checkPowerUp ();
		}
		controlScript.delayedResetFText(fText);
	}

	public void checkPowerUp(){
		if(Random.Range (0, 15) == 0 || controlScript.powerUpCounter >= controlScript.powerUpMax){
			if (transform.position.x < lEdge) { //Make sure the powerUp is inside the game area
				transform.position = new Vector3(lEdge, transform.position.y, transform.position.z);
			} else if (transform.position.x > rEdge) {
				transform.position = new Vector3(rEdge, transform.position.y, transform.position.z);
			}
			PoolingManager.pooler.getPooledObject(Random.Range(1, 4)).GetComponent<PowerUp> ().prep (gameObject.transform.position);
			controlScript.powerUpCounter = 0;
			controlScript.powerUpMax = Random.Range (14, 22);
		}else{
			controlScript.powerUpCounter++;
		}
	}
}
