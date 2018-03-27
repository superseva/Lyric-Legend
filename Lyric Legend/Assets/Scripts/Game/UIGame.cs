using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGame : MonoBehaviour {

	public Text scoreTextField;

	void OnEnable(){
		UIEventManager.OnAddScore += OnAddScore;

		scoreTextField.text = "SCORE " + ScoreCtrl.currentScore.ToString();
	}

	void OnDisable(){
		UIEventManager.OnAddScore -= OnAddScore;
	}

	void OnAddScore(){
		scoreTextField.text = "SCORE " + ScoreCtrl.currentScore.ToString();
	}
}
