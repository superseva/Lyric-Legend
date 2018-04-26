using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeWordsInjector : MonoBehaviour {


    public static void Inject(SongData songData){
        Debug.Log("INJECTING");
        int frequency = Mathf.CeilToInt(songData.wordsList.Count / songData.fakeWords.Count);

        WordData wordData;
        int index = 0;
        for (int i = 0; i < songData.fakeWords.Count;i++)
        {
            index = Mathf.FloorToInt((i + 1) * (frequency)) - Mathf.FloorToInt(frequency * Random.Range(0.2f, 0.8f));
            wordData = songData.fakeWords[i];
            wordData.time = songData.wordsList[index].time;
            wordData.isFake = true;
            songData.wordsList.Insert(index+1, songData.fakeWords[i]);
        }

    }
}
