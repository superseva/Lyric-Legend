using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class BrowserTestCtrl : MonoBehaviour {

	public GameObject choosePanel;
	public RectTransform contentBox;
	public GameObject listItemPrefab;
	public Text textUIJsonSelected;
	public Text textUISongSelected;
	public Text textListTitle;
	public float listItemVerticalDistance = 80f;
	public GameObject gameManager;

	private string jsonFilePatern = "*.json";
	private string assetFilePatern = "*.unity3d";
	private string songFileType;
	private GameObject listItem;
	private string[] fileList;
	private int fileListLength;
	private string filePath;

	private static string jsonItemSelected;
	private static string audioItemSelected;

	void Start ()
	{
		gameManager.SetActive(false);
	}

	private void ClearContentBox()
	{
		foreach(Transform child in contentBox.transform)
		{
			GameObject.Destroy(child.gameObject);
		}
	}
	
	public void BrowseJSON()
	{
		ClearContentBox();
		textListTitle.text = "JSON FILES : ";
		fileList = Directory.GetFiles(Path.Combine(Application.persistentDataPath,"jsons"), jsonFilePatern);
		fileListLength = fileList.Length;
		if(fileListLength==0)
			return;
		
		PopulateList(StaticDataManager.JSON_FILE_TYPE);
	}

	public void BrowseSongs()
	{
		ClearContentBox();
		textListTitle.text = "SONG FILES : ";
		fileList = Directory.GetFiles(Path.Combine(Application.persistentDataPath,"assetbundles"), assetFilePatern);
		fileListLength = fileList.Length;
		if(fileListLength==0)
			return;

		PopulateList(StaticDataManager.ASSET_FILE_TYPE);
	}

	private void PopulateList(string fileType)
	{
		for(int i = 0; i < fileListLength; i++ )
		{
			filePath = fileList[i];
			listItem = Instantiate(listItemPrefab,contentBox);
			listItem.name = "l"+i;
			ListItemFile itemCtrl = listItem.GetComponent<ListItemFile>();
			itemCtrl.SetFile(filePath, fileType);
			listItem.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0-(i*listItemVerticalDistance));
		}
		contentBox.sizeDelta = new Vector2(contentBox.sizeDelta.x, fileListLength * listItemVerticalDistance);

		if(fileType == StaticDataManager.JSON_FILE_TYPE && !string.IsNullOrEmpty(jsonItemSelected))
			selectListItemVisualy(jsonItemSelected);
		if(fileType == StaticDataManager.ASSET_FILE_TYPE && !string.IsNullOrEmpty(audioItemSelected))
			selectListItemVisualy(audioItemSelected);
	}

	private void OnFileSelected(string file_name, string file_type, string item_name)
	{
		if(file_type==StaticDataManager.JSON_FILE_TYPE)
		{
			textUIJsonSelected.text = file_name;
			StaticDataManager.SelectedJsonName = file_name;
			jsonItemSelected = item_name;
			selectListItemVisualy(jsonItemSelected);
		}else{
			textUISongSelected.text = file_name;
			StaticDataManager.SelectedAudioName = file_name;
			audioItemSelected = item_name;
			selectListItemVisualy(audioItemSelected);
		}
	}

	private void selectListItemVisualy(string item_name)
	{
		foreach(Transform obj in contentBox.transform)
		{
			if(obj.gameObject.name == item_name)
			{
				obj.gameObject.GetComponentInChildren<Text>().color = new Color(225,0,225);
			}else{
				obj.gameObject.GetComponentInChildren<Text>().color = Color.white;
			}
		}
	}


	public void OnPressPlay()
	{
		if(string.IsNullOrEmpty(textUIJsonSelected.text) || string.IsNullOrEmpty(textUISongSelected.text))
		{
			Debug.Log("NOT ALL FILES PRESENT");
			return;
		}
		gameManager.SetActive(true);

		UIEventManager.PrepareGameFireEvent();
		choosePanel.SetActive(false);
	}

	void OnEnable(){
		UIEventManager.OnListItemFileSelected += OnFileSelected;
	}

	void OnDisable()
	{
		UIEventManager.OnListItemFileSelected -= OnFileSelected;
	}


	void Update ()
	{
		
	}
}
