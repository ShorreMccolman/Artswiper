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

	public void SetVictory(bool isVict)
	{
		victory = isVict;
	}

	public void SetMode(GameMode mode)
	{
		currentMode = mode;
	}

	public void ModifyRemainingFlags(int change)
	{
		remainingFlags += change;
	}

	public void ModifyRemainingBombs(int change)
	{
		remainingBombs += change;
	}

	public void ModifyLocatedBombs(int change)
	{
		locatedBombs += change;
	}

	public void ModifyMislocatedBombs(int change)
	{
		mislocatedBombs += change;
	}
}

public class GameController : MonoBehaviour {
	public static GameController Instance;
	void Awake()
	{Instance = this;}

	private GameState currentState;
	public static GameState CurrentState
	{
		get{return Instance.currentState;}
		set{Instance.currentState = value;}
	}

	private Board currentBoard;
	public static Board CurrentBoard
	{
		get{return Instance.currentBoard;}
		set{Instance.currentBoard = value;}
	}

	private World currentWorld;
	public static World CurrentWorld
	{
		get{return Instance.currentWorld;}
		set{Instance.currentWorld = value;}
	}

	static Map mapToLoad;

	public static void CompleteLevel(int score)
	{
		PlayerProgression.CompleteLevel (CurrentBoard.map.name, score);
	}

	public static void RestartCurrentBoard()
	{
		if(CurrentState.currentMode == GameMode.Classic) {
			StartClassicGame (CurrentBoard.map.name);
		} else if (CurrentState.currentMode == GameMode.Adventure) {
			StartAdventureGame (CurrentWorld, CurrentBoard.map);
		}
	}

	public static void StartClassicGame(string level = "easy")
	{
		mapToLoad = Map.LoadMap(level);
		CurrentWorld = new World ();
		Instance.StartCoroutine (Instance.StartGameSequence (GameMode.Classic));
	}

	public static void StartAdventureGame(World world, Map map)
	{
		mapToLoad = map;
		CurrentWorld = world;
		Instance.StartCoroutine (Instance.StartGameSequence (GameMode.Adventure));
	}
	IEnumerator StartGameSequence(GameMode mode)
	{
		AppManager.Instance.HideScreen();
		yield return new WaitForSeconds (1.0f);
		float time = Time.time;

		MenuController.CloseAll ();
		currentBoard = BoardGenerator.Instance.CreateBoardFromMap(mapToLoad);
		currentState = new GameState (mode, currentBoard);
		while( Time.time - time < 1.0f)
			yield return null;
		mapToLoad = null;
		AppManager.Instance.RevealScreen ();
		HUD.Instance.StartGame (currentWorld);
	}

	public static void StartNextAdventureGame()
	{
		MenuController.CloseAll ();
		if(CurrentWorld.maps.Length > CurrentBoard.map.currentIndex + 1) {
			mapToLoad = CurrentWorld.maps [CurrentBoard.map.currentIndex + 1];
			Instance.StartCoroutine (Instance.StartGameSequence (GameMode.Adventure));
		}
	}

	public static bool IsFinalLevel()
	{
		return CurrentBoard.map.isFinalLevel;
	}

	public static void Timeout()
	{
		HUD.Running = false;
		BoardGenerator.Instance.FlipBoard ();
		MenuController.OpenMenu (EndgamePopup.Instance);
	}

	public static void FlagSpace(bool flagOn, bool hasBomb)
	{
		int sign = flagOn ? 1 : -1;

		CurrentState.ModifyRemainingFlags(-1 * sign);
		if(hasBomb) {
			CurrentState.ModifyLocatedBombs(1 * sign);
			CurrentState.ModifyRemainingBombs(-1 * sign);
		} else {
			CurrentState.ModifyMislocatedBombs(1 * sign);
		}
		HUD.Instance.UpdateRemaining (CurrentState.remainingFlags);
	}

	public static void HitBomb()
	{
		Instance.StartCoroutine (Instance.EndGameSequence ());
	}

	IEnumerator EndGameSequence()
	{
		HUD.Running = false;
		SoundController.PauseMusic ();
		yield return new WaitForSeconds (0.7f);
		SoundController.PlaySoundEffect (Sounds.ALARM);
		yield return new WaitForSeconds (1.8f);
		ShowEndGame ();
	}

	public static void ShowEndGame()
	{
		SoundController.UnPauseMusic ();
		MenuController.OpenMenu (EndgamePopup.Instance);
	}

	public static void FlipSafeSpace()
	{
		CurrentState.remainingSpaces--;
		CurrentState.flippedSpaces++;

		if(CurrentState.remainingSpaces <= 0 && HUD.Running) {
			HUD.Running = false;
			BoardGenerator.Instance.FlipBoard ();
			CurrentState.SetVictory (true);
			MenuController.OpenMenu (EndgamePopup.Instance);
		} else {
			if(!HUD.Running) {
				HUD.Instance.FlipFirst ();
			}
		}
	}

}
