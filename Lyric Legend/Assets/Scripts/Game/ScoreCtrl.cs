using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCtrl : MonoBehaviour {

	
    public static float currentScore = 0;
    public static float pointValue = 100;
    public static float streak = 0;
    public static float streakMultiplier = 1;
    public static float clickPerfectTimeOffset = 0.05f;

    private static int lastOrderIndex = 0;

    private static float timeDiff;

    public static void ResetScore()
    {
        currentScore = 0;
        UIEventManager.ScoreChangedEvent();
        lastOrderIndex = 0;
        ResetStreak();
        UIEventManager.StreakChangedEvent();

    }

    public static void WordHit(int orderIndex, float hitTime, float wordTime)
    {
        CheckStreak(orderIndex);// set the streak value
        AddScore(hitTime, wordTime);
    }

    public static void WordMiss()
    {
        ResetStreak();
    }

    public static void EmptyClick()
    {
        ResetStreak();
    }

    private static void AddScore(float hitTime, float wordTime)
    {
        // ADD PERFECT/NON PERFECT SCORE
        timeDiff = Mathf.Abs(hitTime - wordTime);
        if (timeDiff < clickPerfectTimeOffset){
            currentScore += pointValue;
        }else{
            currentScore += pointValue / 2;
        }

        // ADD STREAK BONUS
        currentScore += streak*streakMultiplier;

        UIEventManager.ScoreChangedEvent();
	}

    // STREAK
    private static void ResetStreak()
    {
        streak = 0;
        UIEventManager.StreakChangedEvent();
    }

    private static void CheckStreak(int orderIndex){
        
        if(orderIndex-lastOrderIndex==1){
            streak++;
            UIEventManager.StreakChangedEvent();
        }else{
            ResetStreak();
        }
        lastOrderIndex = orderIndex;
    }

	public static void AddHoldPoints(float timeToBegin, float timeStarted, float durationToHold, float currentTime)
    {
		float prc = Mathf.Clamp((currentTime-timeStarted) / durationToHold, 0,1);
        float maxPoints = durationToHold * streakMultiplier;
		if(prc<1){
            currentScore += streakMultiplier*(Mathf.CeilToInt(prc*maxPoints));
            UIEventManager.ScoreChangedEvent();
		}
	}
}
