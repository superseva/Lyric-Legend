﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEventManager : MonoBehaviour {


	// PREPARE GAME EVENT
	public delegate void PrepareGame();
	public static event PrepareGame OnPrepareGame;

	public static void PrepareGameFireEvent()
	{
		if(OnPrepareGame!=null)
		{
			OnPrepareGame();
		}
	}

	// START GAME EVENT
	public delegate void StartGame();
	public static event StartGame OnStartGame;

	public static void StartGameFireEvent()
	{
		if(OnStartGame!=null)
		{
			OnStartGame();
		}
	}

	// SELECT FILE LIST ITEM
	public delegate void SelectListItemFile(string fileName, string fileType, string listItemName);
	public static event SelectListItemFile OnListItemFileSelected;

	public static void SelectFileFromList(string fileName, string fileType, string listItemName)
	{
		if(OnListItemFileSelected!=null)
		{
			OnListItemFileSelected(fileName, fileType, listItemName);
		}
	}
}