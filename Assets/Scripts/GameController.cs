using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board
{
	public Map map;
	public Slot[] slots;
	public int remainingSpaces;
}

public enum GameMode
{
	None,
	Classic,
	Adventure
}

public struct GameState {
	public GameMode currentMode;
	public int remainingFlags;
	public int remainingSpaces;
	public int flippedSpaces;
	public int remainingBombs;
	public int locatedBombs;
	public int mislocatedBombs;
	public bool victory;

	public GameState(GameMode mode, Board board) {
		currentMode = mode;
		remainingFlags = board.map.difficulty;
		remainingSpaces = board.remainingSpaces;
		remainingBombs = board.map.difficulty;
		flippedSpaces = 0;
		locatedBombs = 0;
		mislocatedBombs = 0;
		victory = false;
	}
}

public class GameController : MonoBehaviour {
	public static GameController Instance;
	void Awake()
	{Instance = this;}

	public GameState currentState;
	public Board currentBoard;
	public World currentWorld;
	Map mapToLoad;

	public void CompleteLevel(int score)
	{
		PlayerProgression.CompleteLevel (currentBoard.map.name, score);
	}

	public void RestartCurrentBoard()
	{
		if(currentState.currentMode == GameMode.Classic) {
			StartClassicGame (currentBoard.map.name);
		} else if (currentState.currentMode == GameMode.Adventure) {
			StartAdventureGame (currentWorld, currentBoard.map);
		}
	}

	public void StartClassicGame(string level = "easy")
	{
		mapToLoad = BoardGenerator.Instance.LoadMap(level);
		currentWorld = new World ();
		StartCoroutine (StartGameSequence (GameMode.Classic));
	}

	public void StartAdventureGame(World world, Map map)
	{
		mapToLoad = map;
		currentWorld = world;
		StartCoroutine (StartGameSequence (GameMode.Adventure));
	}
	IEnumerator StartGameSequence(GameMode mode)
	{
		AppManager.Instance.HideScreen();
		yield return new WaitForSeconds (1.0f);
		float time = Time.time;

		MenuController.Instance.CloseAll ();
		currentBoard = BoardGenerator.Instance.CreateBoardFromMap(mapToLoad);
		currentState = new GameState (mode, currentBoard);
		while( Time.time - time < 1.0f)
			yield return null;
		mapToLoad = null;
		AppManager.Instance.RevealScreen ();
		HUD.Instance.StartGame (currentWorld);
	}

	public void StartNextAdventureGame()
	{
		MenuController.Instance.CloseAll ();
		if(currentWorld.maps.Length > currentBoard.map.currentIndex + 1) {
			mapToLoad = currentWorld.maps [currentBoard.map.currentIndex + 1];
			//Debug.LogError ("Name: " + mapToLoad.name + " INDEX: " + currentBoard.map.currentIndex);
			StartCoroutine (StartGameSequence (GameMode.Adventure));
		}
	}

	public bool IsFinalLevel()
	{
		return currentBoard.map.isFinalLevel;
	}

	public void Timeout()
	{
		HUD.Instance.running = false;
		BoardGenerator.Instance.FlipBoard ();
		MenuController.Instance.OpenMenu (EndgamePopup.Instance);
	}

	public void FlagSpace(bool flagOn, bool hasBomb)
	{
		int sign = flagOn ? 1 : -1;

		currentState.remainingFlags += -1 * sign;
		if(hasBomb) {
			currentState.locatedBombs += 1 * sign;
			currentState.remainingBombs += -1 * sign;
		} else {
			currentState.mislocatedBombs += 1 * sign;
		}
		HUD.Instance.UpdateRemaining (currentState.remainingFlags);
	}

	public void HitBomb()
	{
		StartCoroutine (EndGameSequence ());
	}

	IEnumerator EndGameSequence()
	{
		HUD.Instance.running = false;
		SoundController.Instance.PauseMusic ();
		yield return new WaitForSeconds (0.7f);
		SoundController.Instance.PlaySoundEffect (Sounds.ALARM);
		yield return new WaitForSeconds (1.8f);
		ShowEndGame ();
	}

	public void ShowEndGame()
	{
		SoundController.Instance.UnPauseMusic ();
		MenuController.Instance.OpenMenu (EndgamePopup.Instance);
	}

	public void FlipSafeSpace()
	{
		currentState.remainingSpaces--;
		currentState.flippedSpaces++;

		if(currentState.remainingSpaces <= 0 && HUD.Instance.running) {
			HUD.Instance.running = false;
			BoardGenerator.Instance.FlipBoard ();
			currentState.victory = true;
			MenuController.Instance.OpenMenu (EndgamePopup.Instance);
		} else {
			if(!HUD.Instance.running) {
				HUD.Instance.FlipFirst ();
			}
		}
	}

}
