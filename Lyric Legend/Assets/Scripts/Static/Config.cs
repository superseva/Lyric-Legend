using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using LitJson;

public static class Config {

    public static float LYRIC_START_POINT_IN_PERCENT;
    public static float LYRIC_END_POINT_IN_PERCENT;
    public static float STAGE_VISUAL_POSITION_IN_PERCENT;

    public static float CLICK_PERFECT_TIME_OFFSET;
    public static float POINT_VALUE;
    public static float STREAK_MULTIPLIER;
    public static float MINIMUM_TIME_DIFFERENCE_IN_LANE;

    private static JsonData configData;


    public static bool ConfigGeneral () 
    {
        bool isDone = false;
        string path = Path.Combine(Application.persistentDataPath, "config.json");
        if (!File.Exists(path))
        {
            //Debug.Log("NO CONFIG");
            isDone = false;
            return isDone;
        }
        configData = JsonMapper.ToObject<JsonData>(File.ReadAllText(path));

        LYRIC_START_POINT_IN_PERCENT = (float)configData["POSITIONING"]["LYRIC_START_POINT_IN_PERCENT"];
        LYRIC_END_POINT_IN_PERCENT = (float)configData["POSITIONING"]["LYRIC_END_POINT_IN_PERCENT"];
        STAGE_VISUAL_POSITION_IN_PERCENT = (float)configData["POSITIONING"]["STAGE_VISUAL_POSITION_IN_PERCENT"];
        MINIMUM_TIME_DIFFERENCE_IN_LANE = (float)configData["POSITIONING"]["MINIMUM_TIME_DIFFERENCE_IN_LANE"];

        // SCORING CONFIG
        POINT_VALUE = (float)configData["SCORING"]["POINT_VALUE"];
        if (StaticDataManager.difficulty == 2)
        {
            STREAK_MULTIPLIER = (float)configData["SCORING"]["STREAK_MULTIPLIER_HARD"];
        }
        else
        {
            STREAK_MULTIPLIER = (float)configData["SCORING"]["STREAK_MULTIPLIER_NORMAL"];
        }
        CLICK_PERFECT_TIME_OFFSET = (float)configData["SCORING"]["CLICK_PERFECT_TIME_OFFSET"];

        isDone = true;
        return isDone;
	}
}
