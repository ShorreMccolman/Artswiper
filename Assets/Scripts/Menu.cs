using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour {

	[HideInInspector]public bool isOpen = false;
	public bool isPopup = false;

	Vector3 targetScale;

	void Update()
	{
		if(isOpen && isPopup) {
			transform.localScale = Vector3.Lerp (transform.localScale, targetScale, 10*Time.deltaTime);
		}
	}

	[HideInInspector]public Transform contents;
	protected virtual void Awake()
	{
		contents = transform.GetChild (0);
	}

	public void Open(bool shouldPop = true){
		contents.gameObject.SetActive (true);

		isOpen = true;

		if (isPopup && shouldPop) {
			transform.localScale = Vector3.zero;
			targetScale = Vector3.one;
			SoundController.Instance.PlaySoundEffect (Sounds.POP, true);
		}

		OnOpen ();
	}
	protected virtual void OnOpen(){}

	public void Close(){
		contents.gameObject.SetActive (false);

		isOpen = false;

		OnClose ();
	}
	protected virtual void OnClose(){}

	public void Focus()
	{
		isOpen = true;

		OnFocus ();
	}
	protected virtual void OnFocus(){}

	public void Unfocus()
	{
		isOpen = false;

		OnUnfocus ();
	}
	protected virtual void OnUnfocus(){}
}
