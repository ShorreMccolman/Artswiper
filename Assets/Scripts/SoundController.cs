using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SoundClip
{
	[SerializeField]
	private string clipID;
	public string ClipID
	{
		get{return clipID;}
	}

	[SerializeField, Range(0f,1f)]
	private float volume;
	public float Volume
	{
		get{return volume;}
	}

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

	[SerializeField]
	private SoundClip[] soundClips;
	Dictionary<string,SoundClip> soundDict = new Dictionary<string, SoundClip>();

	[SerializeField]
	private AudioSource musicSource;
	[SerializeField]
	private AudioClip menuMusic;

	[SerializeField]
	private float masterMusicVolume;

	[SerializeField]
	private GameObject sourceObject;
	Stack<AudioSource> availableSources = new Stack<AudioSource>();

	// Use this for initialization
	void Start () {
		soundDict = new Dictionary<string, SoundClip> ();
		for(int i=0;i<soundClips.Length;i++) {
			if(!string.IsNullOrEmpty(soundClips[i].ClipID))
				soundDict.Add (soundClips [i].ClipID, soundClips [i]);
		}

		availableSources = new Stack<AudioSource> ();
	}

	public static void ChangeSoundVolume(float val)
	{
		GameSettings.SoundVolume = val;
	}

	public static void ChangeMusicVolume(float val)
	{
		GameSettings.MusicVolume = val;
		Instance.musicSource.volume = val * Instance.masterMusicVolume;
	}

	public static void StartMusic()
	{
		Instance.musicSource.clip = Instance.menuMusic;
		Instance.StartCoroutine (Instance.FadeMusic (true));
	}
		
	public static void PauseMusic(float duration = 0.0f)
	{
		Instance.musicSource.volume = 0.0f;
		if(duration > 0.0f) {
			Instance.Invoke ("ResumeMusic", duration);
		}
	}

	public static void UnPauseMusic()
	{
		Instance.musicSource.volume = GameSettings.MusicVolume * Instance.masterMusicVolume;
	}

	public static void PlaySoundEffect(string clipID, bool randomPitch = false)
	{
		SoundClip sound = Instance.GetClipForID (clipID);
		if(sound.Clip == null) {
			return;
		}

		AudioSource soundSource = null;
		if(Instance.availableSources.Count == 0) {
			soundSource = Instance.sourceObject.AddComponent<AudioSource> ();
		} else {
			soundSource = Instance.availableSources.Pop ();
		}

		if (randomPitch)
			soundSource.pitch = Random.Range (0.8f, 1.2f);
		else
			soundSource.pitch = 1.0f;

		Instance.StartCoroutine (Instance.PlaySound (soundSource, sound));
	}

	public void ResumeMusic()
	{
		musicSource.clip = menuMusic;
		StartCoroutine (FadeMusic (true));
	}

	SoundClip GetClipForID(string id)
	{
		SoundClip sound;
		soundDict.TryGetValue (id, out sound);

		if(sound.Clip == null) {
			Debug.LogError ("Could not find sound clip with id " + id);
		}

		return sound;
	}

	IEnumerator PlaySound(AudioSource source, SoundClip sound)
	{
		source.volume = sound.Volume * GameSettings.SoundVolume;
		source.clip = sound.Clip;
		source.Play ();
		while(source.isPlaying) {
			yield return null;
		}
		availableSources.Push (source);
	}

	public IEnumerator FadeMusic(bool fadeIn)
	{
		if (fadeIn) {
			musicSource.Play ();
			while (musicSource.volume < GameSettings.MusicVolume * masterMusicVolume) {
				musicSource.volume += Time.deltaTime * 0.5f;
				yield return null;
			}
			musicSource.volume = GameSettings.MusicVolume * masterMusicVolume;
		} else {
			while (musicSource.volume > 0.01f) {
				musicSource.volume -= Time.deltaTime * 0.5f;
				yield return null;
			}
			musicSource.volume = 0f;
		}
	}
}
