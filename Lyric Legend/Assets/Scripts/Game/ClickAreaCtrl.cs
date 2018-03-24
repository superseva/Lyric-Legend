using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickAreaCtrl : MonoBehaviour {

	public bool isColliding = false;
	public WordGameObjectCtrl wordCtrl;

	private GameObject word;

	void Start()
	{
		isColliding = false;
	}

	void OnTriggerEnter2D(Collider2D coll) {
		//Debug.Log("HIT");
		if (coll.gameObject.tag == "WordCollider"){
			word = coll.gameObject;
			wordCtrl = word.GetComponent<WordGameObjectCtrl>();
			isColliding = true;
			//Debug.Log(Time.time + " :: Colided with word : " + wordCtrl.orderIndex);
		}
	}
	void OnTriggerExit2D(Collider2D coll)
	{
		if (coll.gameObject.tag == "WordCollider"){
			word = coll.gameObject;
			wordCtrl = word.GetComponent<WordGameObjectCtrl>();
			//Debug.Log(Time.time + " :: Exit collision with word : " + wordCtrl.orderIndex);
			isColliding = false;

		}
	}

}
