using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEventManager : MonoBehaviour
{

    #region GAME EVENTS
    // PREPARE GAME EVENT
    public delegate void PrepareGame();
    public static event PrepareGame OnPrepareGame;
    public static void PrepareGameFireEvent()
    {
        if (OnPrepareGame != null)
        {
            OnPrepareGame();
        }
    }

    // START GAME EVENT
    public delegate void StartGame();
    public static event StartGame OnStartGame;
    public static void StartGameFireEvent()
    {
        if (OnStartGame != null)
        {
            OnStartGame();
        }
    }
    #endregion

    #region SCORE EVENTS
    // SCORE
    public delegate void ScoreChanged();
    public static event ScoreChanged OnScoreChanged;
    public static void ScoreChangedEvent()
    {
        if (OnScoreChanged != null)
        {
            OnScoreChanged();
        }
    }
    public delegate void FakeWordHit();
    public static event FakeWordHit OnFakeWordHit;
    public static void FakeWordHitEvent()
    {
        if (OnFakeWordHit != null)
        {
            OnFakeWordHit();
        }
    }

    #endregion  

    #region STREAK EVENTS
    public delegate void StreakChanged();
    public static event StreakChanged OnStreakChanged;
    public static void StreakChangedEvent()
    {
        if (OnStreakChanged != null)
        {
            OnStreakChanged();
        }
    }

    public delegate void StreakReset();
    public static event StreakReset OnStreakReset;
    public static void StreakResetEvent()
    {
        if (OnStreakReset != null)
        {
            OnStreakReset();
        }
    }
    #endregion

    #region MISC EVENTS

    public delegate void PerfectTapCountChanged();
    public static event PerfectTapCountChanged OnPerfectTapCountChanged;
    public static void PerfectTapCountChangedEvent()
    {
        if (OnPerfectTapCountChanged != null)
        {
            OnPerfectTapCountChanged();
        }
    }

    public delegate void NonPerfectTapCountChanged();
    public static event NonPerfectTapCountChanged OnNonPerfectTapCountChanged;
    public static void NonPerfectTapCountChangedEvent()
    {
        if (OnNonPerfectTapCountChanged != null)
        {
            OnNonPerfectTapCountChanged();
        }
    }

    public delegate void MissWordCountChanged();
    public static event MissWordCountChanged OnMissWordCountChanged;
    public static void MissWordCountChangedEvent()
    {
        if (OnMissWordCountChanged != null)
        {
            OnMissWordCountChanged();
        }
    }

    #endregion

    #region LIST EVENTS
    // SELECT FILE LIST ITEM
    public delegate void SelectListItemFile(string fileName, string fileType, string listItemName);
    public static event SelectListItemFile OnListItemFileSelected;
    public static void SelectFileFromList(string fileName, string fileType, string listItemName)
    {
        if (OnListItemFileSelected != null)
        {
            OnListItemFileSelected(fileName, fileType, listItemName);
        }
    }
    #endregion


    #region button events - DEPRICATED
    /* GAMEOBJECT AS BUTTON */
    /*
	// GameobjectButton TouchDown
	public delegate void TouchDownGameObject (GameObject g);
	public static event TouchDownGameObject OnTouchDownGameObject;
	public static void TouchDownGameObjectEvent(GameObject g){
		if(OnTouchDownGameObject!=null){
			OnTouchDownGameObject(g);
		}
	}
	// GameobjectButton TouchUp
	public delegate void TouchUpGameObject (GameObject g);
	public static event TouchUpGameObject OnTouchUpGameObject;
	public static void TouchUpGameObjectEvent(GameObject g){
		if(OnTouchUpGameObject!=null){
			OnTouchUpGameObject(g);
		}
	}
	// GameobjectButton TouchStay
	public delegate void TouchStayGameObject (GameObject g);
	public static event TouchStayGameObject OnTouchStayGameObject;
	public static void TouchStayGameObjectEvent(GameObject g){
		if(OnTouchStayGameObject!=null){
			OnTouchStayGameObject(g);
		}
	}
	// GameobjectButton TouchExit
	public delegate void TouchExitGameObject (GameObject g);
	public static event TouchExitGameObject OnTouchExitGameObject;
	public static void TouchExitGameObjectEvent(GameObject g){
		if(OnTouchExitGameObject!=null){
			OnTouchExitGameObject(g);
		}
	}
	*/
    #endregion
}
