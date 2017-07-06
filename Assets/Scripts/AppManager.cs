using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum AppState
{
	Init,
	Menu,
	Game
}

public class AppManager : MonoBehaviour {
	public static AppManager Instance;
	void Awake()
	{Instance = this;}

	public static AppState gamestate;
	public static GameSettings settings;

	public Image cover;
	public Text[] loadingTexts;

	void Start () {
		StartCoroutine (LaunchApp ());
	}

	IEnumerator LaunchApp()
	{
		Init ();
		yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync ("Main");
		yield return null; // Leaving a frame for new scene objects to finish init

		SlotPool.Instance.InstantiateSlotPool ();
		yield return null;

		SoundController.Instance.StartMenuMusic ();
		MenuController.Instance.OpenMenu (MainMenu.Instance);
		RevealScreen ();

		yield return null;
		DialogPopup.ShowDialogOnce (PlayerProgression.GREETING_TITLE, PlayerProgression.GREETING_MESSAGE, PlayerProgression.GREETING_VIEWED);
	}

	void Init()
	{
		gamestate = AppState.Init;

		DontDestroyOnLoad (this.gameObject);
		ShowSplashScreen ();

		if(PlayerProgression.ProfileExists()) {
			settings = GameSettings.LoadSettings ();
		} else {
			settings = GameSettings.CreateSettings ();
			PlayerProgression.CreateNewProfile ();
		}
	}

	public void ShowSplashScreen(float fadeTime = 0f)
	{
		cover.enabled = true;
		cover.CrossFadeAlpha (1f, fadeTime, false);
		CancelInvoke ();
	}

	public void HideScreen()
	{
		CancelInvoke ();
		cover.sprite = Resources.Load<Sprite> ("loading");
		cover.enabled = true;
		cover.CrossFadeAlpha (0f, 0f, false);
		cover.CrossFadeAlpha (1f, 1f, false);

		foreach (Text loadingText in loadingTexts) {
			loadingText.text = "Creating Map...";
			loadingText.enabled = true;
			loadingText.CrossFadeAlpha (0f, 0f, false);
			loadingText.CrossFadeAlpha (1f, 1f, false);
		}
	}

	public void RevealScreen()
	{
		cover.enabled = true;
		cover.CrossFadeAlpha (0f, 1f, false);
		foreach (Text loadingText in loadingTexts) {
			loadingText.CrossFadeAlpha (0f, 1f, false);
		}
		CancelInvoke ();
		Invoke ("DisableScreenCover", 2f);
	}
	void DisableScreenCover()
	{
		cover.enabled = false;
		foreach (Text loadingText in loadingTexts) {
			loadingText.enabled = false;
		}
	}

	public void QuitApp()
	{
		Application.Quit ();
	}
}
