using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using LitJson;
using UnityEngine.UI;

public class TestGameManager : MonoBehaviour {

	[HideInInspector]
	public GameObject audioPrefab;
	public GameObject gameUI;
	public GameObject pickUI;
	public GameObject wordGameObjectPrefab;
	public float hitOffset = 0.2f;
	public GameObject bottomLine;

	private AssetBundle myLoadedAssetBundle;
	private GameObject audioGameObject;
	private AudioSource audioSource;
	private AudioClip audioClip;
	private float currentAudioTime;
	private float sampleRate = 44100f;

	private string jsonStringFromFile;
	private SongData songData;
	private float timeOnScreen = 2;

	private int listIndex = 0;
	private float nextShowTime = 0;
	private float nextHitTime = 0;
	private List<WordGameObjectCtrl>wordsCtrlList = new List<WordGameObjectCtrl>();

	private Camera cam;
	//SCREEN POZITIONS
	private float distanceOnScreen;
	private float screenZ = 10;
	private float startPozitionOnScreen; // screen top 
	private float endPozitionOnScreen = 100; // some px from the bottom of the screen
	private float[] xPozitions;
	// WORLD POZITIONS
	private Vector3[] startPositionsInWorld;
	private Vector3[] endPositionsOnInWorld;
	//COLIDER AREAS
	public GameObject clickArea1;
	public GameObject clickArea2;
	public GameObject clickArea3;

	//temp
	private int clicksCount;
	public Text clicksCountText;


	private WordGameObjectCtrl currentWord;

	// Use this for initialization
	void Start () {
		// temporary
		//Input.simulateMouseWithTouches = true;

		cam = Camera.main;
		Screen.sleepTimeout = SleepTimeout.NeverSleep;

		startPozitionOnScreen = Screen.height;


		//Debug.Log( (cam.orthographicSize * 2.0) + " ... " + Screen.height);

		distanceOnScreen = startPozitionOnScreen - endPozitionOnScreen;
		//Debug.Log("DISTANCE ON SCREEN " + distanceOnScreen);

		float screenQ = Screen.width/3;
		xPozitions = new float[4];
		xPozitions[1] = 0;
		xPozitions[1] = Mathf.Round(screenQ/2);
		xPozitions[2] = Mathf.Round(Screen.width/2);
		xPozitions[3] = Mathf.Round(screenQ*3 - screenQ/2);

		startPositionsInWorld = new Vector3[4];
		startPositionsInWorld[0] = new Vector3();
		startPositionsInWorld[1] = cam.ScreenToWorldPoint( new Vector3(xPozitions[1], startPozitionOnScreen, screenZ));
		startPositionsInWorld[2] = cam.ScreenToWorldPoint( new Vector3(xPozitions[2], startPozitionOnScreen, screenZ));
		startPositionsInWorld[3] = cam.ScreenToWorldPoint( new Vector3(xPozitions[3], startPozitionOnScreen, screenZ));

		endPositionsOnInWorld = new Vector3[4];
		endPositionsOnInWorld[0] = new Vector3();
		endPositionsOnInWorld[1] = cam.ScreenToWorldPoint( new Vector3(xPozitions[1], endPozitionOnScreen, screenZ));
		endPositionsOnInWorld[2] = cam.ScreenToWorldPoint( new Vector3(xPozitions[2], endPozitionOnScreen, screenZ));
		endPositionsOnInWorld[3] = cam.ScreenToWorldPoint( new Vector3(xPozitions[3], endPozitionOnScreen, screenZ));

		bottomLine.transform.position = endPositionsOnInWorld[2];

		clickArea1.transform.position = endPositionsOnInWorld[1];
		clickArea2.transform.position = endPositionsOnInWorld[2];
		clickArea3.transform.position = endPositionsOnInWorld[3];

		//testArea.transform.position = endPositionsOnInWorld[3];
	}

	void OnEnable()
	{
		UIEventManager.OnPrepareGame += OnPrepareEvent;
	}

	void OnDisable()
	{
		UIEventManager.OnPrepareGame -= OnPrepareEvent;
		StopCoroutine(LoadAudioAsset());
	}

	void OnPrepareEvent(){
		//Debug.Log( string.Format("PREPARING : {0} file and {1} file", StaticDataManager.SelectedJsonName, StaticDataManager.SelectedAudioName) );
		gameUI.SetActive(true);
		listIndex = 0;
		nextShowTime = 0;
		nextHitTime = 0;
		StartCoroutine(LoadAudioAsset());

		clicksCount = 0;
		clicksCountText.text = "CLICKS COUNT :" + clicksCount.ToString();
			
	}

	IEnumerator LoadAudioAsset(){
		string folderPath = Path.Combine(Application.persistentDataPath, StaticDataManager.AUDIO_FOLDER);
		string filePath = Path.Combine(folderPath, StaticDataManager.SelectedAudioName);
		//Debug.Log( string.Format("PATH TO AUDIO FILE : {0}", filePath) );
		AssetBundleCreateRequest bundle = AssetBundle.LoadFromFileAsync(filePath);
		yield return bundle;

		myLoadedAssetBundle = bundle.assetBundle;
		if(myLoadedAssetBundle == null)
		{
			Debug.Log("FAILED TO LOAD ASSET");
			yield break;
		}

		string assetName = StaticDataManager.SelectedAudioName.Split('.')[0];
		AssetBundleRequest request = myLoadedAssetBundle.LoadAssetAsync<GameObject>(assetName);
		yield return request;

		if(request.asset==null){
			Debug.Log("NO ASSET FOUND IN BUNDLE");
			yield break;
		}
		audioPrefab = request.asset as GameObject;
		OnAudioPrefabLoaded();
	}

	void OnAudioPrefabLoaded()
	{
		audioGameObject = Instantiate(audioPrefab);
		audioSource = audioGameObject.GetComponent<AudioSource>();
		audioClip = audioSource.clip;

		GenerateLyricObjects();

		listIndex = 0;
		nextShowTime = float.Parse(songData.wordsList[listIndex].time) - timeOnScreen;
		nextHitTime = float.Parse(songData.wordsList[listIndex].time);
		// FORWARD SONG TO A WORD (listIndex)
		audioSource.timeSamples = Mathf.CeilToInt(sampleRate * (nextShowTime-2));
		audioSource.Play();
	}

	void GenerateLyricObjects()
	{
		string folderPath = Path.Combine(Application.persistentDataPath, StaticDataManager.JSON_FOLDER);
		string filePath = Path.Combine(folderPath, StaticDataManager.SelectedJsonName);
		jsonStringFromFile = File.ReadAllText(filePath);
		songData = JsonMapper.ToObject<SongData>(jsonStringFromFile);
		timeOnScreen = songData.timeOnScreen;
		//Debug.Log("TIME ON SCREEN " + timeOnScreen);

		wordsCtrlList.Clear();

		GameObject wordGameObject;
		WordGameObjectCtrl wordCtrl;
		for(int i=0; i<songData.wordsList.Length; i++)
		{
			wordGameObject = Instantiate(wordGameObjectPrefab, gameObject.transform);
			wordGameObject.name = "WordAt_"+songData.wordsList[i].time.ToString();
			wordCtrl = wordGameObject.GetComponent<WordGameObjectCtrl>();
			wordCtrl.orderIndex = i;
			wordCtrl.existanceTime = timeOnScreen;
			wordCtrl.distanceToTravel = distanceOnScreen;
			wordCtrl.SetData(songData.wordsList[i]);
			wordCtrl.startPosition = startPositionsInWorld[wordCtrl.wordData.index];
			wordCtrl.endPosition = endPositionsOnInWorld[wordCtrl.wordData.index];

			wordGameObject.transform.localPosition = wordCtrl.startPosition;
			wordsCtrlList.Add(wordCtrl);
			wordGameObject.SetActive(false);
		}
	}
	public void StopPlaying()
	{
		audioSource.Stop();
		ClearChildren();
		listIndex = 0;
		GameObject.Destroy(audioGameObject);
		myLoadedAssetBundle.Unload(true);
		gameUI.SetActive(false);
		pickUI.SetActive(true);
		gameObject.SetActive(false);
	}

	private void ClearChildren()
	{
		foreach(Transform tr in gameObject.transform)
		{
			Destroy(tr.gameObject);
		}
	}

	private RaycastHit2D hitInfo;
		
	void Update () {
		
		if(!audioSource)
			return;

		if(!audioSource.isPlaying)
			return;

		currentAudioTime = audioSource.timeSamples/sampleRate;

		if (currentAudioTime >= nextShowTime && listIndex<wordsCtrlList.Count)
		{
			currentWord = wordsCtrlList[listIndex];
			currentWord.gameObject.SetActive(true);
			listIndex++;
			if(listIndex<wordsCtrlList.Count-1){
				nextShowTime = wordsCtrlList[listIndex].showTime;
			}
		}

		//moving
		float rangeTime;
		float percentTime;
		float newY;
		foreach(WordGameObjectCtrl word in wordsCtrlList)
		{
			if (word.gameObject.activeSelf)
			{
				rangeTime = word.hitTime - word.showTime;
				percentTime = (currentAudioTime - word.showTime) / rangeTime;
				//newY = (distanceOnScreen - (distanceOnScreen * percentTime)) + endPozitionOnScreen;
				//word.gameObject.transform.localPosition = Camera.main.ScreenToWorldPoint(new Vector3(xPozitions[word.wordData.index], newY, 10));
				word.gameObject.transform.localPosition = Vector3.Lerp(word.startPosition, word.endPosition, percentTime);
				if(currentAudioTime >= word.hitTime){
					word.textMesh.color = new Color(255,0,255);
				}
				if(percentTime>=1 && !word.holdGraphicTweening)
				{
					word.StartHoldTween();
				}
			}
		}


		// pressing the buttons in editor
		if(Application.isEditor){
			if(Input.GetMouseButtonDown(0)){
				Vector2 pos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
				hitInfo = Physics2D.Raycast(cam.ScreenToWorldPoint(pos), Vector2.zero);
				if(hitInfo)
				{
					if(hitInfo.transform.gameObject.tag=="ClickArea"){
						Debug.Log("CLICK");
						CheckForWordHit(hitInfo.transform.gameObject.GetComponent<ClickAreaCtrl>());
					}
				}
			}
		}
		else{

			// touch iOS
			for(int t = 0; t < Input.touchCount; t++){
				if(Input.GetTouch(t).phase == TouchPhase.Began)
				{
					clicksCount ++;
					clicksCountText.text = "CLICKS COUNT :" + clicksCount.ToString();

					hitInfo = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.GetTouch(t).position), Vector2.zero);
					if(hitInfo)
					{
						if(hitInfo.transform.gameObject.tag=="ClickArea"){
							Debug.Log("TOUCH");
							CheckForWordHit(hitInfo.transform.gameObject.GetComponent<ClickAreaCtrl>());
						}
					}
				}
			}

		}

	}


	void CheckForWordHit(ClickAreaCtrl clickedArea)
	{
		if(clickedArea.isColliding){
			if(!clickedArea.wordCtrl.isClicked){
				Debug.Log("HIT !!! AREA IS COLLIDING WITH WORD " + clickedArea.wordCtrl.orderIndex);
				clickedArea.wordCtrl.isClicked = true;
				UIEventManager.ScorePointEvent();
			}else{
				Debug.Log("Already Clicked");
			}
		}else{
			Debug.Log("MISS !");
		}
	}

}
