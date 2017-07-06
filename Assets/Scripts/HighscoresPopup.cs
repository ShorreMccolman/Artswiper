using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Difficulty
{
	Easy,
	Medium,
	Hard
}

public struct HighscoreSet
{
	public int first;
	public int second;
	public int third;
	public int fourth;
	public int fifth;
}

public class HighscoresPopup : Menu {
	public static HighscoresPopup Instance;
	void Awake()
	{Instance = this; base.Awake ();}

	public Text[] difficultyLabels;
	public Text[] scores;
	public Difficulty currentDifficulty;

	public Button leftButton;
	public Button rightButton;

	protected override void OnOpen()
	{
		SetupUI ();
	}

	public void SetupUI()
	{
		leftButton.interactable = true;
		rightButton.interactable = true;

		HighscoreSet scoreSet;
		switch(currentDifficulty) {
		case Difficulty.Easy:
			foreach (Text difficultyLabel in difficultyLabels)
				difficultyLabel.text = "Easy Difficulty";
			scoreSet = PlayerProgression.GetClassicModeScores ("easy");
			leftButton.interactable = false;
			break;
		case Difficulty.Medium:
			foreach(Text difficultyLabel in difficultyLabels)
				difficultyLabel.text = "Medium Difficulty";
			scoreSet = PlayerProgression.GetClassicModeScores ("medium");
			break;
		case Difficulty.Hard:
			foreach (Text difficultyLabel in difficultyLabels)
				difficultyLabel.text = "Hard Difficulty";
			scoreSet = PlayerProgression.GetClassicModeScores ("hard");
			rightButton.interactable = false;
			break;

		default:
			foreach(Text difficultyLabel in difficultyLabels)
				difficultyLabel.text = "??? Difficulty";
			scoreSet = new HighscoreSet ();
			break;
		}

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

	protected override void OnClose()
	{
		currentDifficulty = Difficulty.Easy;
	}

	public void OnLeftPressed()
	{
		currentDifficulty = (Difficulty)((int)currentDifficulty - 1);
		SetupUI ();
	}

	public void OnRightPressed()
	{
		currentDifficulty = (Difficulty)((int)currentDifficulty + 1);
		SetupUI ();
	}

	public void ClosePopup()
	{
		MenuController.Instance.CloseCurrent ();
	}

}
