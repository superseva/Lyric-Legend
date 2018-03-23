/* USE THIS CLASS TO REMEBER DATA OVER DIFFERENT SCENES */
public static class StaticDataManager {

	public static string JSON_FILE_TYPE = "json";
	public static string ASSET_FILE_TYPE = "unity3d";

	public static string JSON_FOLDER = "jsons";
	public static string AUDIO_FOLDER = "assetbundles";

	private static string selectedJsonName;
	private static string selectedAudioName;

	public static string SelectedJsonName{
		get 
		{
			return selectedJsonName;
		}
		set 
		{
			selectedJsonName = value;
		}
	}

	public static string SelectedAudioName{
		get 
		{
			return selectedAudioName;
		}
		set 
		{
			selectedAudioName = value;
		}
	}
}
