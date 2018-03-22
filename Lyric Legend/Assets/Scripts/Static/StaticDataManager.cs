/* USE THIS CLASS TO REMEBER DATA OVER DIFFERENT SCENES */
public static class StaticDataManager {

	public static string JSON_FILE_TYPE = "json";
	public static string WAV_FILE_TYPE = "wav";
	public static string MP3_FILE_TYPE = "mp3";

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
