using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using LitJson;

public class TestGameManager : MonoBehaviour {

	[HideInInspector]
	public GameObject audioPrefab;
	public GameObject gameUI;
	public GameObject pickUI;
	public GameObject wordGameObjectPrefab;

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
	private List<GameObject> wordsCollection = new List<GameObject>();

	private Camera cam;
	private float distance;
	private float[] xPozitions;
	private float hitOffset = 0.2f;

	// Use this for initialization
	void Start () {
		cam = Camera.main;
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		distance = Screen.height;
		float screenQ = Screen.width/3;
		xPozitions = new float[4];
		xPozitions[1] = 0;
		xPozitions[1] = Mathf.Round(screenQ/2);
		xPozitions[2] = Mathf.Round(Screen.width/2);
		xPozitions[3] = Mathf.Round(screenQ*3 - screenQ/2);
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
		Debug.Log( string.Format("PREPARING : {0} file and {1} file", StaticDataManager.SelectedJsonName, StaticDataManager.SelectedAudioName) );
		gameUI.SetActive(true);
		listIndex = 0;
		StartCoroutine(LoadAudioAsset());
	}

	IEnumerator LoadAudioAsset(){
		string folderPath = Path.Combine(Application.persistentDataPath, StaticDataManager.AUDIO_FOLDER);
		string filePath = Path.Combine(folderPath, StaticDataManager.SelectedAudioName);
		Debug.Log( string.Format("PATH TO AUDIO FILE : {0}", filePath) );

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
		Debug.Log("TIME ON SCREEN " + timeOnScreen);

		wordsCollection.Clear();
		GameObject wordGameObject;
		WordGameObjectCtrl wordCtrl;

		Debug.Log("songData.wordsList.Length " + songData.wordsList.Length);
		for(int i=0; i<songData.wordsList.Length; i++)
		{
			wordGameObject = Instantiate(wordGameObjectPrefab, gameObject.transform);
			wordCtrl = wordGameObject.GetComponent<WordGameObjectCtrl>();
			wordCtrl.existanceTime = timeOnScreen;
			wordCtrl.SetData(songData.wordsList[i]);
			wordCtrl.startPosition = cam.ScreenToWorldPoint( new Vector3(xPozitions[wordCtrl.wordData.index], Screen.height, 10));
			wordCtrl.endPosition = cam.ScreenToWorldPoint( new Vector3(xPozitions[wordCtrl.wordData.index], 100, 10));
			wordsCollection.Add(wordGameObject);
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


	private GameObject currentWord;
	void Update () {
		
		if(!audioSource)
			return;

		if(!audioSource.isPlaying)
			return;

		currentAudioTime = audioSource.timeSamples/sampleRate;

		if (currentAudioTime >= nextShowTime && listIndex<wordsCollection.Count)
		{
			currentWord = wordsCollection[listIndex];
			currentWord.SetActive(true);

			Debug.Log(listIndex + " of " + wordsCollection.Count);

			listIndex++;
			if(listIndex<wordsCollection.Count-1){
				nextShowTime = wordsCollection[listIndex].GetComponent<WordGameObjectCtrl>().showTime;
			}
		}

		//moving
		WordGameObjectCtrl wordCtrl;
		float rangeTime;
		float percentTime;
		float newY;
		foreach(GameObject gWord in wordsCollection)
		{
			if (gWord.activeSelf)
			{
				wordCtrl = gWord.GetComponent<WordGameObjectCtrl>();
				rangeTime = wordCtrl.hitTime - wordCtrl.showTime;
				percentTime = (currentAudioTime - wordCtrl.showTime) / rangeTime;
				//newY = distance - (distance * percentTime);
				//gWord.transform.localPosition = Camera.main.ScreenToWorldPoint(new Vector3(xPozitions[wordCtrl.wordData.index], newY, 10));
				gWord.transform.localPosition = Vector3.Lerp(wordCtrl.startPosition, wordCtrl.endPosition, percentTime);
				if(currentAudioTime >= wordCtrl.hitTime-hitOffset){
					wordCtrl.textMesh.color = Color.green;
				}

			}
		}


	}
}
