using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using LitJson;
using UnityEngine.UI;
using UnityEngine.Networking;
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

    private Camera cam;
    private AudioPeer audioPeer;
	private AssetBundle myLoadedAssetBundle;
	private GameObject audioGameObject;
	private AudioSource audioSource;
	private AudioClip audioClip;
	private float currentAudioTime;
	private float sampleRate = 44100f;

	private int listIndex = 0;
	private float nextShowTime = 0;
	private float nextHitTime = 0;
	private List<WordGameObjectCtrl>wordsCtrlList = new List<WordGameObjectCtrl>();

    // word positioning and distribution
    private float lastPlacedTimestamp = 0;
    private int lastPlacedIndex = 1; //lane index (1, 2 or 3)

	//SCREEN POZITIONS
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

    private WordGameObjectCtrl currentWord;

	//clicks count TEMP
	private int clicksCount;

	//moving update
	private float rangeTime;
	private float percentTime;
	private float newY;

    //JSON
    private string jsonStringFromFile;
    private SongData songData;
    private float timeOnScreen = 2;
    private string manifestPath = "https://firebasestorage.googleapis.com/v0/b/karaokehero-b35cc.appspot.com/o/AssetBundles?alt=media&token=3103cdd8-6792-4efc-aa21-029e3247512d";

	void Start () {

        if(!Config.ConfigGeneral()){
            return;
        }

		Input.multiTouchEnabled = true;
		cam = Camera.main;
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
	
        //startPozitionOnScreen = Screen.height;
        startPozitionOnScreen = Mathf.Round(Screen.height * Config.LYRIC_START_POINT_IN_PERCENT);
        endPozitionOnScreen = Mathf.Round(Screen.height * Config.LYRIC_END_POINT_IN_PERCENT);
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

        stageVisualPosition = cam.ScreenToWorldPoint( new Vector3(Mathf.Round(Screen.width * 0.5f) , Mathf.Round(Screen.height * Config.STAGE_VISUAL_POSITION_IN_PERCENT), screenZ));
        stageVisual.transform.position = stageVisualPosition;

		PoolManager.WarmPool(successWordHitFX, 20);

        audioPeer = GetComponent<AudioPeer>();
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

    /*
     * PREPARE THE GAMESCREEN
     * Enable/Disable elements
     * finaly start loading AUDIO ASSET
     */

	void OnPrepareEvent(){
		listIndex = 0;
		nextShowTime = 0;
		nextHitTime = 0;
		foreach(ClickAreaCtrl c in clickAreas){
			c.gameObject.SetActive(true);
		}
		gameElements.SetActive(true);
		gameUI.SetActive(true);
        ScoreCtrl.ResetScore();
		clicksCount = 0;
		StartCoroutine(LoadAudioAsset());
	}


    /*
     * LOADING AUDIO ASSETS
     */
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

    /*
    IEnumerator LoadAudioAsset(){
        UnityWebRequest uwr = UnityWebRequest.GetAssetBundle(manifestPath);

        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError || uwr.isHttpError)
        {
            Debug.Log(uwr.error);
            yield break;
        }

        AssetBundle manifesBundle = DownloadHandlerAssetBundle.GetContent(uwr);
        AssetBundleManifest manifest = manifesBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            
        if(manifest==null){
            Debug.Log("NO MANIFEST");
        }
        yield return null;




        //Download the bundle
        string uri = "https://firebasestorage.googleapis.com/v0/b/karaokehero-b35cc.appspot.com/o/girllikeyou.unity3d?alt=media&token=8bf4ada8-57f9-47af-abdb-3076e9ec62eb";
        Hash128 hash = manifest.GetAssetBundleHash("girllikeyou.unity3d");
        UnityWebRequest request = UnityWebRequest.GetAssetBundle(uri, 3, 0);
        yield return request.SendWebRequest();
        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);

        List<Hash128> listOfCachedVersions = new List<Hash128>();
        Caching.GetCachedVersions("girllikeyou.unity3d", listOfCachedVersions);
        Debug.Log("NUM OF CACHES: " + listOfCachedVersions.Count);

        foreach(Hash128 h in  listOfCachedVersions){
            Debug.Log("HASH : " + h);
        }

        //Caching.ClearCache();
        //Cache lastOne = Caching.GetCacheAt(listOfCachedVersions.Count-1);
        //Cache firstOne = Caching.GetCacheAt(0);
        //Caching.MoveCacheBefore(lastOne, firstOne);
        //Caching.ClearOtherCachedVersions("", hash);
    }
    */

    /*
    IEnumerator LoadAudioAsset()
    {

        string filePath = "https://firebasestorage.googleapis.com/v0/b/karaokehero-b35cc.appspot.com/o/girllikeyou.unity3d?alt=media&token=8bf4ada8-57f9-47af-abdb-3076e9ec62eb";
        //Debug.Log( string.Format("PATH TO AUDIO FILE : {0}", filePath) );
        UnityWebRequest uwr = UnityWebRequest.GetAssetBundle(filePath, 2, 0);
        uwr.SendWebRequest();
        while(!uwr.isDone){
            Debug.Log("% "+ uwr.downloadProgress);
            yield return null;
        }
        if (uwr.isNetworkError || uwr.isHttpError)
        {
            Debug.Log(uwr.error);
            yield break;
        }
        Debug.Log("DOwnloading " + filePath);
        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(uwr);
        yield return null;
        //List<Hash128> listOfCachedVersions = new List<Hash128>();
        //Caching.GetCachedVersions(bundle.name, listOfCachedVersions);
        //Debug.Log("NUM OF CACHES: " + listOfCachedVersions.Count);
        //foreach(Hash128 h in  listOfCachedVersions){
        //    Debug.Log("HASH : " + h);
        //}
        string assetName = StaticDataManager.SelectedAudioName.Split('.')[0];
        Debug.Log("ASSET NAME " + assetName);
        AssetBundleRequest request = bundle.LoadAssetAsync<GameObject>(assetName);
        yield return request;

          if(request.asset==null){
              Debug.Log("NO ASSET FOUND IN BUNDLE");
              yield break;
          }
          audioPrefab = request.asset as GameObject;
          OnAudioPrefabLoaded();
    }*/

	void OnAudioPrefabLoaded()
	{
		audioGameObject = Instantiate(audioPrefab);
		audioSource = audioGameObject.GetComponent<AudioSource>();
		audioClip = audioSource.clip;
        audioPeer.audioSource = audioSource;
		GenerateLyricObjects();

		listIndex = 0;
		nextShowTime = float.Parse(songData.wordsList[listIndex].time) - timeOnScreen;
		nextHitTime = float.Parse(songData.wordsList[listIndex].time);
		// FORWARD SONG TO A WORD (listIndex) if you want
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
			wordCtrl.orderIndex = i+1;// starts from 1
            wordCtrl.timeOnScreen = timeOnScreen;
			wordCtrl.distanceToTravel = distanceOnScreen;
            wordCtrl.SetData(songData.wordsList[i], songData.mp3dealy);
            // set hard difficulty
            wordCtrl.xPositionIndex = (StaticDataManager.difficulty == 2) ? Mathf.FloorToInt(Random.Range(1, 3.99f)) : wordCtrl.wordData.index;
            if(wordCtrl.hitTime - lastPlacedTimestamp < Config.MINIMUM_TIME_DIFFERENCE_IN_LANE && wordCtrl.xPositionIndex==lastPlacedIndex){
                wordCtrl.xPositionIndex += 1;
                if(wordCtrl.xPositionIndex>3)
                    wordCtrl.xPositionIndex = 1;
            }
            lastPlacedIndex = wordCtrl.xPositionIndex;
            lastPlacedTimestamp = wordCtrl.hitTime;
            //set start & end positions
            wordCtrl.startPosition = startPositionsInWorld[wordCtrl.xPositionIndex];
            wordCtrl.endPosition = endPositionsOnInWorld[wordCtrl.xPositionIndex];
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
                if (word.isClicked != true)
                {
                    newY = (distanceOnScreen - (distanceOnScreen * percentTime)) + endPozitionOnScreen;
                    //xPozitions[word.wordData.index]
                    word.gameObject.transform.localPosition = Camera.main.ScreenToWorldPoint(new Vector3(xPozitions[word.xPositionIndex], newY, 10));
                }
                else
                {
                    word.gameObject.transform.localPosition = Vector3.Lerp(word.startPosition, word.endPosition, percentTime);
                }
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
						clickAreaCtrl = recipient.GetComponent<ClickAreaCtrl>();
                        wgCollidingCtrl = clickAreaCtrl.GetCollidingWord(); // returns a word if it is not clicked already
						if(wgCollidingCtrl!=null)
						{
                            RegisterWordHit(clickAreaCtrl, wgCollidingCtrl);
							if(wgCollidingCtrl.wordData.duration>0.0)
							{
								clickAreaCtrl.StartHolding(touch.fingerId, (float)wgCollidingCtrl.wordData.duration, currentAudioTime);
							}
                        }else{
                            // empty click. kill streak
                            //RegisterWordMiss(clickAreaCtrl);
                            RegisterEmptyClick();
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

    //REGISTER ANY CLICK ON BUTTON
	void RegisterEmptyClick()
	{
        ScoreCtrl.EmptyClick();
	}

    // REGISTER CLICK ON THE WORD (SCORE)
	GameObject hitFX;
    bool isPerfect;
    void RegisterWordHit(ClickAreaCtrl cArea, WordGameObjectCtrl wrd){
		hitFX = PoolManager.SpawnObject(successWordHitFX);
        HitWordFX hitWordFX = hitFX.GetComponent<HitWordFX>();
        isPerfect = Mathf.Abs(currentAudioTime - wrd.hitTime) < Config.CLICK_PERFECT_TIME_OFFSET;
        if(isPerfect)
            hitWordFX.perfect = true;
        else
            hitWordFX.perfect = false;
		hitFX.transform.position = cArea.gameObject.transform.position;
        hitWordFX.runAnim();
        ScoreCtrl.WordHit(wrd.orderIndex, currentAudioTime, wrd.hitTime, isPerfect);
	}

	void RegisterHoldingWord(ClickAreaCtrl cAreaCtrl){
		
	}
}