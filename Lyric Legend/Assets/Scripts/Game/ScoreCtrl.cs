using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCtrl : MonoBehaviour {

	public static float currentScore;

	public static void AddScore(){
		currentScore ++;
		UIEventManager.AddScoreEvent();
	}

	public static float coeficient = 1;
	public static void AddHoldPoints(float timeToBegin, float timeStarted, float durationToHold, float currentTime){
		float prc = Mathf.Clamp((currentTime-timeStarted) / durationToHold, 0,1);
		float maxPoints = durationToHold * coeficient;
		if(prc<1){
			currentScore += coeficient*(Mathf.CeilToInt(prc*maxPoints));
			UIEventManager.AddScoreEvent();
		}
	}
}
