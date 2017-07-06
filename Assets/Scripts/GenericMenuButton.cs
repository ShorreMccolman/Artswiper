using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenericMenuButton : MonoBehaviour {

	public Vector3 onHoverScale = new Vector3(1.1f,1.1f,1.0f);
	Vector3 targetScale = Vector3.one;

	[HideInInspector]public bool isActive = true;

	void OnEnable()
	{
		targetScale = Vector3.one;
		transform.localScale = Vector3.one;
	}

	public void OnMouseOver()
	{
		SoundController.Instance.PlaySoundEffect (Sounds.HOVER,true);
		targetScale = onHoverScale;
	}
	public void OffMouseOver()
	{
		targetScale = Vector3.one;
	}

	public void OnMouseClick()
	{
		SoundController.Instance.PlaySoundEffect (Sounds.CLICK, true);
	}

	void Update()
	{
		if(isActive)
			transform.localScale = Vector3.Lerp (transform.localScale, targetScale, 7*Time.deltaTime);
	}
}
