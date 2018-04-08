using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCubeCtrl : MonoBehaviour {

    public int band;
    public float startScale, scaleMultiplier;

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    float newScale;
	void Update () {
        newScale = (AudioPeer.bandBuffer[band] * scaleMultiplier) + startScale;
        transform.localScale = new Vector3(newScale, newScale, newScale);
        //transform.localScale = new Vector3(transform.localScale.x, (AudioPeer.freqBand[band] * scaleMultiplier)+ startScale, transform.localScale.z);
	}
}
