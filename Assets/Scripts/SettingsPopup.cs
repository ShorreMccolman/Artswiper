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

	protected override void OnOpen()
	{
		gameVolSlider.value = GameSettings.SoundVolume;
		musicVolSlider.value = GameSettings.MusicVolume;
		hintCheckmark.enabled = GameSettings.ShowHint;

		quitButton.SetActive (GameController.CurrentState.currentMode != GameMode.None);
	}

	protected override void OnClose() {
		GameSettings.SaveSettings ();
	}

	public void AdjustGameVol(float val) {
		SoundController.ChangeSoundVolume (val);
	}

	public void AdjustMusicVol(float val) {
		SoundController.ChangeMusicVolume (val);
	}

	public void ToggleHint()
	{
		GameSettings.ShowHint = !GameSettings.ShowHint;
		hintCheckmark.enabled = GameSettings.ShowHint;
	}

	public void QuitToMain() {
		HUD.Running = false;
		MenuController.OpenMenu (MainMenu.Instance);
	}

	public void ClosePopup() {
		MenuController.CloseCurrent ();
	}
}
