using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class TestLoadJsonFromAppFolder : MonoBehaviour {

	public Text textField;


	void Start () {
		ListJsons();
	}

	private void ListJsons()
	{
		foreach(string filePath in Directory.GetFiles(Application.persistentDataPath, "*.json"))
		{
			//Debug.Log(filePath);
			string fileName =  Path.GetFileName(filePath);
			Debug.Log(fileName);
		}

	}

	public void LoadAndShowJson()
	{
		string jsonPath = Path.Combine(Application.persistentDataPath, "jsontest.json");
		string text = File.ReadAllText(jsonPath);
		Debug.Log(text);
		textField.text = text.Substring(0,50);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
