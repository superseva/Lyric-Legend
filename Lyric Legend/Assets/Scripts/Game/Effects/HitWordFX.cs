using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitWordFX : MonoBehaviour {

    private Animator animator;
    public bool perfect = false;

	private void Awake()
	{
        animator = GetComponent<Animator>();
        gameObject.SetActive(false);
	}

	private void OnEnable()
	{
        Invoke("Destroy", 0.6f);
	}
	private void Destroy()
	{
        PoolManager.ReleaseObject(this.gameObject);
        gameObject.SetActive(false);
	}

	private void OnDisable()
	{
        CancelInvoke();
	}

	public void runAnim(){
        if (perfect)
            animator.SetTrigger("perfect");
        else
            animator.SetTrigger("nonperfect");
    }

}
