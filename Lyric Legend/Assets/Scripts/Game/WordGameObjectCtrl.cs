using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class WordGameObjectCtrl : MonoBehaviour {
	
	public GameObject textMeshGO;
	public float hitOffset=0.2f;
	public GameObject trail;
	[HideInInspector]
	public TextMeshPro textMesh;
	[HideInInspector]
	public WordData wordData;
	[HideInInspector]
	public float showTime;
	[HideInInspector]
	public float hitTime;
	[HideInInspector]
    public float timeOnScreen = 1f;
	[HideInInspector]
	public Vector3 startPosition;
	[HideInInspector]
	public Vector3 endPosition;
	[HideInInspector]
	public float holdDuration = 0;
	[HideInInspector]
	public float distanceToTravel;
	[HideInInspector]
	public bool holdGraphicTweening = false;
	[HideInInspector]
	public int orderIndex = 0; // used for keeping streaks increment. starts from 1
	[HideInInspector]
	public bool isClicked = false;
    [HideInInspector]
    public int xPositionIndex;
    [HideInInspector]
    public bool isFake = false;

	void Awake()
	{
		textMesh = textMeshGO.GetComponent<TextMeshPro>();
		trail.SetActive(false);
		textMesh.raycastTarget = false;
	}

    private void OnEnable()
    {
        if (holdDuration <= 0) {
            Invoke("Destroy", timeOnScreen + hitOffset);
        }else{
            Invoke("Destroy", timeOnScreen + holdDuration);
        }
	}
	private void Destroy()
	{
        //if (!isClicked)
            //ScoreCtrl.WordMiss();
        gameObject.SetActive(false);
	}

	private void OnDestroy()
	{
		CancelInvoke();
	}

    public void SetData(WordData wd, float mp3Delay){
		wordData = wd;
        textMesh.text = wordData.text.ToUpper();
        showTime = (float.Parse(wordData.time) - timeOnScreen) + mp3Delay;
        hitTime = float.Parse(wordData.time) + mp3Delay;
		holdDuration = (float)wordData.duration;
        if (wd.isFake)
        {
            isFake = true;
            textMesh.color = Color.cyan;
        }
		//Debug.Log("duration: " + holdDuration);
		if(holdDuration>0){
			
			//Debug.Log(string.Format("Distance {0} , holdDuration {1}, total = {2}", distanceToTravel, holdDuration*100, distanceToTravel / (holdDuration*100)));
			trail.SetActive(true);
			float sHeight = trail.GetComponent<SpriteRenderer>().sprite.bounds.size.y;
            float prc = holdDuration/timeOnScreen;
			//Debug.Log("prc " + prc);
			float trailHeight = (distanceToTravel / (sHeight*100)) * prc;
			//float trailHeight = ((Camera.main.orthographicSize * 2.0f) / sHeight) * prc;
			//Debug.Log(distanceToTravel +" / "+ sHeight + " = " + trailHeight);
			trail.transform.localScale  = new Vector3(1, trailHeight, 1);
		}
	}
	
	public void StartHoldTween()
	{
		holdGraphicTweening = true;
		trail.transform.DOScaleY(0, holdDuration);
	}

}
