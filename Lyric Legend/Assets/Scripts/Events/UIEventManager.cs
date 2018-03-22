using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEventManager : MonoBehaviour {

	public delegate void SelectListItemFile(string fileName, string fileType);
	public static event SelectListItemFile OnListItemFileSelected;

	public static void SelectFileFromList(string fileName, string fileType)
	{
		if(OnListItemFileSelected!=null)
		{
			OnListItemFileSelected(fileName, fileType);
		}
	}
}
