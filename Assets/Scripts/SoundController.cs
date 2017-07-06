using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SoundClip
{
	public string clipID;
	[Range(0f,1f)]
	public float volume;

	AudioClip clip;
	public AudioClip Clip
	{
		get {
			if(clip == null) {
				Debug.Log ("Loading sound with id " + clipID);
				clip = Resources.Load ("Sounds/" + clipID) as AudioClip;
			}
			if(clip == null) {
				Debug.LogError ("Could not find sound with path " + clipID);
			}
				
			return clip;
		}
	}
}

public static class Sounds {
	public const string CLICK = "Click";
	public const string TICK = "Tick";
	public const string HOVER = "Hover";
	public const string ALARM = "Alarm";
	public const string FLAG = "Flag";
	public const string FLIP = "Flip";
	public const string POP = "Pop";
	public const string HIT = "Hit";
	public const string VICTORY = "Win";
	public const string DEFEAT = "Loss";
}

public class SoundController : MonoBehaviour {
	public static SoundController Instance;
	void Awake()
	{if(Instance == null) Instance = this;}

	public SoundClip[] soundClips;
	Dictionary<string,SoundClip> soundDict = new Dictionary<string, SoundClip>();

	public AudioSource musicSource;
	public AudioClip menuMusic;
	public AudioClip gameMusic;

	public float masterMusicVolume;

	public GameObject sourceObject;
	public AudioSource[] soundSources;
	Stack<AudioSource> availableSources = new Stack<AudioSource>();

	// Use this for initialization
	void Start () {
		soundDict = new Dictionary<string, SoundClip> ();
		for(int i=0;i<soundClips.Length;i++) {
			if(!string.IsNullOrEmpty(soundClips[i].clipID))
				soundDict.Add (soundClips [i].clipID, soundClips [i]);
		}

		availableSources = new Stack<AudioSource> ();
		for(int i=0;i<soundSources.Length;i++) {
			availableSources.Push (soundSources [i]);
		}
	}

	public void SetGameVolume(float val)
	{
		AppManager.settings.soundVolume = val;
		foreach(AudioSource source in soundSources) {
			source.volume = val;
		}
	}

	public void SetMusicVolume(float val)
	{
		AppManager.settings.musicVolume = val;
		musicSource.volume = val * masterMusicVolume;
	}

	public IEnumerator FadeMusic(bool fadeIn)
	{
		if (fadeIn) {
			musicSource.Play ();
			while (musicSource.volume < AppManager.settings.musicVolume * masterMusicVolume) {
				musicSource.volume += Time.deltaTime * 0.5f;
				yield return null;
			}
			musicSource.volume = AppManager.settings.musicVolume * masterMusicVolume;
		} else {
			while (musicSource.volume > 0.01f) {
				musicSource.volume -= Time.deltaTime * 0.5f;
				yield return null;
			}
			musicSource.volume = 0f;
		}
	}

	public void StartMenuMusic()
	{
		musicSource.clip = menuMusic;
		StartCoroutine (FadeMusic (true));
	}

	public void StartGameMusic()
	{
		musicSource.clip = gameMusic;
		musicSource.volume = AppManager.settings.musicVolume * masterMusicVolume;
		musicSource.Play ();
	}

	public void PauseMusic(float duration = 0.0f)
	{
		musicSource.volume = 0.0f;
		if(duration > 0.0f) {
			Invoke ("StartMenuMusic", duration);
		}
	}
	public void UnPauseMusic()
	{
		musicSource.volume = AppManager.settings.musicVolume * masterMusicVolume;
	}

	public SoundClip GetClipForID(string id)
	{
		SoundClip sound = null;
		soundDict.TryGetValue (id, out sound);

		if(sound == null) {
			Debug.LogError ("Could not find sound clip with id " + id);
			return null;
		}

		return sound;
	}

	public void PlaySoundEffect(string clipID, bool randomPitch = false)
	{
		SoundClip sound = GetClipForID (clipID);
		if(sound == null || sound.Clip == null) {
			return;
		}

		AudioSource soundSource = null;
		if(availableSources.Count == 0) {
			soundSource = sourceObject.AddComponent<AudioSource> ();
		} else {
			soundSource = availableSources.Pop ();
		}

		if (randomPitch)
			soundSource.pitch = Random.Range (0.8f, 1.2f);
		else
			soundSource.pitch = 1.0f;

		StartCoroutine (PlaySound (soundSource, sound));
	}

	IEnumerator PlaySound(AudioSource source, SoundClip sound)
	{
		source.volume = sound.volume * AppManager.settings.soundVolume;
		source.clip = sound.Clip;
		source.Play ();
		while(source.isPlaying) {
			yield return null;
		}
		availableSources.Push (source);
	}
}
