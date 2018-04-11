using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGame : MonoBehaviour {

	public Text scoreTextField;
    public Text streakField;
    public Text perfectTapCuntField;
    public Text notPerfectTapCountField;
    public Text missWordCountField;


	void OnEnable(){
        UIEventManager.OnScoreChanged += OnScoreChanged;
        UIEventManager.OnStreakChanged += OnStreakChanged;
        //scoreTextField.text = "SCORE " + ScoreCtrl.currentScore.ToString();
        UIEventManager.OnPerfectTapCountChanged += OnPerfectTap;
        UIEventManager.OnNonPerfectTapCountChanged += OnNotPerfectTap;
        UIEventManager.OnMissWordCountChanged += OnMissWord;
	}

	void OnDisable(){
        UIEventManager.OnScoreChanged -= OnScoreChanged;
        UIEventManager.OnStreakChanged -= OnStreakChanged;
        UIEventManager.OnPerfectTapCountChanged += OnPerfectTap;
        UIEventManager.OnNonPerfectTapCountChanged += OnNotPerfectTap;
        UIEventManager.OnMissWordCountChanged += OnMissWord;
	}

	void OnScoreChanged(){
        scoreTextField.text = "SCORE: " + ScoreCtrl.currentScore.ToString("#,#");
	}

    void OnStreakChanged(){
        //Debug.Log("STREAK =" + ScoreCtrl.streak);
        streakField.text = "STREAK: " + ScoreCtrl.streak.ToString();
    }

    void OnPerfectTap(){
        perfectTapCuntField.text = "PERFECT =  " + ScoreCtrl.perfectCount.ToString();
    }

    void OnNotPerfectTap(){
        notPerfectTapCountField.text = "NOT PERFECT =  " + ScoreCtrl.nonPerfectCount.ToString();
    }

    void OnMissWord(){
        missWordCountField.text = "MISS =  " + ScoreCtrl.missCount;
    }

    void OnEmptyTap()
    {
        
    }
}
