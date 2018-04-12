using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using LitJson;

public class GameLogger : MonoBehaviour {


    public static List<float> wordsTime = new List<float>();
    public static List<float> currentTimes = new List<float>();
    public static List<string> wordList = new List<string>();

    public static List<string> tapStrings = new List<string>();

    public static void Reset()
    {
        wordsTime.Clear();
        currentTimes.Clear();
        wordList.Clear();
        tapStrings.Clear();
    }

    public static void AddTap(float wordTime, float currentTime, string word)
    {
        wordsTime.Add(wordTime);
        currentTimes.Add(currentTime);
        wordList.Add(word);
    }

    static string stringLine;
    public static void ExportToDisk()
    {
        int count = wordsTime.Count;
        bool perfect = false;
        float diff;
        for (int i = 0; i < count; i++){
            diff = wordsTime[i] - currentTimes[i];
            perfect = Mathf.Abs(diff) < Config.CLICK_PERFECT_TIME_OFFSET;
            stringLine = string.Format("{0} : {1} -- Audio Time: {2} -- Difference: {3} -- perfect={4}",wordList[i], wordsTime[i], currentTimes[i], diff, perfect);
            tapStrings.Add(stringLine);
            //Debug.Log(stringLine);
        }

        GamePlayLog gm = new GamePlayLog();
        gm.tapStrings = tapStrings;
        gm.perfectCount = ScoreCtrl.perfectCount;
        gm.nonPerfectCount = ScoreCtrl.nonPerfectCount;
        gm.missCount = ScoreCtrl.missCount;

        string gmString = JsonMapper.ToJson(gm);

        string path = Path.Combine(Application.persistentDataPath, "GamePlayLog.json");
        File.WriteAllText(path, gmString);

    }


}

[Serializable]
public class GamePlayLog
{

    public int perfectCount;
    public int nonPerfectCount;
    public int missCount;
    public List<string> tapStrings;

}

