using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using LitJson;
using UnityEngine.UI;
using DG.Tweening;

public class TestGameManager : MonoBehaviour {

	[HideInInspector]
	public GameObject audioPrefab;
	public GameObject gameUI;
	public GameObject pickUI;
	public GameObject wordGameObjectPrefab;
	public GameObject successWordHitFX;
	public float hitOffset = 0.2f;
	public GameObject bottomLine;
	public GameObject gameElements;
    public GameObject stageVisual;

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
    private float LyricStartPointInPercent = 1;
    private float LyricEndPointInPercent = 0.1f;
    private float StageVisualPositionInPercent = 0.5f;
    private Vector3 stageVisualPosition = Vector3.zero;

	private float distanceOnScreen;
	private float screenZ = 10;
	private float startPozitionOnScreen; // screen top 
	public float endPozitionOnScreen = 100; // some px from the bottom of the screen
	private float[] xPozitions;
	// WORLD POZITIONS
	private Vector3[] startPositionsInWorld;
	private Vector3[] endPositionsOnInWorld;
	//COLIDER AREAS
	public ClickAreaCtrl[] clickAreas = new ClickAreaCtrl[3];

	//temp
	private int clicksCount;
	public Text clicksCountText;

	private WordGameObjectCtrl currentWord;

	//moving update
	private float rangeTime;
	private float percentTime;
	private float newY;

    private JsonData configData;

	void Start () {

        Configurate();

		Input.multiTouchEnabled = true;
		cam = Camera.main;
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
	
        //startPozitionOnScreen = Screen.height;
        startPozitionOnScreen = Mathf.Round(Screen.height * LyricStartPointInPercent);
        endPozitionOnScreen = Mathf.Round(Screen.height * LyricEndPointInPercent);
		distanceOnScreen = startPozitionOnScreen - endPozitionOnScreen;
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

		clickAreas[0].gameObject.transform.position = endPositionsOnInWorld[1];
		clickAreas[1].gameObject.transform.position = endPositionsOnInWorld[2];
		clickAreas[2].gameObject.transform.position = endPositionsOnInWorld[3];

        stageVisualPosition = cam.ScreenToWorldPoint( new Vector3(Mathf.Round(Screen.width * 0.5f) , Mathf.Round(Screen.height * StageVisualPositionInPercent), screenZ));
        stageVisual.transform.position = stageVisualPosition;

		PoolManager.WarmPool(successWordHitFX, 20);

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
		ScoreCtrl.currentScore = 0;
		listIndex = 0;
		nextShowTime = 0;
		nextHitTime = 0;

		foreach(ClickAreaCtrl c in clickAreas){
			c.gameObject.SetActive(true);
		}
		gameElements.SetActive(true);
		gameUI.SetActive(true);
		clicksCount = 0;
		clicksCountText.text = "CLICKS COUNT :" + clicksCount.ToString();

		StartCoroutine(LoadAudioAsset());
	}

    void Configurate(){
        string path = Path.Combine(Application.persistentDataPath, "config.json");
        if (!File.Exists(path))
        {
            Debug.Log("NO CONFIG");
            return;
        }

        configData = JsonMapper.ToObject<JsonData>(File.ReadAllText(path));
        //Debug.Log("LyricStartPointFromScreenBottomInPercent = " + configData["LyricStartPointFromScreenBottomInPercent"]);
        LyricStartPointInPercent = (float)configData["LyricStartPointInPercent"];
        LyricEndPointInPercent = (float)configData["LyricEndPointInPercent"];
        StageVisualPositionInPercent = (float)configData["StageVisualPositionInPercent"];
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
            wordCtrl.SetData(songData.wordsList[i], songData.mp3dealy);
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
		foreach(ClickAreaCtrl c in clickAreas){
			c.gameObject.SetActive(false);
		}
		gameUI.SetActive(false);
		pickUI.SetActive(true);
		gameElements.SetActive(false);
		gameObject.SetActive(false);
	}

	private void ClearChildren()
	{
		foreach(Transform tr in gameObject.transform)
		{
			Destroy(tr.gameObject);
		}
	}



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

		CheckTouches();

	}


	private ClickAreaCtrl clickAreaCtrl;
	WordGameObjectCtrl wgCollidingCtrl;

	void CheckTouches(){
		if(Input.touchCount < 1)
			return;

		foreach(Touch touch in Input.touches)
		{
			if(touch.phase==TouchPhase.Began)
			{
				RaycastHit2D hitInfo = Physics2D.Raycast(cam.ScreenToWorldPoint(touch.position), Vector2.zero);
				if(hitInfo){
					GameObject recipient = hitInfo.transform.gameObject;
					if(recipient.tag == "ClickArea")
					{
						RegisterClick();
						clickAreaCtrl = recipient.GetComponent<ClickAreaCtrl>();
						wgCollidingCtrl = clickAreaCtrl.GetCollidingWord();
						if(wgCollidingCtrl!=null)
						{
							RegisterWordHit(clickAreaCtrl);
							if(wgCollidingCtrl.wordData.duration>0.0)
							{
								clickAreaCtrl.StartHolding(touch.fingerId, (float)wgCollidingCtrl.wordData.duration, currentAudioTime);
							}
						}
					}
				}
			}

			if(touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
			{
				foreach(ClickAreaCtrl caCtrl in clickAreas)
				{
					if(touch.fingerId==caCtrl.fingerIdDown)
					{
						//Debug.Log("STOP HOLDING " + caCtrl.gameObject.name + " fid =" +caCtrl.fingerIdDown+ " WITH FINGER ID "+ touch.fingerId);
						float timePressed = currentAudioTime - caCtrl.startHoldTime;
						//Debug.Log("HODING FOR : " + timePressed + " ["+caCtrl.durationToHold+"]");
						caCtrl.fingerIdDown = 100;
						caCtrl.startHoldTime = 0;
						caCtrl.durationToHold = 0;
						caCtrl.StopParticles();
					}
				}
			}

			if(touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary){
				foreach(ClickAreaCtrl caCtrl in clickAreas)
				{
					if(caCtrl.fingerIdDown!=100 && caCtrl.durationToHold!=0){
                        ScoreCtrl.AddHoldPoints(caCtrl.wordCtrl.hitTime, caCtrl.startHoldTime, caCtrl.durationToHold, currentAudioTime);
					}
				}
			}

		}
	}

	void RegisterClick()
	{
		clicksCount ++;
		clicksCountText.text = "CLICKS COUNT :" + clicksCount.ToString();
	}

	GameObject hitFX;
	void RegisterWordHit(ClickAreaCtrl cArea){
		hitFX = PoolManager.SpawnObject(successWordHitFX);
		hitFX.transform.position = cArea.gameObject.transform.position;
		ScoreCtrl.AddScore();
	}

	void RegisterHoldingWord(ClickAreaCtrl cAreaCtrl){
		
	}
}