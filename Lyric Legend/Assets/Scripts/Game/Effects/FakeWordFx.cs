using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FakeWordFx : MonoBehaviour {

    private Image img;

	private void OnEnable()
	{
        UIEventManager.OnFakeWordHit += OnFakeWordHit;
	}

    private void OnDisable()
    {
        UIEventManager.OnFakeWordHit -= OnFakeWordHit;
    }

	void Start () {
        img = gameObject.GetComponent<Image>();
        img.color = new Color(0, 0, 0, 0);
	}
	
    void OnFakeWordHit(){
        img.color = Color.red;
        img.DOFade(0, 0.3f);
    }
}
