using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class UIHelpers {

	public static string ConvertToMillisecondsTimeString(float time)
	{
		float mins = time / 60;
		int milli = (int)((time - (int)time) * 100);

		return ((int)mins).ToString("00") + ":" + ((int)((int)time % 60)).ToString("00") + ":" + milli.ToString("00");
	}

	public static string ConvertToSecondsTimeString(float time)
	{
		float mins = time / 60;

		return ((int)mins).ToString("0") + ":" + ((int)((int)time % 60)).ToString("00");
	}

	public static IEnumerator FadeTo(GameObject obj, float aVal, float time, bool hideAfter = false)
	{
		Image[] images = obj.GetComponentsInChildren<Image>();
		if (images.Length > 0) {
		
			float alpha = images[0].color.a;
			for (float t=0.0f; t < 1.0f; t += Time.deltaTime / time)
			{
				Color color = new Color (1f, 1f, 1f, Mathf.Lerp (alpha, aVal, t));
				foreach(Image image in images)
					image.color = color;
				yield return null;
			}

			if (hideAfter) {
				foreach(Image image in images)
					image.enabled = false;
			}
		}
	}

	public static IEnumerator FadeToBlack(GameObject obj, float aVal, float time, bool hideAfter = false)
	{
		Image[] images = obj.GetComponentsInChildren<Image>();
		if (images.Length > 0) {

			float alpha = images [0].color.a;
			for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / time) {
				Color color = new Color (0f, 0f, 0f, Mathf.Lerp (alpha, aVal, t));
				foreach (Image image in images)
					image.color = color;
				yield return null;
			}

			if (hideAfter) {
				foreach (Image image in images)
					image.enabled = false;
			}
		}
	}



	public static IEnumerator FadeTo(Image image, float aVal, float time, bool hideAfter = false)
	{
		float alpha = image.color.a;
		for (float t=0.0f; t < 1.0f; t += Time.deltaTime / time)
		{
			Color color = new Color (1f, 1f, 1f, Mathf.Lerp (alpha, aVal, t));
			image.color = color;
			yield return null;
		}

		if (hideAfter)
			image.enabled = false;
	}

	public static IEnumerator FadeToBlack(Image image, float aVal, float time, bool hideAfter = false)
	{
		float alpha = image.color.a;
		for (float t=0.0f; t < 1.0f; t += Time.deltaTime / time)
		{
			Color color = new Color (0f, 0f, 0f, Mathf.Lerp (alpha, aVal, t));
			image.color = color;
			yield return null;
		}

		if (hideAfter)
			image.enabled = false;
	}
}
