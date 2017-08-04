using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour {
	public static MenuController Instance;
	void Awake()
	{Instance = this;}
	
	static Stack<Menu> stack = new Stack<Menu>();
	public static Stack<Menu> Stack
	{
		get { return stack; }
	}
		
	public static bool IsAMenuOpen() {
		return Stack.Count > 0;
	}

	public static void OpenMenu(Menu menu, bool closePrevious = true)
	{
		if (Stack.Count > 0) {
			if (Stack.Peek () == menu)
				return;

			if (closePrevious)
				Stack.Peek ().Close ();
			else
				Stack.Peek ().isOpen = false;
		}

		if (Stack.Count == 0) {

		}
		
		menu.Open ();
		Stack.Push (menu);
	}

	public static void CloseCurrent()
	{
		if (Stack.Count == 0)
			return;

		Stack.Pop ().Close ();

		if (Stack.Count == 0) {

		} else {
			Stack.Peek ().Open (false);
		}
	}

	public static void UnfocusCurrent()
	{
		if (Stack.Count == 0)
			return;

		Stack.Pop ().Unfocus();

		if (Stack.Count == 0) {

		} else {
			Stack.Peek ().Focus ();
		}
	}

	public static void CloseAll()
	{
		if (Stack.Count == 0)
			return;
		for( int i=0;i<Stack.Count + 1;i++) {
			Stack.Pop ().Close ();
		}
		Stack.Clear ();
	}
		
	public static void ShakeMenu(Menu menu)
	{
		Instance.StartCoroutine (Instance.Shake (menu.contents));
	}

	public static void ShakeTransform(Transform trans)
	{
		Instance.StartCoroutine (Instance.Shake (trans));
	}

	IEnumerator Shake(Transform trans)
	{
		float elapsed = 0.0f;
		float duration = 0.5f;
		float magnitude = 2.0f;

		Vector3 origPos = trans.localPosition;

		while(elapsed < duration) {
			elapsed += Time.deltaTime;

			float percentComplete = elapsed / duration;
			float damper = 1.0f - Mathf.Clamp(4.0f * percentComplete - 3.0f, 0.0f, 1.0f);

			float x = Random.value * 2.0f - 1.0f;
			float y = Random.value * 2.0f - 1.0f;
			x *= magnitude * damper;
			y *= magnitude * damper;

			trans.localPosition = origPos + new Vector3 (x, y, origPos.z);

			yield return null;
		}

		trans.localPosition = origPos;
	}
}
