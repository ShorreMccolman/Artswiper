using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour {
	public static MenuController Instance;
	void Awake()
	{if(Instance == null) Instance = this;}

	Stack<Menu> _stack = new Stack<Menu>();

	void Start() {
		Init ();
	}

	public bool MenuOpen()
	{return _stack.Count > 0;}

	public void Init()
	{
		_stack = new Stack<Menu> ();
	}

	public void ClearMenus()
	{
		_stack = new Stack<Menu> ();
	}

	public void OpenMenu(Menu menu, bool closePrevious = true)
	{
		if (_stack.Count > 0) {
			if (_stack.Peek () == menu)
				return;

			if (closePrevious)
				_stack.Peek ().Close ();
			else
				_stack.Peek ().isOpen = false;
		}

		if (_stack.Count == 0) {

		}
		
		menu.Open ();
		_stack.Push (menu);
	}

	public void CloseCurrent()
	{
		if (_stack.Count == 0)
			return;

		_stack.Pop ().Close ();

		if (_stack.Count == 0) {

		} else {
			_stack.Peek ().Open (false);
		}
	}

	public void UnfocusCurrent()
	{
		if (_stack.Count == 0)
			return;

		_stack.Pop ().Unfocus();

		if (_stack.Count == 0) {

		} else {
			_stack.Peek ().Focus ();
		}
	}

	public void CloseAll()
	{
		if (_stack.Count == 0)
			return;
		for( int i=0;i<_stack.Count + 1;i++) {
			_stack.Pop ().Close ();
		}
		_stack = new Stack<Menu> ();
	}
		
	public void ShakeMenu(Menu menu)
	{
		StartCoroutine (Shake (menu.contents));
	}

	public void ShakeTransform(Transform trans)
	{
		StartCoroutine (Shake (trans));
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
