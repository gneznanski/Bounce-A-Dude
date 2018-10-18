using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonAnchor : MonoBehaviour {
    public GameObject begin, target;
    public int speed;

	void Update () {
		//Move the balloon across the screen, or reset position after reaching end of path
        if (transform.position != target.transform.position){
            transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
        }else {
            transform.position = begin.transform.position;
        }
    }

    public void destroyClone()
    {
        Destroy(gameObject);
    }
}
