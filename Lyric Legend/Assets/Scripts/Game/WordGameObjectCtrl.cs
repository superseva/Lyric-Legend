using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WordGameObjectCtrl : MonoBehaviour {

	[HideInInspector]
	public WordData wordData;

	public GameObject textMeshGO;
	public TextMeshPro textMesh;
	public float showTime;
	public float hitTime;
	public float existanceTime = 1f;
	public Vector3 startPosition;
	public Vector3 endPosition;
	public float hitOffset=0.2f;
	public float holdDuration = 0;

	void Awake()
	{
		textMesh = textMeshGO.GetComponent<TextMeshPro>();
	}

	private void OnEnable()
	{
		Invoke("Destroy", existanceTime + hitOffset + holdDuration);
	}
	private void Destroy()
	{
		gameObject.SetActive(false);
	}

	private void OnDestroy()
	{
		CancelInvoke();
	}

	public void SetData(WordData wd){
		wordData = wd;
		textMesh.text = wordData.text;
		showTime = float.Parse(wordData.time) - existanceTime;
		hitTime = float.Parse(wordData.time);
		holdDuration = (float)wordData.duration;
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
}
