using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class BrowserTestCtrl : MonoBehaviour {

	public RectTransform contentBox;
	public GameObject listItemPrefab;
	public Text textUIJsonSelected;
	public Text textUISongSelected;
	public float listItemVerticalDistance = 80f;

	private string jsonFilePatern = "*.json";
	private string assetFilePatern = "*.unity3d";
	private string songFileType;
	private GameObject listItem;
	private string[] fileList;
	private int fileListLength;
	private string filePath;

	void Start ()
	{
		
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
		fileList = Directory.GetFiles(Path.Combine(Application.persistentDataPath,"jsons"), jsonFilePatern);
		fileListLength = fileList.Length;
		if(fileListLength==0)
			return;
		
		PopulateList(StaticDataManager.JSON_FILE_TYPE);
	}

	public void BrowseSongs()
	{
		ClearContentBox();
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
			ListItemFile itemCtrl = listItem.GetComponent<ListItemFile>();
			itemCtrl.SetFile(filePath, fileType);
			listItem.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0-(i*listItemVerticalDistance));
		}
		contentBox.sizeDelta = new Vector2(contentBox.sizeDelta.x, fileListLength * listItemVerticalDistance);
	}

	private void OnFileSelected(string file_name, string file_type)
	{
		if(file_type==StaticDataManager.JSON_FILE_TYPE)
		{
			textUIJsonSelected.text = file_name;
		}else{
			textUISongSelected.text = file_name;
		}
	}


	public void OnPressPlay()
	{
		if(string.IsNullOrEmpty(textUIJsonSelected.text) || string.IsNullOrEmpty(textUISongSelected.text))
		{
			Debug.Log("NOT ALL FILES PRESENT");
			return;
		}

		gameObject.SetActive(false);

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
