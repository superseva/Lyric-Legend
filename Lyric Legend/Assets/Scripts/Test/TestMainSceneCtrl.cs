using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class TestMainSceneCtrl : MonoBehaviour {


	private void Awake()
	{
        Application.targetFrameRate = 60;
	}
	// Use this for initialization
	void Start () {
		string pth1 = Path.Combine(Application.persistentDataPath,"assetbundles");
		string pth2 = Path.Combine(pth1,"girllikeyou.unity3d");
		Debug.Log("PATH TO LOAD ASSET FROM: " + pth2);
		//StartCoroutine(LoadObject(pth2));
	}

	public void LoadPlayTestScene()
	{
		SceneManager.LoadScene("TestPlay");
	}
	
	IEnumerator LoadObject(string path)
	{

		AssetBundleCreateRequest bundle = AssetBundle.LoadFromFileAsync(path);
		yield return bundle;

		AssetBundle myLoadedAssetBundle = bundle.assetBundle;
		if(myLoadedAssetBundle == null)
		{
			Debug.Log("Failed To Load Asset Bundle");
			yield break;
		}

		AssetBundleRequest request = myLoadedAssetBundle.LoadAssetAsync<GameObject>("GirlLikeYou");
		yield return request;

		GameObject pref = request.asset as GameObject;
		GameObject obj =  Instantiate(pref);

		obj.GetComponent<AudioSource>().Play();

		myLoadedAssetBundle.Unload(false);

	}
}
