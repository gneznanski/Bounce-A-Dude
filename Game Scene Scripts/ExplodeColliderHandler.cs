using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeColliderHandler : MonoBehaviour {
	public GameObject target, explodePair;
	public ParticleSystem subEmit;
	public AudioClip bombSound;
	public bool soundPlayed, finished, remove, waitingOnCoroutine;
	public int row, index;
	private AudioSource source;
	private Animator anim;
	private bool exploding;

	void Start () {
		source = GetComponent<AudioSource> ();
		soundPlayed = false;
	}
	void Update () {
		if(source != null && exploding){
			if (!soundPlayed)
			{
				source.PlayOneShot(bombSound, .4f); //Play the explosion sound
				soundPlayed = true;
				anim = GameObject.Find ("Main Camera").GetComponent<Animator> ();
				anim.SetBool ("Shake", true); //Shake the camera during the explosion
			}
		}
		if(SmScript.gameManager.showingAd){ //Stop music playing before an ad
			if(source.isPlaying){
				source.Stop ();
			}
		}
	}

	//Ignore collisions with the Dude and the boundary colliders
	public void OnCollisionEnter2D(Collision2D col){
		if (col.gameObject.tag == "Boundary Collider" || col.gameObject.tag == "Dude") {
			Physics2D.IgnoreCollision (GetComponent<BoxCollider2D> (), col.collider);
		}
	}

	public void playSound(){
		exploding = true;
	}
}
