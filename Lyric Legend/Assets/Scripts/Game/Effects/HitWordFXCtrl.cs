using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HitWordFXCtrl : MonoBehaviour {

	Vector3 startScale = Vector3.zero;
	Vector3 endScale = new Vector3(1,1,1);
	Animator anim;
	// Use this for initialization
	void Awake(){
		anim = this.gameObject.GetComponent<Animator>();
		this.gameObject.SetActive(false);
	}


	void OnEnable()
	{
		runAnim();
	}

	void runAnim()
	{
		anim.SetBool("wordIsHit", true);
		//this.gameObject.transform.localScale = startScale;
		//this.gameObject.transform.DOScale(endScale, 0.2f).SetEase(Ease.Linear).OnComplete(OnAnimDone);
	}

	public void OnAnimDone()
	{
		anim.SetBool("wordIsHit", false);
		PoolManager.ReleaseObject(this.gameObject);
		this.gameObject.SetActive(false);
	}
}
