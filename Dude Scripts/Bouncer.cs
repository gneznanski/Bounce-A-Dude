using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Parent of dude scripts, methods meant to be overriden in individual scripts
//Heirarcy ensures new dude prefabs are handled the same by other game objects
public class Bouncer : MonoBehaviour {
	public bool movingUpBouncer, attachedBouncer, colliderSwitch, hitGroundBouncer, bigModeBouncer, duckAttached, greenPowerUpBouncer;
	public int animState;
	public virtual void Awake(){
	}
	public virtual void Start (){
	}
	public virtual void Update(){
	}
	public virtual void OnCollisionEnter2D(Collision2D col){
	}
	public virtual void updateVector(){
	}
	public virtual void checkBounds(){
	}
	public virtual void setTeeterCol(bool active){
	}
	public virtual void setAttached(char side, bool active){
	}
	public virtual bool isAttached(){
		return attachedBouncer;
	}
	public virtual void setParent(Transform parent){
	}
	public virtual void setBalloonCol(bool active){
	}
	public virtual void setPositionOnTeeter(float sawPositionX, char side){
	}
	public virtual void setBounce(int bounceNum){
	}
	public virtual void setupAttached(string name, Transform teeter){
	}
	public virtual void setupFalling(string name){
	}
	public virtual bool isMovingUp(){
		return movingUpBouncer;
	}
	public virtual void setAnim(int value){
	}
	public virtual int getAnim(){
		return animState;
	}
	public virtual void switchPlatformCollider(bool active){
	}
	public virtual bool isPlatformColliderSwitched(){
		return colliderSwitch;
	}
	public virtual bool getHitGround(){ //Duck Team specific method, used to see whether the duck is alive or dead on the ground for bouncing purposes
		return hitGroundBouncer;
	}
	public virtual void toggleBigMode(bool toggle){
	}
	public virtual bool inBigMode(){
		return bigModeBouncer;
	}
	public virtual bool duckIsAttached(){
		return duckAttached;
	}
	public virtual bool getGreenPowerUp(){
		return greenPowerUpBouncer;
	}
	public virtual void setGreenPowerUp(bool toggle){
	}
}
