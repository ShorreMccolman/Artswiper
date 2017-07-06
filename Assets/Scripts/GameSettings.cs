using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : ScriptableObject {

	public const string SOUND_VOLUME = "SndVlm";
	public const string MUSIC_VOLUME = "MscVlm";
	public const string HINT_TOGGLE = "HntTggl";

	public float soundVolume;
	public float musicVolume;
	public bool showHint;

	public static GameSettings CreateSettings()
	{
		GameSettings settings = new GameSettings ();
		settings.soundVolume = 1f;
		settings.musicVolume = 1f;
		settings.showHint = true;

		GameSettings.SaveSettings (settings);

		return settings;
	}

	public static void SaveSettings(GameSettings settings)
	{
		PlayerPrefs.SetFloat (SOUND_VOLUME, settings.soundVolume);
		PlayerPrefs.SetFloat (MUSIC_VOLUME, settings.musicVolume);
		PlayerPrefs.SetInt (HINT_TOGGLE, settings.showHint ? 1 : 0);

		PlayerPrefs.Save ();
	}

	public static GameSettings LoadSettings()
	{
		GameSettings settings = new GameSettings ();
		settings.soundVolume = PlayerPrefs.GetFloat (SOUND_VOLUME, 1f);
		settings.musicVolume = PlayerPrefs.GetFloat (MUSIC_VOLUME, 1f);;
		settings.showHint = PlayerPrefs.GetInt (HINT_TOGGLE, 0) == 1;

		return settings;
	}
}
