using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class TestScripts : MonoBehaviour {

	public static bool hasRan;
	public bool shouldClearPrefs;

	// Use this for initialization
	void Start () {
		#if UNITY_EDITOR
		if(shouldClearPrefs)
			PlayerPrefs.DeleteAll();

		if(!hasRan)
			Invoke("LoadMenu",0.05f);
		#endif
	}

	void LoadMenu()
	{
		if(!AppManager.Instance)
		{
			#if UNITY_EDITOR
			Assembly assembly = Assembly.GetAssembly(typeof(UnityEditor.SceneView));
			System.Type logEntries = assembly.GetType("UnityEditorInternal.LogEntries");
			MethodInfo clearConsole = logEntries.GetMethod("Clear");
			clearConsole.Invoke(new object(), null);
			#endif
			hasRan = true;
			UnityEngine.SceneManagement.SceneManager.LoadScene("Load");
			return;
		}
	}

}
