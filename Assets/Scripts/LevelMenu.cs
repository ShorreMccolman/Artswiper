using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelMenu : Menu {
	public static LevelMenu Instance;
	void Awake()
	{Instance = this; base.Awake ();}

	public Transform anchor;

	public Text[] titleLabels;

	public Image levelInfoBg;
	public Text[] levelNameLabels;
	public Text authorLabel;
	public Text minesLabel;
	public Text timeLabel;
	public Text scoreLabel;
	public Text checkmark;

	[HideInInspector]
	public World currentWorld;
	List<LevelSelectButton> levelButtons = new List<LevelSelectButton>();

	protected override void OnOpen()
	{
		foreach(LevelSelectButton button in levelButtons) {
			Destroy (button.gameObject);
		}
		levelButtons = new List<LevelSelectButton> ();

		bool hasFourMaps = currentWorld.maps.Length == 4;
		string but = hasFourMaps ? "LevelSelectButton" : "LevelSelectButtonSmall";

		foreach (Text label in titleLabels)
			label.text = currentWorld.displayName.ToUpper();

		string nextKey = "";
		for (int i = 0; i < currentWorld.maps.Length; i++) {
			currentWorld.maps [i].currentIndex = i;
			currentWorld.maps [i].keyID = nextKey;
			nextKey = currentWorld.maps [i].name;
			if (i == currentWorld.maps.Length - 1) {
				currentWorld.maps [i].isFinalLevel = true;
			}

			GameObject obj = Instantiate (Resources.Load (but)) as GameObject;
			LevelSelectButton button = obj.GetComponent<LevelSelectButton> ();
			button.Init (currentWorld.maps [i]);

			button.transform.parent = anchor;
			if (hasFourMaps) {
				button.transform.localPosition = new Vector3 (i * 200f - 300f, 0f, 0f);
			} else {
				button.transform.localPosition = new Vector3 (i * 100f - 350f, 25f - (i % 2) * 50f, 0f);
			}
			button.transform.localScale = Vector3.one;

			levelButtons.Add (button);
		}

		foreach(Text levelNameLabel in levelNameLabels)
			levelNameLabel.text = currentWorld.displayName;
			
		levelInfoBg.CrossFadeAlpha (0f, 0f, true);
		Image[] images = levelInfoBg.GetComponentsInChildren<Image> ();
		foreach (Image image in images)
			image.CrossFadeAlpha (0f, 0f, true);
		Text[] texts = levelInfoBg.GetComponentsInChildren<Text> ();
		foreach (Text text in texts)
			text.CrossFadeAlpha (0f, 0f, true);
	}

	public void ShowLevelInfo(Map map)
	{
		levelInfoBg.CrossFadeAlpha (1f, 0.2f, true);
		Image[] images = levelInfoBg.GetComponentsInChildren<Image> ();
		foreach (Image image in images)
			image.CrossFadeAlpha (1f, 0.2f, true);
		Text[] texts = levelInfoBg.GetComponentsInChildren<Text> ();
		foreach (Text text in texts)
			text.CrossFadeAlpha (1f, 0.2f, true);

		foreach(Text levelNameLabel in levelNameLabels)
			levelNameLabel.text = map.title;
		authorLabel.text = "by " + map.author;
		scoreLabel.text = "Best Score: " + PlayerProgression.GetBestScore(map.name);
		timeLabel.text = "Time Limit: " + UIHelpers.ConvertToSecondsTimeString (map.timeLimit);
		minesLabel.text = "Mines: " + map.difficulty.ToString ();

		if(PlayerProgression.HasCompletedLevel(map.name)) {
			checkmark.enabled = true;
		} else {
			checkmark.enabled = false;
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

	public void SelectLevel(Map map)
	{
		GameController.Instance.StartAdventureGame (currentWorld, map);
	}

	public void GoBack()
	{
		MenuController.CloseCurrent ();
	}
}
