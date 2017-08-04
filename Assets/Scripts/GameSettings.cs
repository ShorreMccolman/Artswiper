using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings {
	public const string SOUND_VOLUME = "SndVlm";
	public const string MUSIC_VOLUME = "MscVlm";
	public const string HINT_TOGGLE = "HntTggl";

	private static float soundVolume;
	public static float SoundVolume
	{
		get{return soundVolume;}
		set{
			if (value < 0f)
				soundVolume = 0f;
			else if (value > 1f)
				soundVolume = 1f;
			else
				soundVolume = value;
		}
	}

	private static float musicVolume;
	public static float MusicVolume
	{
		get{return musicVolume;}
		set{
			if (value < 0f)
				musicVolume = 0f;
			else if (value > 1f)
				musicVolume = 1f;
			else
				musicVolume = value;
		}
	}

	private static bool showHint;
	public static bool ShowHint
	{
		get{return showHint;}
		set{
			showHint = value;
		}
	}

	public static void CreateSettings()
	{
		SoundVolume = 1f;
		MusicVolume = 1f;
		ShowHint = true;

		GameSettings.SaveSettings ();
	}

	public static void SaveSettings()
	{
		PlayerPrefs.SetFloat (SOUND_VOLUME, SoundVolume);
		PlayerPrefs.SetFloat (MUSIC_VOLUME, MusicVolume);
		PlayerPrefs.SetInt (HINT_TOGGLE, ShowHint ? 1 : 0);

		PlayerPrefs.Save ();
	}

	public static void LoadSettings()
	{
		SoundVolume = PlayerPrefs.GetFloat (SOUND_VOLUME, 1f);
		MusicVolume = PlayerPrefs.GetFloat (MUSIC_VOLUME, 1f);;
		ShowHint = PlayerPrefs.GetInt (HINT_TOGGLE, 0) == 1;
	}
}
