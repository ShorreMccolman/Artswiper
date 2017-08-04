using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {
	public static HUD Instance;
	void Awake()
	{Instance = this;}

	public Image feature;
	public Image frame;

	public Text remainingLabel;
	public Text time;

	private bool running;
	public static bool Running
	{
		get{return Instance.running;}
		set{Instance.running = value;}
	}

	float startTime;
	float endTime;
	bool usesLimit;

	int tick;

	private float finalTime;
	public static float FinalTime
	{
		get{return Instance.finalTime;}
		set{Instance.finalTime = value;}
	}

	public void StartGame (World world) {

		int limit = GameController.CurrentBoard.map.timeLimit;
		usesLimit = limit > 0;
		running = false;
		tick = 10;
		time.color = Color.green;
		time.text = usesLimit ?  UIHelpers.ConvertToMillisecondsTimeString((float)limit) : "00:00:00";
	}

	public void FlipFirst()
	{
		running = true;
		int limit = GameController.CurrentBoard.map.timeLimit;
		startTime = Time.time;
		endTime = Time.time + (float)limit;
	}

	public void RestartGame()
	{
		GameController.RestartCurrentBoard ();
	}

	void Update () {
		if (running) {
			if(usesLimit) {
				finalTime = endTime - Time.time;

				if(finalTime < (float)tick + 0.05f) {
					if(tick == 0) {
						
					} else {
						SoundController.PlaySoundEffect (Sounds.TICK);
					}
					tick--;
				}

				if(finalTime <= 10f) {
					time.color = Color.red;
				}

				time.text = UIHelpers.ConvertToMillisecondsTimeString(finalTime);
				if(Time.time > endTime) {
					running = false;
					time.text = "00:00:00";
					GameController.Timeout ();
				}
			} else {
				finalTime = Time.time - startTime;
				time.text = UIHelpers.ConvertToMillisecondsTimeString(finalTime);
			}
		}
	}

	public void UpdateRemaining(int flags)
	{
		int remaining = flags;
		remainingLabel.text = remaining.ToString();
	}

	public void OpenSettingsPopup()
	{
		MenuController.OpenMenu (SettingsPopup.Instance, false);
	}

	public void OpenAboutPopup()
	{
		
	}
}
