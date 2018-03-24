using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGame : MonoBehaviour {

	public Text scoreTextField;
	private int score = 0;

	void OnEnable(){
		score = 0;
		UIEventManager.OnScorePoint += ScorePoint;
	}

	void OnDisable(){
		UIEventManager.OnScorePoint -= ScorePoint;
	}

	void ScorePoint(){
		score = score + 1;
		scoreTextField.text = "SCORE " + score.ToString();
	}
}
