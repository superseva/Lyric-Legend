using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAudioCtrl : MonoBehaviour {

    public int band, emitCount;
    public float startScale, scaleMultiplier, trashold;


    private ParticleSystem particleSystem;

	// Use this for initialization
	void Start () {
        particleSystem = GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
        //Debug.Log(AudioPeer.bandBuffer[band]);
        if(AudioPeer.bandBuffer[band]>trashold){
            Debug.Log(AudioPeer.bandBuffer[band]);
            particleSystem.Emit(emitCount);
        }
	}
}
