using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : Menu {
	public static MainMenu Instance;
	void Awake()
	{Instance = this; base.Awake ();}

	protected override void OnOpen()
	{
		GameController.Instance.currentState.currentMode = GameMode.None;
	}

	public void StartClassic()
	{
		MenuController.OpenMenu (DifficultyPopup.Instance, false);
	}

	public void StartAdventure()
	{
		MenuController.OpenMenu (WorldMenu.Instance, false);
	}

	public void OpenStash()
	{
		MenuController.OpenMenu (StashMenu.Instance);
	}

	public void OpenSettingsPopup()
	{
		MenuController.OpenMenu (SettingsPopup.Instance, false);
	}

	public void OpenAboutPopup()
	{
		MenuController.OpenMenu (AboutPopup.Instance, false);
	}

	public void QuitGame()
	{
		AppManager.Instance.QuitApp ();
	}
}
