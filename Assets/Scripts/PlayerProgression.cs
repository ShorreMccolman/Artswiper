using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerProgression {

	public const string PROFILE_CREATED = "prfl_crtd";

	public const string GREETING_TITLE = "Welcome to Artswiper!";
	public const string GREETING_MESSAGE = "The day of the big gallery heist is today but alas, a new security system has thrown a hitch in our plan. Before you can swipe a painting you will need" +
		"to reveal the blocks covering it. Left click on a block to reveal it but be careful, some blocks are hiding alarms! Accidently uncover one of these alarms and the score is off." +
		"\n\nI have gone ahead and marked each painting with a safe entry point but from there it will be all up to you. Blocks that have been revealed will indicate how many adjacent blocks" +
		"contain alarms. If you think a block is hiding an alarm, you can mark it with right click. Once all of the blocks without alarms have been revealed, the painting will be yours!";
	public const string GREETING_VIEWED = "grtng_vwd";

	public const string LEVELS_COMPLETED = "lvls_cpltd";
	public const string ALARMS_TRIGGERED = "alrms_trggrd";
	public const string POINTS_ACCUMULATED = "pnts_ccmltd";
	public const string CLASSIC_WINS = "clssc_wns";
	public const string NUMBER_7s = "num_svn";

	public static void SetFlag(string flag)
	{
		PlayerPrefs.SetInt (flag, 1);
		PlayerPrefs.Save ();
	}

	public static bool HasFlag(string flag)
	{
		return PlayerPrefs.HasKey (flag);
	}

	public static bool ProfileExists()
	{
		return PlayerPrefs.HasKey (PROFILE_CREATED);
	}

	public static void CreateNewProfile()
	{
		PlayerPrefs.SetInt (PROFILE_CREATED, 1);
	}

	public static bool CompleteClassicMode(string level, float time)
	{
		bool newScore = false;

		int newTime = (int)(time * 10);

		for(int i=0; i<5; i++) {
			string key = level + "_scr_" + i;
			int curTime = PlayerPrefs.GetInt (key, -1);
			if (curTime < 0 || newTime < curTime) {
				PlayerPrefs.SetInt (key, newTime);
				newTime = curTime;
				newScore = true;
			}
		}

		AddToPref (CLASSIC_WINS);

		PlayerPrefs.Save ();
			
		return newScore;
	}

	public static int GetClassicWins()
	{
		return PlayerPrefs.GetInt (CLASSIC_WINS, 0);
	}

	public static HighscoreSet GetClassicModeScores(string level)
	{
		HighscoreSet scores = new HighscoreSet ();
		scores.first = PlayerPrefs.GetInt (level + "_scr_0", -1);
		scores.second = PlayerPrefs.GetInt (level + "_scr_1", -1);
		scores.third = PlayerPrefs.GetInt (level + "_scr_2", -1);
		scores.fourth = PlayerPrefs.GetInt (level + "_scr_3", -1);
		scores.fifth = PlayerPrefs.GetInt (level + "_scr_4", -1);

		return scores;
	}

	public static bool CompleteLevel(string level, int score)
	{
		if(GetBestScore(level) <= 0) {
			int completed = PlayerPrefs.GetInt (LEVELS_COMPLETED, 0);
			completed++;
			PlayerPrefs.SetInt (LEVELS_COMPLETED, completed);
		}

		PlayerPrefs.SetInt (level, 1);

		int best = GetBestScore (level);
		if(score > best) {
			PlayerPrefs.SetInt (level + "_scr", score);
			return true;
		}

		return false;
	}

	public static bool HasCompletedLevel(string level)
	{
		if (string.IsNullOrEmpty (level))
			return true;

		return PlayerPrefs.GetInt (level, 0) == 1;
	}

	public static int GetLevelsCompleted()
	{
		return PlayerPrefs.GetInt (LEVELS_COMPLETED, 0);
	}

	public static int GetLevelsCompleted(World world)
	{
		int val = 0;
		foreach(Map map in world.maps) {
			if (HasCompletedLevel (map.name))
				val++;
		}
		return val;
	}

	public static int GetMedalsCompleted(World world)
	{
		int val = 0;
		foreach(Map map in world.maps) {
			int score = GetBestScore (map.name);
			if (score >= map.pointTarget)
				val++;
		}
		return val;
	}

	public static void CompleteWorld(World world)
	{
		PlayerPrefs.SetInt (world.name + "_cmplt", 1);
		PlayerPrefs.Save ();
	}

	public static bool HasCompletedWorld(World world)
	{
		if (world == null)
			return true;

		return PlayerPrefs.GetInt (world.name + "_cmplt", 0) == 1;
	}

	public static void TriggerBomb()
	{
		AddToPref (ALARMS_TRIGGERED);
	}

	public static int GetBombsTriggered()
	{
		return PlayerPrefs.GetInt (ALARMS_TRIGGERED, 0);
	}

	public static int GetBestScore(string level)
	{
		if (string.IsNullOrEmpty (level))
			return 0;

		return PlayerPrefs.GetInt (level + "_scr", 0);
	}

	public static bool HasEnoughPoints(string world, int points)
	{
		return PlayerPrefs.GetInt (world, 0) >= points;
	}

	public static void FlipNumber7()
	{
		AddToPref (NUMBER_7s);
	}

	public static int Number7sFlipped()
	{
		return PlayerPrefs.GetInt (NUMBER_7s, 0);
	}
		
	public static bool SetBestPref(string pref, int score)
	{
		int val = PlayerPrefs.GetInt (pref, 0);
		if(score > val) {
			PlayerPrefs.SetInt (pref, score);
			return true;
		}
		return false;
	}
		
	public static void AddToPref(string pref, int additions = 1)
	{
		int val = PlayerPrefs.GetInt (pref, 0);
		val += additions;
		PlayerPrefs.SetInt (pref, val);
	}
}
