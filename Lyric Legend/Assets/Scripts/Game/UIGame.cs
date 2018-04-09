using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGame : MonoBehaviour {

	public Text scoreTextField;
    public Text streakField;

	void OnEnable(){
        UIEventManager.OnScoreChanged += OnScoreChanged;
        UIEventManager.OnStreakChanged += OnStreakChanged;
		scoreTextField.text = "SCORE " + ScoreCtrl.currentScore.ToString();
	}

	void OnDisable(){
        UIEventManager.OnScoreChanged -= OnScoreChanged;
        UIEventManager.OnStreakChanged -= OnStreakChanged;
	}

	void OnScoreChanged(){
		scoreTextField.text = "SCORE " + ScoreCtrl.currentScore.ToString("#,#");
	}

    void OnStreakChanged(){
        //Debug.Log("STREAK =" + ScoreCtrl.streak);
        streakField.text = "STREAK " + ScoreCtrl.streak.ToString();
    }
}
