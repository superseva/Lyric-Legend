using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCtrl : MonoBehaviour {

	
    public static float currentScore = 0;
    public static float streak = 0;

    public static int perfectCount = 0;
    public static int nonPerfectCount = 0;
    public static int missCount = 0;

    private static int lastOrderIndex = 0;

    private static float timeDiff;

    public static void ResetScore()
    {
        currentScore = 0;
        UIEventManager.ScoreChangedEvent();
        lastOrderIndex = 0;
        ResetStreak();
        UIEventManager.StreakChangedEvent();

        perfectCount = 0;
        nonPerfectCount = 0;
        missCount = 0;
        UIEventManager.PerfectTapCountChangedEvent();
        UIEventManager.NonPerfectTapCountChangedEvent();
        UIEventManager.MissWordCountChangedEvent();
    }

    public static void WordHit(int orderIndex, float hitTime, float wordTime, bool isPerfect)
    {
        CheckStreak(orderIndex);// set the streak value
        AddScore(hitTime, wordTime, isPerfect);
    }

    public static void WordMiss()
    {
        ResetStreak();
        missCount++;
        UIEventManager.MissWordCountChangedEvent();
    }

    public static void EmptyClick()
    {
        ResetStreak();
    }

    private static void AddScore(float hitTime, float wordTime, bool isPerfect)
    {
        if(isPerfect){
            currentScore += Config.POINT_VALUE;
            perfectCount++;
            UIEventManager.PerfectTapCountChangedEvent();
        }else{
            currentScore += Config.POINT_VALUE / 2;
            nonPerfectCount++;
            UIEventManager.NonPerfectTapCountChangedEvent();
        }

        // ADD STREAK BONUS
        currentScore += streak*Config.STREAK_MULTIPLIER;
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
        float maxPoints = durationToHold * Config.STREAK_MULTIPLIER;
		if(prc<1){
            currentScore += Config.STREAK_MULTIPLIER*(Mathf.CeilToInt(prc*maxPoints));
            UIEventManager.ScoreChangedEvent();
		}
	}
}
