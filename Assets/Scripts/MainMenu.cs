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
		MenuController.Instance.OpenMenu (DifficultyPopup.Instance, false);
	}

	public void StartAdventure()
	{
		MenuController.Instance.OpenMenu (WorldMenu.Instance, false);
	}

	public void OpenStash()
	{
		MenuController.Instance.OpenMenu (StashMenu.Instance);
	}

	public void OpenSettingsPopup()
	{
		MenuController.Instance.OpenMenu (SettingsPopup.Instance, false);
	}

	public void OpenAboutPopup()
	{
		MenuController.Instance.OpenMenu (AboutPopup.Instance, false);
	}

	public void QuitGame()
	{
		AppManager.Instance.QuitApp ();
	}
}
