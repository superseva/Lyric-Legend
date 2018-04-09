using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CrowdFX : MonoBehaviour {

    public int band;
    public float startScale, endScale, scaleMultiplier, trashold, time;
    float newScale;


    Tween myTweenAlpha;
    Tween myTweenScale;

	// Use this for initialization
	void Start () {
        myTweenAlpha = transform.GetComponent<Renderer>().material.DOFade(0, 0);
        transform.localScale = Vector3.one;
	}


    void Update()
    {
       if (AudioPeer.bandBuffer[band] > trashold)
        {
            if(!myTweenAlpha.IsPlaying()){
                transform.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 1);
                myTweenAlpha = transform.GetComponent<Renderer>().material.DOFade(0, time);
                myTweenScale.Kill();
                transform.localScale = Vector3.one;
                //myTweenScale.SetLoops(1, LoopType.Yoyo);
                endScale = (AudioPeer.bandBuffer[band] * scaleMultiplier) + startScale;
                myTweenScale = transform.DOScale(new Vector3(endScale, endScale, endScale), time).SetLoops(2, LoopType.Yoyo);
            }


        }
    }
}
