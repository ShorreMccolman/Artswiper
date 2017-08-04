using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndgamePopup : Menu {
	public static EndgamePopup Instance;
	void Awake()
	{Instance = this; base.Awake ();}

	public GameObject ribbon;

	public GameObject actionButton;

	public GameObject[] fireworks;
	public GameObject flashes;

	public Text[] titleLabels;
	public Text label1;
	public Text value1;
	public Text label2;
	public Text value2;
	public Text label3;
	public Text value3;
	public Text label4;
	public Text value4;

	protected override void OnOpen()
	{
		ribbon.SetActive (false);

		SoundController.PauseMusic (3.0f);

		switch(GameController.CurrentState.currentMode) {
		case GameMode.Adventure:
			
			float T = GameController.CurrentState.victory ? HUD.FinalTime / GameController.CurrentBoard.map.timeLimit * 100 : 0;
			float D = GameController.CurrentBoard.map.pointsMultiplier;
			float N = (float)GameController.CurrentState.flippedSpaces / (float)GameController.CurrentBoard.slots.Length * 100;
			int bonus = GameController.CurrentState.victory ? 10 : 2;
			int points = (int)((T * T + N * N) * D) * bonus;

			foreach (Text title in titleLabels)
				title.text = GameController.CurrentState.victory ? "Swiped it!" : "Busted!";
			label1.text = "Total Points";
			value1.text = points.ToString ();
			label2.text = "Time Remaining";
			value2.text = UIHelpers.ConvertToSecondsTimeString(HUD.FinalTime);
			label3.text = "Mines Flagged Incorrectly";
			value3.text = GameController.CurrentState.mislocatedBombs.ToString ();
			label4.text = "";
			value4.text = "";

			if (GameController.CurrentState.victory) {
				GameController.CompleteLevel (points);
				actionButton.SetActive (true);

				if (GameController.IsFinalLevel ()) {
					actionButton.transform.GetChild (0).GetComponent<Text> ().text = "FINISH";
					PlayerProgression.CompleteWorld (GameController.CurrentWorld);
				} else {
					actionButton.transform.GetChild (0).GetComponent<Text> ().text = "NEXT LEVEL";
				}
				SoundController.PlaySoundEffect (Sounds.VICTORY);

				if(points >= GameController.CurrentBoard.map.pointTarget) {
					Invoke ("TargetReached", 2.5f);
				}
			} else {
				actionButton.SetActive (false);
				SoundController.PlaySoundEffect (Sounds.DEFEAT);
			}

			break;
		case GameMode.Classic:
			foreach(Text title in titleLabels)
				title.text = GameController.CurrentState.victory ? "Success!" : "Defeat!";
			
			actionButton.SetActive (true);
			actionButton.transform.GetChild(0).GetComponent<Text>().text = "CHANGE DIFFICULTY";

			label1.text = "Total Time";
			value1.text = UIHelpers.ConvertToSecondsTimeString(HUD.FinalTime);
			label2.text = "Mines Flagged Correctly";
			value2.text = GameController.CurrentState.locatedBombs.ToString ();
			label3.text = "Mines Flagged Incorrectly";
			value3.text = GameController.CurrentState.mislocatedBombs.ToString();
			label4.text = "Mines Remaining";
			value4.text = GameController.CurrentState.remainingBombs.ToString();

			SoundController.PlaySoundEffect (GameController.CurrentState.victory ? Sounds.VICTORY : Sounds.DEFEAT);

			if(GameController.CurrentState.victory)
				PlayerProgression.CompleteClassicMode (GameController.CurrentBoard.map.name, HUD.FinalTime);
			break;
		}

		foreach (GameObject work in fireworks)
			work.SetActive (GameController.CurrentState.victory);
	}

	public void Retry()
	{
		MenuController.CloseCurrent ();
		GameController.RestartCurrentBoard ();
	}

	public void ActionButtonPressed()
	{
		if(GameController.CurrentState.currentMode == GameMode.Classic) {
			ChangeDifficulty ();
		} else if (GameController.CurrentState.currentMode == GameMode.Adventure) {
			if (GameController.IsFinalLevel ()) {
				MenuController.OpenMenu (MainMenu.Instance);
				MenuController.OpenMenu (WorldMenu.Instance);
			} else {
				StartNextLevel ();
			}
		}
	}

	public void TargetReached()
	{
		ribbon.SetActive (true);
	}

	public void StartNextLevel()
	{
		GameController.StartNextAdventureGame ();
	}

	public void ChangeDifficulty()
	{
		MenuController.OpenMenu (DifficultyPopup.Instance);
	}

	public void QuitToMenu()
	{
		MenuController.OpenMenu (MainMenu.Instance);
	}

	public void ClosePopup()
	{
		MenuController.CloseCurrent ();
	}
}
