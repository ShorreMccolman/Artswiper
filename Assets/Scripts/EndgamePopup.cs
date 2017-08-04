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
		GameState state = GameController.Instance.currentState;
		Board board = GameController.Instance.currentBoard;

		ribbon.SetActive (false);

		SoundController.PauseMusic (3.0f);

		switch(state.currentMode) {
		case GameMode.Adventure:
			
			float T = state.victory ? HUD.Instance.finalTime / GameController.Instance.currentBoard.map.timeLimit * 100 : 0;
			float D = GameController.Instance.currentBoard.map.pointsMultiplier;
			float N = (float)GameController.Instance.currentState.flippedSpaces / (float)GameController.Instance.currentBoard.slots.Length * 100;
			int bonus = state.victory ? 10 : 2;
			int points = (int)((T * T + N * N) * D) * bonus;

			foreach (Text title in titleLabels)
				title.text = state.victory ? "Swiped it!" : "Busted!";
			label1.text = "Total Points";
			value1.text = points.ToString ();
			label2.text = "Time Remaining";
			value2.text = UIHelpers.ConvertToSecondsTimeString(HUD.Instance.finalTime);
			label3.text = "Mines Flagged Incorrectly";
			value3.text = state.mislocatedBombs.ToString ();
			label4.text = "";
			value4.text = "";

			if (state.victory) {
				GameController.Instance.CompleteLevel (points);
				actionButton.SetActive (true);

				if (GameController.Instance.IsFinalLevel ()) {
					actionButton.transform.GetChild (0).GetComponent<Text> ().text = "FINISH";
					PlayerProgression.CompleteWorld (GameController.Instance.currentWorld);
				} else {
					actionButton.transform.GetChild (0).GetComponent<Text> ().text = "NEXT LEVEL";
				}
				SoundController.PlaySoundEffect (Sounds.VICTORY);

				if(points >= board.map.pointTarget) {
					Invoke ("TargetReached", 2.5f);
				}
			} else {
				actionButton.SetActive (false);
				SoundController.PlaySoundEffect (Sounds.DEFEAT);
			}

			break;
		case GameMode.Classic:
			foreach(Text title in titleLabels)
				title.text = state.victory ? "Success!" : "Defeat!";
			
			actionButton.SetActive (true);
			actionButton.transform.GetChild(0).GetComponent<Text>().text = "CHANGE DIFFICULTY";

			label1.text = "Total Time";
			value1.text = UIHelpers.ConvertToSecondsTimeString(HUD.Instance.finalTime);
			label2.text = "Mines Flagged Correctly";
			value2.text = state.locatedBombs.ToString ();
			label3.text = "Mines Flagged Incorrectly";
			value3.text = state.mislocatedBombs.ToString();
			label4.text = "Mines Remaining";
			value4.text = state.remainingBombs.ToString();

			SoundController.PlaySoundEffect (state.victory ? Sounds.VICTORY : Sounds.DEFEAT);

			if(state.victory)
				PlayerProgression.CompleteClassicMode (GameController.Instance.currentBoard.map.name, HUD.Instance.finalTime);
			break;
		}

		foreach (GameObject work in fireworks)
			work.SetActive (state.victory);
	}

	public void Retry()
	{
		MenuController.CloseCurrent ();
		GameController.Instance.RestartCurrentBoard ();
	}

	public void ActionButtonPressed()
	{
		GameState state = GameController.Instance.currentState;
		if(state.currentMode == GameMode.Classic) {
			ChangeDifficulty ();
		} else if (state.currentMode == GameMode.Adventure) {
			if (GameController.Instance.IsFinalLevel ()) {
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
		GameController.Instance.StartNextAdventureGame ();
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
