using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BandLight : MonoBehaviour {

    public int band;
    public float startScale, scaleMultiplier;
    public float[] rgb = new float[3];

    private Material material;


	// Use this for initialization
	void Start () {
        material = GetComponent<Renderer>().material;
        //material.SetColor("_TintColor", new Color(128, 128, 128, 0));
        //Debug.Log(material.HasProperty("_MainTex"));
	}
	
	// Update is called once per frame
    float newScale;
	void Update () {
        newScale = (AudioPeer.bandBuffer[band] * scaleMultiplier) + startScale;
        material.SetColor("_TintColor", new Color(rgb[0], rgb[1], rgb[2], newScale));
	}
}
