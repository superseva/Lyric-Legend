using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ListItemFile : MonoBehaviour {

	public Text label;
	public string filePath;
	public string fileName;
	public string fileType;

	public void SetFile(string fPath, string fType)
	{
		filePath = fPath;
		fileName = Path.GetFileName(fPath);
		label.text = fileName;
		fileType = fType;
	}

	public void OnClickListItem()
	{
		UIEventManager.SelectFileFromList(fileName, fileType);
	}
}
