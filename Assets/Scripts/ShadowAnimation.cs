using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowAnimation : MonoBehaviour {

	// Use this for initialization
	void OnEnable () {
		StartCoroutine (Animate ());
	}
	
	IEnumerator Animate()
	{
		RectTransform trans = (RectTransform)transform;
		while (true) {
			trans.localPosition += Vector3.left;
			yield return new WaitForSeconds (0.01f);

			if (trans.localPosition.x < -600)
				trans.localPosition += Vector3.right * 1200f;
		}
	}
}
