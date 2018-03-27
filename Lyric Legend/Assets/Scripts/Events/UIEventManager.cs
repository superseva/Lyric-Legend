using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEventManager : MonoBehaviour {
	
	// PREPARE GAME EVENT
	public delegate void PrepareGame();
	public static event PrepareGame OnPrepareGame;
	public static void PrepareGameFireEvent(){
		if(OnPrepareGame!=null){
			OnPrepareGame();
		}
	}

	// START GAME EVENT
	public delegate void StartGame();
	public static event StartGame OnStartGame;
	public static void StartGameFireEvent(){
		if(OnStartGame!=null){
			OnStartGame();
		}
	}


	// SCORE
	public delegate void ScorePoint();
	public static event ScorePoint OnScorePoint;
	public static void ScorePointEvent(){
		if(OnScorePoint!=null){
			OnScorePoint();
		}
	}

	// SELECT FILE LIST ITEM
	public delegate void SelectListItemFile(string fileName, string fileType, string listItemName);
	public static event SelectListItemFile OnListItemFileSelected;
	public static void SelectFileFromList(string fileName, string fileType, string listItemName){
		if(OnListItemFileSelected!=null){
			OnListItemFileSelected(fileName, fileType, listItemName);
		}
	}

	/* SCORE */
	public delegate void AddScore();
	public static event AddScore OnAddScore;
	public static void AddScoreEvent(){
		if(OnAddScore!=null){
			OnAddScore();
		}
	}


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
}
