using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogPopup : Menu {
	public static DialogPopup Instance;
	protected override void Awake ()
	{base.Awake ();Instance = this;}

	public Text[] titleLabels;
	public Text dialogLabel;

	public static void ShowDialog(string title, string message)
	{
		MenuController.OpenMenu (Instance, false);
		foreach(Text titleLabel in Instance.titleLabels)
			titleLabel.text = title;
		Instance.dialogLabel.text = message;
	}

	public static void ShowDialogOnce(string title, string message, string flag)
	{
		//PlayerPrefs.DeleteKey (flag);
		if (!PlayerProgression.HasFlag (flag)) {
			ShowDialog (title, message);
			PlayerProgression.SetFlag (flag);
		}
	}

	public void ClosePopup() {
		MenuController.CloseCurrent ();
	}
}
