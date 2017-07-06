using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPopup : Menu {

	public static SettingsPopup Instance;
	void Awake()
	{Instance = this; base.Awake ();}

	public GameObject quitButton;
	public Slider gameVolSlider;
	public Slider musicVolSlider;

	public Text hintCheckmark;

	float soundVolume;
	float musicVolume;
	bool showingHint;

	protected override void OnOpen()
	{
		soundVolume = AppManager.settings.soundVolume;
		musicVolume = AppManager.settings.musicVolume;

		gameVolSlider.value = soundVolume;
		musicVolSlider.value = musicVolume;

		showingHint = AppManager.settings.showHint;
		hintCheckmark.enabled = showingHint;

		quitButton.SetActive (GameController.Instance.currentState.currentMode != GameMode.None);
	}

	protected override void OnClose()
	{
		GameSettings.SaveSettings (AppManager.settings);
	}

	public void AdjustGameVol(float val) {
		SoundController.Instance.SetGameVolume (val);
		soundVolume = val;
	}

	public void AdjustMusicVol(float val) {
		SoundController.Instance.SetMusicVolume (val);
		musicVolume = val;
	}

	public void ToggleHint()
	{
		showingHint = !showingHint;
		hintCheckmark.enabled = showingHint;
		AppManager.settings.showHint = showingHint;
	}

	public void QuitToMain() {
		HUD.Instance.running = false;
		MenuController.Instance.OpenMenu (MainMenu.Instance);
	}

	public void ClosePopup() {
		MenuController.Instance.CloseCurrent ();
	}
}
