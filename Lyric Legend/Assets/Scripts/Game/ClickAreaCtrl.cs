using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickAreaCtrl : MonoBehaviour {

	public ParticleSystem particlesFx;
	[HideInInspector]
	public int fingerIdDown = 100;
	[HideInInspector]
	public WordGameObjectCtrl wordCtrl;
	[HideInInspector]
	public float durationToHold;
	[HideInInspector]
	public float startHoldTime;
	private List<WordGameObjectCtrl> colliders = new List<WordGameObjectCtrl>();


	void Start(){
		fingerIdDown = 100;
		StopParticles();
	}

	void OnDisable(){
		StopParticles();
		CancelInvoke();
	}

	void OnTriggerEnter2D(Collider2D coll) 
	{
		if (coll.gameObject.tag == "WordCollider"){
			wordCtrl = coll.gameObject.GetComponent<WordGameObjectCtrl>();
			if(!colliders.Contains(wordCtrl)){
				colliders.Add(wordCtrl);
			}
		}
	}

	void OnTriggerExit2D(Collider2D coll)
	{
		if (coll.gameObject.tag == "WordCollider"){
			wordCtrl = coll.gameObject.GetComponent<WordGameObjectCtrl>();
            if(!wordCtrl.isClicked && !wordCtrl.isFake){
                ScoreCtrl.WordMiss();
            }
			if(colliders.Contains(wordCtrl)){
				colliders.Remove(wordCtrl);
			}
		}
	}

	private WordGameObjectCtrl wgCtrl;
	public WordGameObjectCtrl GetCollidingWord(){
		if(colliders.Count > 0){
			for(int i =0; i < colliders.Count; i++){
				wgCtrl = colliders[i];
				if(!wgCtrl.isClicked){
					wgCtrl.isClicked = true;
					return wgCtrl;
				}
			}
			return null;
		}else{
			return null;
		}
	}

	public void StartHolding(int fId, float dHold, float startH){
		fingerIdDown = fId;
		durationToHold = dHold;
		startHoldTime = startH;
		Invoke("TerminateHolding", dHold);
		particlesFx.transform.position = this.gameObject.transform.position;
		particlesFx.gameObject.SetActive(true);
	}

	public void TerminateHolding(){
		particlesFx.Stop();
		fingerIdDown = 100;
		durationToHold = 0;
		startHoldTime = 0;
		StopParticles();
	}

	public void StopParticles(){
        if(particlesFx!=null)
		    particlesFx.gameObject.SetActive(false);
	}
}
