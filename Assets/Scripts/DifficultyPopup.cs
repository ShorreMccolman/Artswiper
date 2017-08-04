using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyPopup : Menu {
	public static DifficultyPopup Instance;
	void Awake()
	{Instance = this; base.Awake ();}

	public Text[] scores;
	public Image levelInfoBg;

	bool chosen = false;

	protected override void OnOpen()
	{
		chosen = false;
		levelInfoBg.CrossFadeAlpha (0f, 0f, true);
		Image[] images = levelInfoBg.GetComponentsInChildren<Image> ();
		foreach (Image image in images)
			image.CrossFadeAlpha (0f, 0f, true);
		Text[] texts = levelInfoBg.GetComponentsInChildren<Text> ();
		foreach (Text text in texts)
			text.CrossFadeAlpha (0f, 0f, true);
	}

	public void ChooseMap(string map)
	{
		if (!chosen) {
			chosen = true;
			GameController.Instance.StartClassicGame (map);
		}
	}
		
	public void ShowLevelInfo(string diff)
	{
		levelInfoBg.CrossFadeAlpha (1f, 0.2f, true);
		Image[] images = levelInfoBg.GetComponentsInChildren<Image> ();
		foreach (Image image in images)
			image.CrossFadeAlpha (1f, 0.2f, true);
		Text[] texts = levelInfoBg.GetComponentsInChildren<Text> ();
		foreach (Text text in texts)
			text.CrossFadeAlpha (1f, 0.2f, true);

		HighscoreSet scoreSet = PlayerProgression.GetClassicModeScores (diff);
		if(scoreSet.first > 0) {
			float time = ((float)scoreSet.first / 10);
			scores[0].text = UIHelpers.ConvertToSecondsTimeString(time);
		} else {
			scores[0].text = "--:--";
		}
		if(scoreSet.second > 0) {
			float time = ((float)scoreSet.second / 10);
			scores[1].text = UIHelpers.ConvertToSecondsTimeString(time);
		} else {
			scores[1].text = "--:--";
		}
		if(scoreSet.third > 0) {
			float time = ((float)scoreSet.third / 10);
			scores[2].text = UIHelpers.ConvertToSecondsTimeString(time);
		} else {
			scores[2].text = "--:--";
		}
		if(scoreSet.fourth > 0) {
			float time = ((float)scoreSet.fourth / 10);
			scores[3].text = UIHelpers.ConvertToSecondsTimeString(time);
		} else {
			scores[3].text = "--:--";
		}
		if(scoreSet.fifth > 0) {
			float time = ((float)scoreSet.fifth / 10);
			scores[4].text = UIHelpers.ConvertToSecondsTimeString(time);
		} else {
			scores[4].text = "--:--";
		}
	}

	public void HideLevelInfo()
	{
		levelInfoBg.CrossFadeAlpha (0f, 0.2f, true);
		Image[] images = levelInfoBg.GetComponentsInChildren<Image> ();
		foreach (Image image in images)
			image.CrossFadeAlpha (0f, 0.2f, true);
		Text[] texts = levelInfoBg.GetComponentsInChildren<Text> ();
		foreach (Text text in texts)
			text.CrossFadeAlpha (0f, 0.2f, true);
	}
		
	public void ClosePopup()
	{
		MenuController.CloseCurrent ();
	}
}
