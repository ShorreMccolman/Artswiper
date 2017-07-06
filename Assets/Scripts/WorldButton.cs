using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldButton : GenericMenuButton {

	public World world;
	public World worldToComplete;

	bool enabled = true;

	public void OnEnable()
	{
		if(!PlayerProgression.HasCompletedWorld(worldToComplete)) {
			enabled = false;
			GetComponent<Image> ().color = Color.gray;
		} else {
			enabled = true;
			GetComponent<Image> ().color = Color.white;
		}
	}

	public void SelectWorld()
	{
		if(!enabled) {
			DialogPopup.ShowDialog ("Gallery Locked", "You need to complete the " + worldToComplete.displayName + " Gallery to unlock this one!");
			return;
		}
		WorldMenu.Instance.SelectWorld (world);
	}
}
