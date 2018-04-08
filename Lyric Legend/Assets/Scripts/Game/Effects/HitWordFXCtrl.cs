using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HitWordFXCtrl : MonoBehaviour {

	Vector3 startScale = Vector3.zero;
	Vector3 endScale = new Vector3(1,1,1);
	Animator anim;

    public AnimationClip burstAnim;
    public AnimationClip idleAnim;
	// Use this for initialization
	void Awake(){
		anim = this.gameObject.GetComponent<Animator>();
		this.gameObject.SetActive(false);
	}


	void OnEnable()
	{
		runAnim();
	}

	//private void OnDisable()
	//{
 //       CancelInvoke();
	//}

	void runAnim()
	{
		anim.SetBool("wordIsHit", true);
        float animtime = burstAnim.length;
        //Invoke("OnAnimDone", animtime);
	}

	public void OnAnimDone()
	{
		anim.SetBool("wordIsHit", false);
		PoolManager.ReleaseObject(this.gameObject);
		this.gameObject.SetActive(false);
	}

    //public void OnAnimComplete(){
    //    anim.SetBool("wordIsHit", false);
    //    PoolManager.ReleaseObject(this.gameObject);
    //    this.gameObject.SetActive(false);
    //}
}
