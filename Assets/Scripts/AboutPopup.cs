using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AboutPopup : Menu {
	public static AboutPopup Instance;
	protected override void Awake ()
	{base.Awake ();Instance = this;}

	public void ClosePopup()
	{
		MenuController.Instance.CloseCurrent ();
	}
}
