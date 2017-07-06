using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMenu : Menu {
	public static WorldMenu Instance;
	void Awake()
	{Instance = this; base.Awake ();}

	public void SelectWorld(World world)
	{
		LevelMenu.Instance.currentWorld = world;
		MenuController.Instance.CloseCurrent ();
		MenuController.Instance.OpenMenu (LevelMenu.Instance);
	}

	public void GoBack()
	{
		MenuController.Instance.CloseCurrent ();
	}
}
